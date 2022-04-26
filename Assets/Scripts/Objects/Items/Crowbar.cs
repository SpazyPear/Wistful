using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crowbar : Item
{
    Camera camera;
    float range = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Hit();
        }
    }

    void Hit()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, camera.transform.TransformDirection(Vector3.forward), out hit, range))
        {
            Debug.Log(hit.transform.gameObject.name);
            if (hit.transform.gameObject.tag == "Glass")
            {
                hit.transform.gameObject.SetActive(false);
            }
        }
    }
}
