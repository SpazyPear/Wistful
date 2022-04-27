using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class PopUpManager : MonoBehaviour
{
    public Transform player;
    public GameObject prefab;
    private List<GameObject> toRise = new List<GameObject>();
    private float posX;
    private float posZ;
    public float posY;
    public Tweener tweener;

    [HideInInspector]
    public List<GameObject> pastPlatforms = new List<GameObject>();

    [SerializeField]
    private List<LevelTerrain> gameTerrain = new List<LevelTerrain>();

    [SerializeField]
    private List<LevelTerrain> itemTerrain = new List<LevelTerrain>();

    [SerializeField]
    private List<GamePathTiles> crumbledTiles = new List<GamePathTiles>();

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
    public bool obstacleTime;
    bool obstacleWasActive;

    public bool itemPickedUp = true;
   

    List<List<GameObject>> currentPaths = new List<List<GameObject>>();
    List<List<GameObject>> fullCurrentPaths = new List<List<GameObject>>();


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
        normalizeProbabilities(ref crumbledTiles[currentLevel].pathTiles);
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

    void normalizeProbabilities(ref List<PathTile> items)
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
        normalizeProbabilities(ref crumbledTiles[currentLevel].pathTiles);
    }

    void posChangedInstantiate()
    {
        Debug.Log(obstacleTime);
        double relPosX = Math.Round(player.position.x);
        double relPosZ = Math.Round(player.position.z);
        double relPosY = Math.Round(player.position.y);
        if ((relPosX > posX + 2 || relPosZ > posZ + 2 || relPosX < posX - 2 || relPosZ < posZ - 2) && tweener.activeTweens.Count == 0)
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
                if (obstacleWasActive)
                {
                    obstacleWasActive = false;
                    
                    return;
                }
                popBiome(); 
            }
            else
            {
                popObstacle();
            }

            riseBlocks();

        }
    }

    public void riseBlocks()
    {
        foreach (GameObject obj in toRise)
        {
            tweener.AddTween(obj.transform, obj.transform.position, roundVector3(new Vector3(obj.transform.position.x, levelHeights[currentLevel], obj.transform.position.z)), 1.5f);
        }
    }

    void dropBlocks()
    {
        foreach(GameObject obj in pastPlatforms)
        {
            if (Vector3.Distance(new Vector3(player.position.x, levelHeights[currentLevel], player.position.z), obj.transform.position) > 10f)
                tweener.AddTween(obj.transform, obj.transform.position, roundVector3(new Vector3(obj.transform.position.x, obj.transform.position.y - 12, obj.transform.position.z)), 3f);
        }
    }

    void createDebugSphere(Vector3 pos, Vector3 scale)
    {
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
        obj.transform.localScale = scale;
        Destroy(obj, 2f);
    }

    public void popBiome() // Make recusrive, if something doesn't fit, find somewhere else or return something smaller.
    {

        for (int x = -width; x <= width; x++)
        {
            for (int y = 0; y <= length; y++)
            {
                Vector3 pos = player.position + (player.forward.normalized * y * blockSize) + (player.right * x * blockSize);
                pos = roundVector3(new Vector3(pos.x, levelHeights[currentLevel], pos.z));
                bool edgeCase = y == length ? true : false;
                checkSpawnBlock(pos, edgeCase && itemPickedUp);
            }
        }
    }

    public async void generatePath(int pathSize)
    {

        dropBlocks();

        await tweener.waitForComplete();

        List<List<GameObject>> paths = new List<List<GameObject>>();
        Vector3[] cardinals = new Vector3[4];
        cardinals[0] = new Vector3(1, 0, 0);
        cardinals[1] = new Vector3(-1, 0, 0);
        cardinals[2] = new Vector3(0, 0, 1);
        cardinals[3] = new Vector3(0, 0, -1);

        Vector3[] edges = new Vector3[4];
        int cardinalsIndex = 0;

        // Find edges
        while (cardinalsIndex < cardinals.Length - 1)
        {
            Vector3 edge = roundVector3(player.position);
            edge = new Vector3(edge.x, levelHeights[currentLevel], edge.z);

            Vector3 forward = cardinals[cardinalsIndex];
            int index = 0;
            while (true)
            {
                if (!Physics.CheckBox(edge + (forward * blockSize) * index, new Vector3(4, 4, 4), Quaternion.identity, 1, QueryTriggerInteraction.Collide))
                    break;

                edges[cardinalsIndex] = roundVector3(edge + (forward * blockSize) * (index - 1));
                index++;
            }
            cardinalsIndex++;
        }

        cardinalsIndex = 0;
        currentPaths.Clear();

        // Layout the paths in all cardinal directions
        while (cardinalsIndex < cardinals.Length) 
        {
            paths.Add(new List<GameObject>());
            List<Direction> directions = new List<Direction>();
            for (int x = 0; x < pathSize; x++)
            {
                PathTile tile = getTile();
                while (true)
                {
                     if (tile.tileDirection != Direction.Forward && directions.Contains(tile.tileDirection))
                     {
                        tile = getTile();
                     }
                     else
                        break;
                }

                edges[cardinalsIndex] += new Vector3(cardinals[cardinalsIndex].x * tile.size.x, 0, cardinals[cardinalsIndex].z * tile.size.z) * blockSize;
                Vector3 pos = roundVector3(edges[cardinalsIndex]);
                pos.y = levelHeights[currentLevel] - 3;
                GameObject obj = Instantiate(tile.prefab, pos, Quaternion.identity);
                var angle = Vector3.Angle(transform.forward, Vector3.Scale(transform.InverseTransformPoint(cardinals[cardinalsIndex]), new Vector3(1, 0, 1)));
                angle = Vector3.Dot(Vector3.right, transform.InverseTransformPoint(cardinals[cardinalsIndex])) > 0.0f ? angle : -angle;
                obj.transform.eulerAngles = new Vector3(0, angle, 0);

                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    paths[cardinalsIndex].Add(obj.transform.GetChild(i).gameObject);
                    obj.transform.GetChild(i).gameObject.SetActive(false);
                }
                             
            }
            cardinalsIndex++;
        }
        markPathEdges(paths);
        currentPaths = paths;
        fullCurrentPaths = copyPaths(paths);

        List<List<GameObject>> copyPaths (List<List<GameObject>> paths)
        {
            List<List<GameObject>> copy = new List<List<GameObject>>();
            for (int x = 0; x < paths.Count; x++)
            {
                copy.Add(new List<GameObject>());
                for (int y = 0; y < paths[x].Count; y++)
                {
                    copy[x].Add(paths[x][y]);
                }
            }
            return copy;
        }

        PathTile getTile()
        {
            float result = UnityEngine.Random.Range(0, 1f);

            int resultIndex = 0;

            while (true)
            {
                result -= crumbledTiles[currentLevel].pathTiles.ElementAt(resultIndex).probability;
                if (result < 0)
                {
                    return crumbledTiles[currentLevel].pathTiles.ElementAt(resultIndex);
                }
                resultIndex++;
            }
        }

        void markPathEdges(List<List<GameObject>> paths)
        {
            Vector3[] cardinals = new Vector3[4];
            cardinals[0] = new Vector3(1, 0, 0);
            cardinals[1] = new Vector3(-1, 0, 0);
            cardinals[2] = new Vector3(0, 0, 1);
            cardinals[3] = new Vector3(0, 0, -1);

            int cardinalsIndex = 0;
            List<List<GameObject>> edges = new List<List<GameObject>>();
            while (cardinalsIndex < cardinals.Length)
            {
                edges.Add(new List<GameObject>());
                edges[cardinalsIndex].Add(null);
                foreach (GameObject obj in paths[cardinalsIndex])
                {
                    if (edges[cardinalsIndex][0] == null || Mathf.Abs(Vector3.Scale(obj.transform.position, cardinals[cardinalsIndex]).magnitude) > Mathf.Abs(Vector3.Scale(edges[cardinalsIndex][0].transform.position, cardinals[cardinalsIndex]).magnitude))
                    {
                        edges[cardinalsIndex].Clear();
                        edges[cardinalsIndex].Add(obj);
                        continue;
                    }
                    else if (Mathf.Abs(Vector3.Scale(obj.transform.position, cardinals[cardinalsIndex]).magnitude) == Mathf.Abs(Vector3.Scale(edges[cardinalsIndex][0].transform.position, cardinals[cardinalsIndex]).magnitude))
                    {
                        edges[cardinalsIndex].Add(obj);
                    }
                }
                cardinalsIndex++;
            }
            foreach (List<GameObject> list in edges)
            {
                foreach (GameObject obj in list)
                {
                    obj.transform.gameObject.AddComponent<BoxCollider>().isTrigger = true;
                    obj.transform.GetComponent<BoxCollider>().size = new Vector3(blockSize, blockSize, blockSize);
                    obj.transform.tag = "pathEdge";
                }
            }
        }
    }

    void popObstacle()
    {
        obstacleWasActive = true;
        for (int x = currentPaths.Count - 1; x >= 0; x--)
        {
            for (int y = currentPaths[x].Count - 1; y >= 0; y--)
            {
                if (Vector3.Distance(player.transform.position, currentPaths[x][y].transform.position) < 20f)
                {
                    currentPaths[x][y].gameObject.SetActive(true);
                    tweener.AddTween(currentPaths[x][y].transform, currentPaths[x][y].transform.position, new Vector3(currentPaths[x][y].transform.position.x, currentPaths[x][y].transform.position.y + 3, currentPaths[x][y].transform.position.z), 1);
                    currentPaths[x].RemoveAt(y);
                }
            }
        }
    }

    public async void destroyPath(CancellationToken token)
    {
        float timer = 0;
        while (timer < 2f)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
            timer += Time.deltaTime;
            await Task.Yield();
        }
        for (int x = fullCurrentPaths.Count - 1; x >= 0; x--)
        {
            for (int y = fullCurrentPaths[x].Count - 1; y >= 0 ; y--)
            {
                try
                {
                    tweener.AddTween(fullCurrentPaths[x][y].transform, fullCurrentPaths[x][y].transform.position, new Vector3(fullCurrentPaths[x][y].transform.position.x, -12, fullCurrentPaths[x][y].transform.position.z), 2f);
                }
                catch (MissingReferenceException e)
                {

                }
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

        toRise.Add(Instantiate(toSpawn.prefab, new Vector3(pos.x, levelHeights[currentLevel] - 4, pos.z), Quaternion.identity));

        if (toSpawn.containsItem)
        {
            currentItemNum++;
            itemPickedUp = false;
        }
    }


    TerrainBlock getModel(bool canBeItem)
    {

        float result = UnityEngine.Random.Range(0, 1f);

        if (canBeItem && currentItemNum < itemTerrain[currentLevel].blocks.Count)
            if (deservesItem || result > itemTerrain[currentLevel].blocks[currentItemNum].probability)
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
