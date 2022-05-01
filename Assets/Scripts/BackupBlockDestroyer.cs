using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BackupBlockDestroyer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        try
        {
            if (collision.transform.GetChild(0).gameObject.tag == "ground")
            {
                Debug.Log("used");
                Destroy(gameObject);
            }
        }
        catch (Exception e)
        {

        }
    }
}
