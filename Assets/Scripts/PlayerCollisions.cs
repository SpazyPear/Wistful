using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    Dictionary<string, GameObject> itemsHeld = new Dictionary<string, GameObject>(); 

    private void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag("RocketToy") && Input.GetKeyDown(KeyCode.E))
        {
            itemsHeld.Add("RocketToy", collider.gameObject);
            Destroy(collider.gameObject);
        }

        if (collider.CompareTag("Ladder") && Input.GetKeyDown(KeyCode.E))
        {
            itemsHeld.Add("Ladder", collider.gameObject);
            Destroy(collider.gameObject);
        }

        if (collider.CompareTag("LadderPlacement") && Input.GetKeyDown(KeyCode.E))
        {
            collider.gameObject.transform.parent.gameObject.GetComponent<TreehouseManager>().placeLadder();
        }
    }
}
