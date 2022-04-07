using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    [HideInInspector]
    public List<string> itemsHeld = new List<string>();

    public UIManager uiManager;
    public InventoryManager inventoryManager;

    private void Update()
    {
    }

    private void OnTriggerStay(Collider collider)
    {
        /*if (collider.CompareTag("RocketToy") && Input.GetKeyDown(KeyCode.E))
        {
            itemsHeld.Add("RocketToy");
            uiManager.toggleRocketBar(true);

            Destroy(collider.gameObject);
        }

        if (collider.CompareTag("Ladder") && Input.GetKeyDown(KeyCode.E))
        {
            itemsHeld.Add("Ladder");
            Destroy(collider.gameObject);
        }

        if (collider.CompareTag("LadderPlacement") && Input.GetKeyDown(KeyCode.E) && itemsHeld.Contains("Ladder"))
        {
            collider.gameObject.transform.parent.gameObject.GetComponent<TreehouseManager>().placeLadder();
            itemsHeld.Remove("Ladder");
        }*/
        if (Input.GetKeyDown(KeyCode.E) && collider.gameObject.GetComponents(typeof(Item)).Length > 0)
        {
            inventoryManager.pickUpItem((collider.gameObject.GetComponent(typeof(Item))).GetType());
            Destroy(collider.gameObject);
        }
    }

    float easeInOutElastic(float x) {
            const float c5 = (2f * Mathf.PI) / 4.5f;

            return x == 0
            ? 0
            : x == 1
            ? 1
            : x< 0.5
            ? -(Mathf.Pow(2, 20 * x - 10) * Mathf.Sin((20f * x - 11.125f) * c5)) / 2
            : (Mathf.Pow(2, -20 * x + 10) * Mathf.Sin((20f * x - 11.125f) * c5)) / 2 + 1;
    }

}
