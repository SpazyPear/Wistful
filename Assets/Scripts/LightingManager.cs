using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{

    public Light light;
    public Movement movement;
    

    // Start is called before the first frame update
    void Start()
    {
        movement.nextBiomeEvent += setLighting;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void setLighting(object sender, EventArgs e)
    {
        light.intensity -= 0.5f;
        light.colorTemperature -= 5f;
    }

}
