using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { Forward, Left, Right, Up, Down }

[System.Serializable]
public class PathTile
{
    public GameObject prefab;
    public float probability;
    public Vector3 size;
    public Direction tileDirection;

    public PathTile(GameObject prefab, Direction tileDirection, float probability, Vector3 size)
    {
        this.prefab = prefab;
        this.probability = probability;
        this.size = size;
        this.tileDirection = tileDirection;
    }


}
