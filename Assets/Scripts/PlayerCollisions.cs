using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    Animator anim;

    [HideInInspector]
    public List<string> itemsHeld = new List<string>();

    public LevelManager levelManager;
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

    bool foundPhoto, foundLadder, foundRocket, foundKite, foundCrowbar, foundBook, foundSolarSystem, foundStickyNote, foundKey, foundPC = false;
    public AudioSource audioSource;
    public AudioClip positiveSound;
    public AudioClip negativeSound;
    public AudioClip staticSound;

    private void Start()
    {
        camera = Camera.main;
        //anim = this.transform.parent.GetComponent<Animator>();
        onNextLevel += popUpManager.spawnLevelLink;
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
            if (Physics.Raycast(camera.transform.position, camera.transform.TransformDirection(Vector3.forward), out hit, hitRange))
            {
                if (hit.transform.gameObject.GetComponent(typeof(Item)))
                {
                    hitItem = hit.transform.gameObject.GetComponent(typeof(Item)) as Item;
                    itemsHeld.Add(hitItem.itemID);
                    gameObject.AddComponent(hitItem.GetType());
                    audioSource.clip = positiveSound;
                    audioSource.Play();
                    if (SceneManager.GetActiveScene().name == "Level 1")
                    {
                        CollectLevelOneItems();
                    }
                    if (SceneManager.GetActiveScene().name == "Level 2")
                    {
                        CollectLevelTwoItems();
                    }
                    if (SceneManager.GetActiveScene().name == "Level3")
                    {
                        CollectLevelThreeItems();
                    }
                //uiManager.collectedObjectText = true;
                (GetComponent(typeof(Item)) as Item).setItemProperties(hitItem.itemID, hitItem.prefab, hitItem.menuSprite, hitItem.description);
                    audioSource.Play();

                    if (hitItem.triggersPath)
                    {
                        popUpManager.obstacleTime = true;
                        popUpManager.generatePath(4);
                    }

                    if (hitItem.triggersNextItem)
                    {
                        hitItem = hit.transform.gameObject.GetComponent(typeof(Item)) as Item;
                        itemsHeld.Add(hitItem.itemID);
                        gameObject.AddComponent(hitItem.GetType());
                        audioSource.clip = positiveSound;
                        audioSource.Play();
                        (GetComponent(typeof(Item)) as Item).setItemProperties(hitItem.itemID, hitItem.prefab, hitItem.menuSprite, hitItem.description);
                        audioSource.Play();

                        if (hitItem.triggersPath)
                        {
                            popUpManager.obstacleTime = true;
                            popUpManager.generatePath(4);
                        }

                        if (hitItem.triggersNextItem)
                        {
                            popUpManager.readyForNextItemSpawn = true;
                        }

                        inventoryManager.pickUpItem(hitItem);
                        Destroy(hitItem.gameObject);
                        hitItem = null;
                        uiManager.updateInteractPrompt("");
                    }
                }
                if (hit.transform.gameObject.GetComponent(typeof(Door)))
                {
                    hitDoor = hit.transform.gameObject.GetComponent(typeof(Door)) as Door;
                    if (hitDoor.isLocked && !itemsHeld.Contains("Key"))
                        return;
                    hitDoor.toggleDoor();
                    uiManager.updateInteractPrompt("");
                }
            }

        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.GetComponent(typeof(Item)))
        {
            hitItem = collider.gameObject.GetComponent(typeof(Item)) as Item;
            uiManager.updateInteractPrompt("Press E to Interact");
        }
        if (collider.gameObject.GetComponent(typeof(Door)))
        {
            hitDoor = collider.gameObject.GetComponent(typeof(Door)) as Door;
            uiManager.updateInteractPrompt("Press E to Interact");
        }
        if (collider.gameObject.tag.Equals("pathEdge"))
        {
            if (destroyPathTokenSource != null)
                destroyPathTokenSource.Cancel();

            popUpManager.obstacleTime = false;
            popUpManager.popBiome();
            popUpManager.riseBlocks();
        }
        if (collider.gameObject.tag.Equals("levelEnd"))
        {
            collider.gameObject.tag = "Untagged";
            if (staticSound)
            {
                audioSource.clip = staticSound;
                audioSource.Play();
            }
            levelManager.nextLevel();
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.GetComponent(typeof(Item)))
        {
            hitItem = null;
            uiManager.updateInteractPrompt("");
        }
        if (collider.gameObject.GetComponent(typeof(Door)))
        {
            hitDoor = null;
            uiManager.updateInteractPrompt("");
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


    void CollectLevelOneItems()
    {
            if (hitItem.itemID == "Ladder")
            {
                uiManager.findObject2Text.enabled = false;
                //uiManager.collectedObjectText.text = "Collects " + hitItem.itemID;
                //StartCoroutine(HideText());
                foundLadder = true;
            }
            if (hitItem.itemID == "Rocket")
            {
                uiManager.findObject3Text.enabled = false;
                //uiManager.collectedObjectText.text = "Collects " + hitItem.itemID;
                foundRocket = true;
                //StartCoroutine(HideText());
            }
            if (hitItem.itemID == "Kite")
            {
                uiManager.findObject4Text.enabled = false;
                //uiManager.collectedObjectText.text = "Collects " + hitItem.itemID;
                foundKite = true;
                //StartCoroutine(HideText());
            }
            if (hitItem.itemID == "Photo")
            {
                uiManager.findObject1Text.enabled = false;
                //uiManager.collectedObjectText.text = "Collects " + hitItem.itemID;
                foundPhoto = true;
                //StartCoroutine(HideText());
            }
        if (foundKite && foundLadder && foundPhoto && foundRocket)
        {
            uiManager.collectedObjectText.text = "Go to the Vault";
            StartCoroutine(uiManager.HideText());
        }
    }

    void CollectLevelTwoItems()
    {
            if (hitItem.itemID == "Crowbar")
            {
                uiManager.findObject1Text.enabled = false;
                //uiManager.collectedObjectText.text = "Collects " + hitItem.itemID;
                foundCrowbar = true;
                //StartCoroutine(HideText());
            }
             if (hitItem.itemID == "Book")
            {
                uiManager.findObject3Text.enabled = false;
                //uiManager.collectedObjectText.text = "Collects " + hitItem.itemID;
                foundBook = true;
                //StartCoroutine(HideText());
            }
             if (hitItem.itemID == "SolarSystem")
            {
                uiManager.findObject2Text.enabled = false;
                uiManager.findObject4Text.enabled = false;
                //uiManager.collectedObjectText.text = "Collects " + hitItem.itemID;
                foundSolarSystem = true;
                //StartCoroutine(HideText());
            }
        if (foundCrowbar && foundBook && foundSolarSystem)
        {
            uiManager.collectedObjectText.text = "Solve the puzzle";
            StartCoroutine(uiManager.HideText());
        }
    }
     void CollectLevelThreeItems()
    {
             if (hitItem.itemID == "StickyNote")
            {
                uiManager.findObject1Text.enabled = false;
                uiManager.findObject4Text.enabled = false;
                //uiManager.collectedObjectText.text = "Collects " + hitItem.itemID;
                foundStickyNote = true;
                //StartCoroutine(HideText());
            }
             if (hitItem.itemID == "Key")
            {
                uiManager.findObject2Text.enabled = false;
                //uiManager.collectedObjectText.text = "Collects " + hitItem.itemID;
                foundKey = true;
                //StartCoroutine(HideText());
            }
            if(!uiManager.findObject3Text.enabled)
            {
                foundPC = true;
            }
        if (foundStickyNote && foundKey && foundPC && uiManager.goToVaultlvl3)
        {
            uiManager.collectedObjectText.text = "Go to the Vault";
        }
    }
}
