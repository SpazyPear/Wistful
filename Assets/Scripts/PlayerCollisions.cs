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
    public PopUpManager popUpManager;

    Door hitDoor;
    Item hitItem;

    Camera camera;
    [SerializeField]
    private float hitRange;

    bool foundPhoto, foundLadder, foundRocket, foundKite = false;
    private void Start()
    {
        camera = Camera.main;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (hitDoor)
            {
                hitDoor.toggleDoor();
            }
            else if (Physics.Raycast(transform.position, camera.transform.TransformDirection(Vector3.forward), out hit, hitRange))
            {
                if (hit.transform.gameObject.GetComponent(typeof(Item)))
                {
                    hitItem = hit.transform.gameObject.GetComponent(typeof(Item)) as Item;
                    inventoryManager.pickUpItem(hitItem);
                    itemsHeld.Add(hitItem.itemID);
                    gameObject.AddComponent(hitItem.GetType());
                    CollectLevelOneItems();
                    //uiManager.collectedObjectText.enabled = true;
                    (GetComponent(typeof(Item)) as Item).setItemProperties(hitItem.itemID, hitItem.prefab, hitItem.menuSprite, hitItem.description);
                    Destroy(hitItem.gameObject);
                    hitItem = null;
                    popUpManager.obstacleTime = true;
                }
            }
            //else if (hitItem)
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
        switch (hitItem.gameObject.name)
        {
            case "Stairs":
                uiManager.findObject2Text.enabled = false;
                uiManager.collectedObjectText.text = "Collects Ladder";
                StartCoroutine(HideText());
                foundLadder = true;
                break;
            case "Rocket":
                uiManager.findObject3Text.enabled = false;
                uiManager.collectedObjectText.text = "Collects " + hitItem.gameObject.name;
                foundRocket = true;
                StartCoroutine(HideText());
                break;
            case "KitePrefab":
                uiManager.findObject4Text.enabled = false;
                uiManager.collectedObjectText.text = "Collects Kite";
                foundKite = true;
                StartCoroutine(HideText());
                break;
            case "Object029": //should be photo
                uiManager.findObject1Text.enabled = false;
                uiManager.collectedObjectText.text = "Collects " + hitItem.gameObject.name;
                foundPhoto = true;
                StartCoroutine(HideText());
                break;
        }
        if (foundKite && foundLadder && foundPhoto && foundRocket)
        {
            uiManager.collectedObjectText.text = "Go to the Vault";
            uiManager.collectedObjectText.fontSize = 24;
        }
    }
    IEnumerator HideText()
    {
        yield return new WaitForSeconds(3);
        uiManager.HideText();
    }
}
