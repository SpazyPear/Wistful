using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : Item
{
    private void Awake()
    {
        itemID = "Ladder";
    }
    private void Update()
    {
        
    }
    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("LadderPlacement") && Input.GetKeyDown(KeyCode.E) && GetComponent<PlayerCollisions>().itemsHeld.Contains("Ladder"))
        {
            collider.gameObject.GetComponent<LadderPlacementTrigger>().ladder.SetActive(true);
        }
    }
}
