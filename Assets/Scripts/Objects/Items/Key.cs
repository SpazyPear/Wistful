using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Item
{
    public GameObject door;
    private bool doorLocked = true;

    private void Awake()
    {
        itemID = "Key";
        UseKey();
    }

    public void UseKey()
    {
        if (doorLocked)
        {
            door.transform.Rotate(0, 90, 0);
        }
        doorLocked = false;
    }

}
