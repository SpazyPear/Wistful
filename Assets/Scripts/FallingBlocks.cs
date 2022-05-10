using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlocks : MonoBehaviour
{
    public float spawnRate;
    public GameObject grassForFalling;

    public bool blockSpawned;

    GameObject player;


    // Start is called before the first frame update
    void Start()
    {
        spawnRate = 5000.0f;
        blockSpawned = false;
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnBlock(Vector3 spawnCoordinates)
    {
        if (!blockSpawned)
        {
            Instantiate(grassForFalling, spawnCoordinates, Quaternion.identity);
            /*Instantiate(grassForFalling, new Vector3(spawnCoordinates.x + 8, spawnCoordinates.y, spawnCoordinates.z), Quaternion.identity);
            Instantiate(grassForFalling, new Vector3(spawnCoordinates.x - 4, spawnCoordinates.y, spawnCoordinates.z), Quaternion.identity);
            Instantiate(grassForFalling, new Vector3(spawnCoordinates.x - 8, spawnCoordinates.y, spawnCoordinates.z), Quaternion.identity);
            Instantiate(grassForFalling, new Vector3(spawnCoordinates.x + 4, spawnCoordinates.y, spawnCoordinates.z), Quaternion.identity);
            Instantiate(grassForFalling, new Vector3(spawnCoordinates.x, spawnCoordinates.y, spawnCoordinates.z - 4), Quaternion.identity);
            Instantiate(grassForFalling, new Vector3(spawnCoordinates.x, spawnCoordinates.y, spawnCoordinates.z + 4), Quaternion.identity);*/
            grassForFalling.GetComponent<Rigidbody>().AddForce(new Vector3(0, -5000.0f * spawnRate, 0), ForceMode.Impulse);
            grassForFalling.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            blockSpawned = true;
        }
    }
}
