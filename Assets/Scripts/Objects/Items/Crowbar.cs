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
        
    }

    // Update is called once per frame
    void Update()
    {
        // When mouse clicks
        if (Input.GetMouseButtonDown(0))
        {
            Hit();
        }
        // Raycast in front
        // Check if glass
        // Break glass
    }

    void Hit()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, range))
        {
            Glass glass = hit.transform.GetComponent<Glass>();
            if (glass != null)
            {
                glass.Shatter();
            }
        }
    }
}
