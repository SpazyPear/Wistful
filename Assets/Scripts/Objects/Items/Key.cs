using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Item
{

    private void Awake()
    {
        itemID = "Key";
    }

    override public void setItemProperties(string itemID, GameObject prefab = null, Sprite menuSprite = null, string description = "")
    {
        base.setItemProperties(itemID, prefab, menuSprite, description);
        GameObject stickyNoteObj = GameObject.Find("StickyNote");
        if (stickyNoteObj)
            (stickyNoteObj.GetComponent(typeof(Item)) as Item).triggersNextItem = true;

    }
}

