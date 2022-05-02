using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaultDoor : Door
{

    LevelManager levelManager;
                


    public void Start()
    {
        levelManager = GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelManager>();
    }
    override public void toggleDoor()
    {
        levelManager.invokeVaultOpened();
    }


    
}
