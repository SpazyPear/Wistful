using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaultDoor : MonoBehaviour
{
    [SerializeField]
    private Animator vaultDoor;
    [SerializeField]
    private string doorOpen = "VaultDoor";



    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {

        vaultDoor.Play(doorOpen, 0, 0.0f);
        }
        
    }
  

}
