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

    bool foundPhoto = false;
    bool foundLadder = false;

    bool foundRocket = false;
    bool foundKite = false;

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
                CollectLevelOneItems();
                uiManager.collectedObjectText.enabled = true;
                 uiManager.collectedObjectText.text = "Collects " + hitItem.gameObject.name; 
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
            if (collision.gameObject.GetComponent<Door>().isLocked && !itemsHeld.Contains("Key"))
                return;

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

    void CollectLevelOneItems()
    {
        switch(hitItem.gameObject.name)
                {
                    case "Stairs":
                    uiManager.findObject2Text.enabled = false;
                    foundLadder=true;
                    break;
                    case "Rocket":
                    uiManager.findObject3Text.enabled = false;
                    foundRocket=true;
                    break;
                    case "KitePrefab":
                    uiManager.findObject4Text.enabled = false;
                    foundKite = true;
                    break;
                    case "Object029": //should be photo
                    uiManager.findObject1Text.enabled = false;
                    foundPhoto = true;
                    break;

                }
        if(foundKite && foundLadder && foundPhoto && foundRocket)
        {
             uiManager.collectedObjectText.text = "Go and find the Vault"; 
              uiManager.collectedObjectText.fontSize = 19;
        }
    }
}
