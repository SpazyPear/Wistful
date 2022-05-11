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

    public FallingBlocks fallingBlocks;

    public event EventHandler onNextLevel;

    bool foundPhoto, foundLadder, foundRocket, foundKite = false;

    public AudioSource audioSource;
    public AudioClip positiveSound;
    public AudioClip negativeSound;

    private void Start()
    {
        camera = Camera.main;
        //anim = this.transform.parent.GetComponent<Animator>();
        onNextLevel += popUpManager.spawnLevelLink;
        startCalled = true;
        fallingBlocks = GameObject.Find("FallingBlockSpawner").GetComponent<FallingBlocks>();
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
                if (hit.transform.gameObject.GetComponent(typeof(Door)))
      
                itemsHeld.Add(hitItem.itemID);
                gameObject.AddComponent(hitItem.GetType());
                audioSource.clip = positiveSound;
                audioSource.Play();
                (GetComponent(typeof(Item)) as Item).setItemProperties(hitItem.itemID, hitItem.prefab, hitItem.menuSprite, hitItem.description);
                audioSource.Play();

                if (hitItem.triggersPath)
                {
                    hitDoor = hit.transform.gameObject.GetComponent(typeof(Door)) as Door;
                    if (hitDoor.isLocked && !itemsHeld.Contains("Key"))
                        return;
                    hitDoor.toggleDoor();
                }
                else if (hit.transform.gameObject.GetComponent(typeof(Item)))
                {
                    hitItem = hit.transform.gameObject.GetComponent(typeof(Item)) as Item;
                    itemsHeld.Add(hitItem.itemID);
                    gameObject.AddComponent(hitItem.GetType());
                    audioSource.clip = positiveSound;
                    audioSource.Play();
                    //CollectLevelOneItems();
                    //uiManager.collectedObjectText.enabled = true;
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

                }
                else
                {
                    audioSource.clip = negativeSound;
                    audioSource.Play();
                }
            }
            else
            {
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

            popUpManager.obstacleTime = false;
            popUpManager.popBiome();
            popUpManager.riseBlocks();
        }
        if (collider.gameObject.tag.Equals("levelEnd"))
        {
            collider.gameObject.tag = "Untagged";
            levelManager.nextLevel();
        }
        if (collider.gameObject.GetComponent(typeof(Door)))
        {
            if (collider.gameObject.GetComponent<Door>().isLocked && !itemsHeld.Contains("Key"))
                return;

            hitDoor = collider.gameObject.GetComponent(typeof(Door)) as Door;
        }
         if(collider.gameObject.tag.Equals("Falling"))
        {
            Debug.Log("dead");
            this.gameObject.SetActive(false);
            Invoke("PlayerRespawn", 2.0f);
            Destroy(collider.gameObject);
        }
    }

    void PlayerRespawn()
    {
        this.gameObject.SetActive(true);
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


    /*void CollectLevelOneItems()
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
    }*/
}
