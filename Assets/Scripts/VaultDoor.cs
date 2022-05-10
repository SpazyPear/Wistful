using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaultDoor : Door
{

    [SerializeField]
    private Animator vaultDoor;
    [SerializeField]
    private string doorOpen = "VaultDoor";

    LevelManager levelManager;
                


    public void Start()
    {
        levelManager = GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelManager>();
    }
    override public void toggleDoor()
    {
        levelManager.invokeVaultOpened();
        vaultDoor.Play(doorOpen, 0, 0.0f);

    }


    
}
