using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crowbar : Item
{
    Camera camera;
    float range = 2.0f;

    GameObject hitPane;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && hitPane)
        {
            hitPane.tag = "Untagged";
            
            for (int x = 0; x < hitPane.transform.childCount; x++)
            {
                hitPane.transform.GetChild(x).gameObject.layer = 3;
                hitPane.transform.GetChild(x).gameObject.AddComponent<Rigidbody>();
                hitPane.transform.GetChild(x).gameObject.AddComponent<BoxCollider>();  
                Destroy(hitPane.transform.GetChild(x).gameObject, 2f);
            }

            hitPane = null;
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
