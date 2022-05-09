using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    Animator anim;

    [HideInInspector]
    public List<string> itemsHeld = new List<string>();

    public UIManager uiManager;
    public InventoryManager inventoryManager;
    public PopUpManager popUpManager;
    CancellationTokenSource destroyPathTokenSource;

    Camera camera;
    [SerializeField]
    private float hitRange;

    Door hitDoor;
    Item hitItem;

    bool startCalled = false;

    public event EventHandler onNextLevel;

    bool foundPhoto, foundLadder, foundRocket, foundKite = false;

    public AudioSource audioSource;
    public AudioClip positiveSound;
    public AudioClip negativeSound;

    private void Start()
    {
        camera = Camera.main;
        //anim = this.transform.parent.GetComponent<Animator>();
        onNextLevel += popUpManager.spawnPlatformLink;
        startCalled = true;
    }

    private void Update()
    {
        if (startCalled == false)
        {
            Start();
            startCalled = true;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            
            if (hitDoor)
            {
                hitDoor.toggleDoor();
            }
            else if (Physics.Raycast(camera.transform.position, camera.transform.TransformDirection(Vector3.forward), out hit, hitRange))
            {
                if (hit.transform.gameObject.GetComponent(typeof(Item)))
                {
                    hitItem = hit.transform.gameObject.GetComponent(typeof(Item)) as Item;
                    inventoryManager.pickUpItem(hitItem);
                    itemsHeld.Add(hitItem.itemID);
                    gameObject.AddComponent(hitItem.GetType());
                    audioSource.clip = positiveSound;
                    audioSource.Play();
                    CollectLevelOneItems();
                    uiManager.collectedObjectText.enabled = true;
                    (GetComponent(typeof(Item)) as Item).setItemProperties(hitItem.itemID, hitItem.prefab, hitItem.menuSprite, hitItem.description);
                    audioSource.Play();

                    if (hitItem.triggersPath)
                    {
                        popUpManager.obstacleTime = true;
                        popUpManager.generatePath(4);
                    }

                    inventoryManager.pickUpItem(hitItem);
                    popUpManager.itemPickedUp = true;
                    Destroy(hitItem.gameObject);
                    hitItem = null;
                }
            } else {
                audioSource.clip = negativeSound;
                audioSource.Play();
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

            Debug.Log("hit");

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
                //anim.SetBool("isOpening", true);
                //And trigger "Ascend blocks" UI
            }
        }
    }

    void CollectLevelOneItems()
    {
        switch (hitItem.itemID)
        {
            case "Ladder":
                uiManager.findObject2Text.enabled = false;
                uiManager.collectedObjectText.text = "Collects " + hitItem.itemID;
                StartCoroutine(HideText());
                foundLadder = true;
                break;
            case "Rocket":
                uiManager.findObject3Text.enabled = false;
                uiManager.collectedObjectText.text = "Collects " + hitItem.itemID;
                foundRocket = true;
                StartCoroutine(HideText());
                break;
            case "Kite":
                uiManager.findObject4Text.enabled = false;
                uiManager.collectedObjectText.text = "Collects " + hitItem.itemID;
                foundKite = true;
                StartCoroutine(HideText());
                break;
            case "Photo": //should be photo
                uiManager.findObject1Text.enabled = false;
                uiManager.collectedObjectText.text = "Collects " + hitItem.itemID;
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
