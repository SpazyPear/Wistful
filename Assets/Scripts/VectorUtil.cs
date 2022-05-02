using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class VectorUtil
{
    public static Vector3 roundVector3(Vector3 pos)
    {
        return new Vector3(nearestMultiple(Convert.ToInt32(Mathf.Round(pos.x))), nearestMultiple(Convert.ToInt32(Mathf.Round(pos.y))), nearestMultiple(Convert.ToInt32(Mathf.Round(pos.z))));
    }

    static int nearestMultiple(int num)
    {
        return Mathf.RoundToInt(num / 4) * 4;
    }
}
