using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyNote : Item
{
    public GameObject UiObject;
    Animator stickyNote;
    bool isFliped;
    private bool hasPlayer;

    private void Start()
    {
        stickyNote = GameObject.FindGameObjectWithTag("SickyNoteCanvas").GetComponent<Animator>();
        UiObject.SetActive(false);
    }

    void flipNote()
    {
        if (!isFliped)
            stickyNote.SetTrigger("FlipNote");
        else
            stickyNote.SetTrigger("RevertNote");

        isFliped = !isFliped;
    }

    // Start is called before the first frame update
    override public void setItemProperties(string itemID, GameObject prefab = null, Sprite menuSprite = null, string description = "")
    {
        base.setItemProperties(itemID, prefab, menuSprite, description);
        GameObject keyObj = GameObject.Find("Key");
        if (keyObj)
            (keyObj.GetComponent(typeof(Item)) as Item).triggersNextItem = true;

    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Mirror"))
        {
            flipNote();
        }

        if (collider.CompareTag("StickyNote2"))
        {
            hasPlayer = true;
        }

    }

    //private void OnTriggerStay(Collider collider)
    //{
    //    if (collider.CompareTag("StickyNote2") && (Input.GetKeyDown(KeyCode.E)))
    //    {
    //        UiObject.SetActive(true);
    //    }
    //}


    private void Update()
    {
        if (hasPlayer && Input.GetKeyDown("e"))
        {
            UiObject.SetActive(true);
        }
    }
}
