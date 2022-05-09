using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyNote : Item
{
    // Start is called before the first frame update
    override public void setItemProperties(string itemID, GameObject prefab = null, Sprite menuSprite = null, string description = "")
    {
        base.setItemProperties(itemID, prefab, menuSprite, description);
        GameObject keyObj = GameObject.FindGameObjectWithTag("Key");
        if (keyObj)
            (keyObj.GetComponent(typeof(Item)) as Item).triggersNextItem = true;
    }
}
