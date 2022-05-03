using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crowbar : Item
{
    GameObject shatteredPrefab;

    GameObject hitPane;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && hitPane)
        {
            shatteredPrefab = hitPane.GetComponent<Glass>().shatteredPrefab;
            GameObject shatteredPane = Instantiate(shatteredPrefab, hitPane.transform.position, Quaternion.identity);
            shatteredPane.transform.localScale = new Vector3(2, 2, 2);
            Destroy(hitPane);

            hitPane = null;

            for (int x = 0; x < shatteredPane.transform.childCount; x++)
            {
                shatteredPane.transform.GetChild(x).gameObject.layer = 3;
                shatteredPane.transform.GetChild(x).gameObject.AddComponent<Rigidbody>();
                shatteredPane.transform.GetChild(x).gameObject.AddComponent<BoxCollider>();  
                Destroy(shatteredPane.transform.GetChild(x).gameObject, 2f);
            }

        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag.Equals("Glass"))
        {
            hitPane = collider.gameObject;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag.Equals("Glass"))
        {
            hitPane = null;
        }
    }
}
