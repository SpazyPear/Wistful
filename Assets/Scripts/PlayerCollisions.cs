using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    [HideInInspector]
    public List<string> itemsHeld = new List<string>();

    public UIManager uiManager;
    public InventoryManager inventoryManager;
    public PopUpManager popUpManager;
    CancellationTokenSource destroyPathTokenSource;

    Door hitDoor;
    Item hitItem;

    public event EventHandler onNextLevel;

    bool foundPhoto, foundLadder, foundRocket, foundKite = false;

    private void Start()
    {
        onNextLevel += popUpManager.spawnPlatformLink;

        onNextLevel += popUpManager.incrementDataStructures;
    }

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
                (GetComponent(typeof(Item)) as Item).setItemProperties(hitItem.itemID, hitItem.prefab, hitItem.menuSprite, hitItem.description);

                if (hitItem.triggersPath)
                {
                    popUpManager.obstacleTime = true;
                    popUpManager.itemPickedUp = true;
                    popUpManager.generatePath(4);
                    inventoryManager.pickUpItem(hitItem);
                }

                Destroy(hitItem.gameObject);
                hitItem = null;
            }

            
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.GetComponent(typeof(Item)))
        {
            hitItem = collider.gameObject.GetComponent(typeof(Item)) as Item;
        }
        if (collider.gameObject.tag.Equals("pathEdge"))
        {
            if (destroyPathTokenSource != null)
                destroyPathTokenSource.Cancel();

            popUpManager.obstacleTime = false;
            popUpManager.popBiome();
            popUpManager.riseBlocks();
        }
        if (collider.gameObject.GetComponent(typeof(Door)))
        {
            if (collider.gameObject.GetComponent<Door>().isLocked && !itemsHeld.Contains("Key"))
                return;

            hitDoor = collider.gameObject.GetComponent(typeof(Door)) as Door;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.GetComponent(typeof(Item)))
        {
            hitItem = null;
        }
        if (collider.gameObject.GetComponent(typeof(Door)))
        {
            hitDoor = null;
        }
        if (collider.gameObject.tag.Equals("pathEdge"))
        {
            if (destroyPathTokenSource != null)
                destroyPathTokenSource.Cancel();
            
            destroyPathTokenSource = new CancellationTokenSource();
            var destroyPathToken = destroyPathTokenSource.Token;
            popUpManager.destroyPath(destroyPathToken);
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.tag.Equals("Vault Door"))
        {
            if (Input.GetKeyDown(KeyCode.E)) {
                onNextLevel.Invoke(this, new EventArgs());
                collider.tag = "Untagged";
            }
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
