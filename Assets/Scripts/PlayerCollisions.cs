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

    private void OnTriggerStay(Collider collider)
    {
        if (Input.GetKeyDown(KeyCode.E)) {
            if (collider.gameObject.GetComponent(typeof(Item)))
            {
                gameObject.AddComponent(collider.gameObject.GetComponent(typeof(Item)).GetType());
                Item hitItem = collider.gameObject.GetComponent(typeof(Item)) as Item;
                inventoryManager.pickUpItem(hitItem);
                itemsHeld.Add(hitItem.itemID);
                (GetComponent(typeof(Item)) as Item).setItemProperties(hitItem.itemID, hitItem.prefab, hitItem.menuSprite, hitItem.description);
                Destroy(collider.gameObject);
            }
            else if (collider.gameObject.GetComponent(typeof(Door)))
            {
                StartCoroutine((collider.gameObject.GetComponent(typeof(Door)) as Door).toggleDoor());
            }
        }
    }
}
