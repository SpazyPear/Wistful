using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartRateManager : MonoBehaviour
{
    public float heartRate = 0.01f;
    public float increaseDamper = 3f;
    public float maxHeartRate = 100;
    public bool endLevel;

    private void Update()
    {
        if (endLevel)
        {
            heartRate = Mathf.Clamp(heartRate + Time.deltaTime / increaseDamper, 0, maxHeartRate);
            Debug.Log(heartRate);
        }
    }

}
