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

    Door hitDoor;
    Item hitItem;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (hitDoor)
                hitDoor.toggleDoor();

            else if (hitItem)
            {
                inventoryManager.pickUpItem(hitItem);
                itemsHeld.Add(hitItem.itemID);
                gameObject.AddComponent(hitItem.GetType());
                (GetComponent(typeof(Item)) as Item).setItemProperties(hitItem.itemID, hitItem.prefab, hitItem.menuSprite, hitItem.description);
                Destroy(hitItem.gameObject);
                hitItem = null;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent(typeof(Door)))
        {
            hitDoor = collision.gameObject.GetComponent(typeof(Door)) as Door;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent(typeof(Door)))
        {
            hitDoor = null;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.GetComponent(typeof(Item)))
        {
            hitItem = collider.gameObject.GetComponent(typeof(Item)) as Item;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.GetComponent(typeof(Item)))
        {
            hitItem = null;
        }
    }
}
