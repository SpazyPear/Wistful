using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainBlock
{
    public GameObject prefab;
    public float probability;
    public Vector3 size;
    public bool containsItem;
    public bool canSpawn;

    public TerrainBlock(GameObject prefab, float probability, Vector3 size, bool containsItem = false)
    {
        this.prefab = prefab;
        this.probability = probability;
        this.size = size;
        this.containsItem = containsItem;
        canSpawn = true;
    }
}
