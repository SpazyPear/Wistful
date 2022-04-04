using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class customGrid : MonoBehaviour
{

    public GameObject structure;
    //public float gridSize;



    // Start is called before the first frame update
    void LateUpdate()
    {

        for (int i = 0; i < structure.transform.childCount; i++)
        {
            structure.transform.GetChild(i).gameObject.transform.position = roundVector3(structure.transform.GetChild(i).gameObject.transform.position);

            
        } 
    }

    Vector3 roundVector3(Vector3 pos)
    {
        return new Vector3(nearestMultiple(Convert.ToInt32(Mathf.Round(pos.x))), nearestMultiple(Convert.ToInt32(Mathf.Round(pos.y))), nearestMultiple(Convert.ToInt32(Mathf.Round(pos.z))));
    }

    int nearestMultiple(int num)
    {

        num = num % 2 == 0 ? num : num + 1;

        if (num < 0)
        {
            int remainderNeg = num % 4;
            return num + remainderNeg;
        }


        int remainder = num % 4;


        return num - remainder;
    }
}
