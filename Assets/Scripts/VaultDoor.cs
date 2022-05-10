using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaultDoor : Door
{

    [SerializeField]
    private Animator vaultDoor;
    [SerializeField]
    private string doorAnim = "VaultDoor";

    LevelManager levelManager;
                


    public void Start()
    {
        levelManager = GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelManager>();
    }
    override public void toggleDoor()
    {
        levelManager = GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelManager>();

        levelManager.invokeVaultOpened();
        vaultDoor.Play(doorAnim, 0, 0.0f);
    }


    
}
