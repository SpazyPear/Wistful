using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    [HideInInspector]
    public List<string> itemsHeld = new List<string>();

    public UIManager uiManager;
    public InventoryManager inventoryManager;
    public Controls controls;

    Door lastDoor;


    private void Update()
    {
        if (controls.interactDown && lastDoor)
        {
            lastDoor.toggleDoor();
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (controls.interactDown) {
            if (collider.gameObject.GetComponent(typeof(Item)))
            {
                gameObject.AddComponent(collider.gameObject.GetComponent(typeof(Item)).GetType());
                Item hitItem = collider.gameObject.GetComponent(typeof(Item)) as Item;
                inventoryManager.pickUpItem(hitItem);
                itemsHeld.Add(hitItem.itemID);
                (GetComponent(typeof(Item)) as Item).setItemProperties(hitItem.itemID, hitItem.prefab, hitItem.menuSprite, hitItem.description);
                Destroy(collider.gameObject);
            }
            
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent(typeof(Door)))
        {
            lastDoor = collision.gameObject.GetComponent(typeof(Door)) as Door;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent(typeof(Door)))
        {
            lastDoor = null;
        }
    }
}
