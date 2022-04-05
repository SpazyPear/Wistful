using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreehouseManager : MonoBehaviour
{
    [SerializeField]
    GameObject ladder;
    public void placeLadder()
    {
        ladder.SetActive(true);
    }
}
