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
    private List<GameObject> toRise = new List<GameObject>();
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

    [SerializeField]
    private List<LevelTerrain> crumbledTiles = new List<LevelTerrain>();

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

    public const float obstacleInterval = 20f;
    bool obstacleTimerRunning = false;
    bool obstacleTime;
    bool obstacleWasActive;

    List<GameObject> currentPath = new List<GameObject>();


    void Awake()
    {
        instantiateDataStructures(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    void Start()
    {
        SceneManager.sceneLoaded += instantiateDataStructures;
        StartCoroutine(obstacleTimer());
    }

    void Update()
    {
        posChangedInstantiate();
    }

    IEnumerator obstacleTimer()
    {
        if (!obstacleTimerRunning)
        {
            obstacleTimerRunning = true;
            float timer = obstacleInterval;
            while (true)
            {
                timer -= Time.deltaTime;
                if (timer < 0)
                    break;
                yield return null;
            }
            obstacleTime = true;
            obstacleTimerRunning = false;
            
        }
    }

    void instantiateDataStructures(Scene scene, LoadSceneMode mode)
    {
        posX = player.position.x;
        posZ = player.position.z;
        currentLevel = scene.buildIndex;
        currentLevelTerrain = gameTerrain[currentLevel].blocks;
        itemTerrain[0].blocks.Add(vaultBlock);
        normalizeProbabilities(ref currentLevelTerrain);
        normalizeProbabilities(ref crumbledTiles[currentLevel].blocks);
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
        normalizeProbabilities(ref crumbledTiles[currentLevel].blocks);
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


            foreach (GameObject obj in toRise)
            {
                pastPlatforms.Add(obj);
            }

            toRise.Clear();

            if (!obstacleTime)
            {
                popBiome();
                if (obstacleWasActive)
                {
                    obstacleWasActive = false;
                    StartCoroutine(obstacleTimer());
                }
            }
            else
            {
                if (!obstacleWasActive)
                {
                    currentPath = generatePath(4);
                }
                popObstacle();

            }

            tweenerManager();

        }
    }

    void tweenerManager()
    {
        foreach (GameObject obj in toRise)
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
                Vector3 pos = player.position + (player.forward.normalized * y * blockSize) + (player.right * x * blockSize);
                pos = roundVector3(new Vector3(pos.x, levelHeights[currentLevel], pos.z));
                bool edgeCase = y == length ? true : false;
                checkSpawnBlock(pos, edgeCase);

            }
        }
    }

    List<GameObject> generatePath(int pathSize)
    {
        List<GameObject> path = new List<GameObject>();
        Vector3 edge = roundVector3(player.position);
        Vector3 forward = nearestCardinal(player.transform.forward);
        int index = 0;
        while (true)
        {
            if (!Physics.CheckBox(edge + forward * index, new Vector3(4, 4, 4))) {
                break;
            }
            edge = roundVector3(edge + forward * index);
            index++;
        }

        for (int x = 0; x < pathSize; x++)
        {
            float result = UnityEngine.Random.Range(0, 1f);

            int resultIndex = 0;
            
            while (true)
            {
                result -= crumbledTiles[currentLevel].blocks.ElementAt(resultIndex).probability;
                if (result < 0)
                {
                    TerrainBlock block = crumbledTiles[currentLevel].blocks.ElementAt(resultIndex);
                    edge += new Vector3(forward.x * block.size.x, 0, forward.z * block.size.z) * blockSize;
                    Vector3 pos = roundVector3(edge);
                    pos.y = levelHeights[currentLevel] - 4;
                    GameObject obj = Instantiate(block.prefab, pos, Quaternion.identity);
                    for (int i = 0; i < obj.transform.childCount; i++)
                    {
                        path.Add(obj.transform.GetChild(i).GetChild(0).gameObject);
                        obj.transform.GetChild(i).transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                    }
                    break;
                }
                resultIndex++;
            }
        }
        return path;
    }

    Vector3 nearestCardinal(Vector3 input)
    {
        Vector3 output = Vector3.zero;
        int index = 0;
        float largest = 0;
        for (int x = 0; x < 3; x++)
        {
            if (largest < Mathf.Abs(input[x]))
            {
                largest = input[x];
                index = x;
            }
        }
        output[index] = 1;
        return output;
    }

    void popObstacle()
    {
        obstacleWasActive = true;
        for (int x = currentPath.Count - 1; x >= 0; x--)
        {
            if (Vector3.Distance(player.transform.position, currentPath[x].transform.position) < 20f)
            {
                currentPath[x].GetComponent<MeshRenderer>().enabled = true;
                tweener.AddTween(currentPath[x].transform, currentPath[x].transform.position, currentPath[x].transform.position + new Vector3(0, 4, 0), 1);
                currentPath.RemoveAt(x);
            }
        }
        if (currentPath.Count == 0)
        {
            obstacleTime = false;
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

        toRise.Add(Instantiate(toSpawn.prefab, new Vector3(pos.x, levelHeights[currentLevel] - 4, pos.z), Quaternion.identity));

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
            if (result < 0)
            {
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
