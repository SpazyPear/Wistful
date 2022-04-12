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
        if (Input.GetKeyDown(KeyCode.E) && collider.gameObject.GetComponents(typeof(Item)).Length > 0)
        {
            inventoryManager.pickUpItem((collider.gameObject.GetComponent(typeof(Item))).GetType());
            itemsHeld.Add((collider.gameObject.GetComponent(typeof(Item)) as Item).itemID);
            Debug.Log(itemsHeld[itemsHeld.Count - 1]);
            Destroy(collider.gameObject);
        }
    }

}
