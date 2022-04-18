using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopUpManager : MonoBehaviour
{
    public Transform player;
    public GameObject prefab;
    private List<GameObject> posArray = new List<GameObject>();
    private float posX;
    private float posZ;
    public float posY;
    public Tweener tweener;
    private List<GameObject> pastPlatforms = new List<GameObject>();
    private List<GameObject> toBeRemoved = new List<GameObject>();
   
    [SerializeField]
    private List<LevelTerrain> gameTerrain = new List<LevelTerrain>();

    [SerializeField]
    private List<LevelTerrain> itemTerrain = new List<LevelTerrain>();

    private bool vaultUp = false;

    public const int width = 3;
    public const int length = 3;
    public const int blockSize = 4;
    public int currentItemNum = 0;
    public bool deservesItem;

    public float[] levelHeights = { 0 };
    public int currentLevel;

    private List<TerrainBlock> currentLevelTerrain;

    public TerrainBlock vaultBlock;

    public Movement movement;


    void Awake()
    {
        instantiateDataStructures(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    void Start()
    {
        SceneManager.sceneLoaded += instantiateDataStructures;
    }

    void Update()
    {
        posChangedInstantiate();
    }

    void instantiateDataStructures(Scene scene, LoadSceneMode mode)
    {
        posX = player.position.x;
        posZ = player.position.z;
        currentLevel = scene.buildIndex;
        currentLevelTerrain = gameTerrain[currentLevel].blocks;
        itemTerrain[0].blocks.Add(vaultBlock);
        normalizeProbabilities(ref currentLevelTerrain);
    }

    void normalizeProbabilities(ref List<TerrainBlock> items)
    {
        float sum = items.Sum(x => x.probability);
        for (int index = 0; index < items.Count; index++)
        {
            items.ElementAt(index).probability /= sum;
        }
        items.OrderByDescending(x => x.probability);
    }

    void nextBiome(object sender, EventArgs e)
    {
        currentLevel++;

        currentLevelTerrain = gameTerrain[currentLevel].blocks;
        normalizeProbabilities(ref currentLevelTerrain);
    }

    void posChangedInstantiate()
    {
        double relPosX = Math.Round(player.position.x);
        double relPosZ = Math.Round(player.position.z);
        double relPosY = Math.Round(player.position.y);
        if (relPosX > posX + 2 || relPosZ > posZ + 2 || relPosX < posX - 2 || relPosZ < posZ - 2)
        {
            
            posX = player.position.x;
            posZ = player.position.z;


            foreach (GameObject obj in posArray)
            {
                pastPlatforms.Add(obj);
            }

            posArray.Clear();

            popBiome();

            tweenerManager();

        }
    }

    void tweenerManager()
    {
        foreach (GameObject obj in posArray)
        {
            tweener.AddTween(obj.transform, obj.transform.position, roundVector3(new Vector3(obj.transform.position.x, levelHeights[currentLevel], obj.transform.position.z)), 1.5f);
        }
    }
    
    void createDebugSphere(Vector3 pos, Vector3 scale)
    {
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
        obj.transform.localScale = scale;
        //Destroy(obj, 2f);
    }

    void popBiome() // Make recusrive, if something doesn't fit, find somewhere else or return something smaller.
    {

        for (int x = -width; x <= width; x++)
        {
            for (int y = 0; y <= length; y++)
            {
                float result = UnityEngine.Random.Range(0, 1f);
                
                Vector3 pos = player.position + (player.forward.normalized * y * blockSize) + (player.right * x * blockSize);
                pos = roundVector3(new Vector3(pos.x, levelHeights[currentLevel], pos.z));
                bool edgeCase = y == length ? true : false;
                checkSpawnBlock(pos, edgeCase);

            }
        }
    }

    void checkSpawnBlock(Vector3 pos, bool canBeItem)
    {
        TerrainBlock toSpawn = getModel(canBeItem);

        Vector3 searchPos = pos;

        if (toSpawn.containsItem)
        {
            pos = roundVector3(pos + new Vector3((toSpawn.size.x * blockSize), 0, toSpawn.size.z * blockSize));
        }

        if (Physics.CheckBox(pos, toSpawn.size, Quaternion.identity, 1, QueryTriggerInteraction.Collide))
        {
            if (toSpawn.containsItem)
            {
                deservesItem = true;
                checkSpawnBlock(searchPos, false);
            }
            return;
        }

        posArray.Add(Instantiate(toSpawn.prefab, new Vector3(pos.x, levelHeights[currentLevel] - 4, pos.z), Quaternion.identity));

        if (toSpawn.containsItem)
        {
            currentItemNum++;
        }
    }


    TerrainBlock getModel(bool canBeItem)
    {

        float result = UnityEngine.Random.Range(0, 1f);

        if (canBeItem && currentItemNum < itemTerrain[currentLevel].blocks.Count)
            if (deservesItem || result < itemTerrain[currentLevel].blocks[currentItemNum].probability)
            {
                deservesItem = false;
                return itemTerrain[currentLevel].blocks[currentItemNum];
            }

        int index = 0;
        while (true)
        {
            result -= currentLevelTerrain.ElementAt(index).probability;
            if (result < 0) {
                TerrainBlock block = currentLevelTerrain.ElementAt(index);

                return block;
            }
            index++;
        }
    }

    Vector3 roundVector3(Vector3 pos)
    {
        return new Vector3(nearestMultiple(Convert.ToInt32(Mathf.Round(pos.x))), nearestMultiple(Convert.ToInt32(Mathf.Round(pos.y))), nearestMultiple(Convert.ToInt32(Mathf.Round(pos.z))));
    }

    int nearestMultiple(int num)
    {
        return Mathf.RoundToInt(num / blockSize) * blockSize;
    }

}
