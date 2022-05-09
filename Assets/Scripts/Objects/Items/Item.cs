using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemID;
    public GameObject prefab;
    public Sprite menuSprite;
    public string description;
    public bool triggersPath = true;
    public bool triggersNextItem = true;

    virtual public void setItemProperties(string itemID, GameObject prefab = null, Sprite menuSprite = null, string description = "")
    {
        this.itemID = itemID;
        this.prefab = prefab;
        this.menuSprite = menuSprite;
        this.description = description;
    }

}
