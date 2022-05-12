using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyNote : Item
{

    private void Start()
    {
        GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>().updateHoldPrompt("Sticky Note Said W8pvXi0m...");

    }
    // Start is called before the first frame update
    override public void setItemProperties(string itemID, GameObject prefab = null, Sprite menuSprite = null, string description = "")
    {
        base.setItemProperties(itemID, prefab, menuSprite, description);
        GameObject keyObj = GameObject.Find("Key");
        if (keyObj)
            (keyObj.GetComponent(typeof(Item)) as Item).triggersNextItem = true;
    }

}
