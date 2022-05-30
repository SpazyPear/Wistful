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
    public GameObject debugCube;
    private List<GameObject> toRise = new List<GameObject>();
    private float posX;
    private float posZ;
    private float posY;
    public Tweener tweener;
    public Pointer pointer;

    [HideInInspector]
    public List<GameObject> pastPlatforms = new List<GameObject>();

    [SerializeField]
    private List<TerrainBlock> currentLevelTerrain = new List<TerrainBlock>();

    [SerializeField]
    private List<TerrainBlock> itemTerrain = new List<TerrainBlock>();

    [SerializeField]
    private List<PathTile> crumbledTiles = new List<PathTile>();

    
    public GameObject platformLink;

    [SerializeField]
    private GameObject startingPlatforms;

    private bool vaultUp = false;

    public const int width = 3;
    public const int length = 3;
    public const int blockSize = 4;
    public int currentItemNum = 0;
    public bool deservesItem;

    public float levelHeight = 0;

    public bool obstacleTime;
    bool obstacleWasActive;

    public bool readyForNextItemSpawn = true;


    List<List<GameObject>> currentPaths = new List<List<GameObject>>();
    List<List<GameObject>> fullCurrentPaths = new List<List<GameObject>>();

    public AudioSource rumbleSource;

    HeartRateManager heartRateManager;
    public GameObject fallingBlockPrefab;
    public bool dropFallingBlocks;

    void Awake()
    {
        instantiateDataStructures();
    }

    void Start()
    {
        pointer = GameObject.FindGameObjectWithTag("Pointer").GetComponent<Pointer>();
        heartRateManager = player.GetComponent<HeartRateManager>();
        readyForNextItemSpawn = true;
        for (int x = 0; x < startingPlatforms.transform.childCount; x++)
            pastPlatforms.Add(startingPlatforms.transform.GetChild(x).gameObject);
        
    }

    void Update()
    {
        posChangedInstantiate();
    }

    void instantiateDataStructures()
    {
        posX = player.position.x;
        posZ = player.position.z;
        normalizeProbabilities(ref currentLevelTerrain);
        normalizeProbabilities(ref crumbledTiles);
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

    void posChangedInstantiate()
    {
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

    public async void riseBlocks()
    {
        await tweener.waitForComplete();
        foreach (GameObject obj in toRise)
        {
            tweener.AddTween(obj.transform, obj.transform.position, VectorUtil.roundVector3(new Vector3(obj.transform.position.x, levelHeight, obj.transform.position.z)), 1.5f);
        }
    }

    public void dropBlocks(object sender = null, EventArgs e = null)
    {
        foreach (GameObject obj in pastPlatforms)
        {
            if (Vector3.Distance(new Vector3(player.position.x, levelHeight, player.position.z), obj.transform.position) > 10f)
                tweener.AddTween(obj.transform, obj.transform.position, VectorUtil.roundVector3(new Vector3(obj.transform.position.x, obj.transform.position.y - 12, obj.transform.position.z)), 3f);
        }
    }

    void createDebugSphere(Vector3 pos, Vector3 scale)
    {
        GameObject obj = Instantiate(debugCube, pos, Quaternion.identity);
        obj.transform.localScale = scale;
        //Destroy(obj, 2f);
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
        while (cardinalsIndex < cardinals.Length)
        {
            edges[cardinalsIndex] = VectorUtil.roundVector3(new Vector3(player.position.x, levelHeight, player.position.z));
            Vector3 forward = cardinals[cardinalsIndex];
            int index = 0;
            while (true)
            {
                if (!Physics.CheckBox(edges[cardinalsIndex] + (forward * blockSize), new Vector3(4, 4, 4), Quaternion.identity, 1, QueryTriggerInteraction.Collide))
                    break;

                edges[cardinalsIndex] = VectorUtil.roundVector3(edges[cardinalsIndex] + (forward * blockSize));
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
                Vector3 pos = VectorUtil.roundVector3(edges[cardinalsIndex]);
                pos.y = levelHeight - 3;
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

        PathTile getTile()
        {
            float result = UnityEngine.Random.Range(0, 1f);

            int resultIndex = 0;

            while (true)
            {
                result -= crumbledTiles.ElementAt(resultIndex).probability;
                if (result < 0)
                {
                    return crumbledTiles.ElementAt(resultIndex);
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
                    if (edges[cardinalsIndex][0] == null || (signedCardinalMagnitude(Vector3.Scale(obj.transform.position, cardinals[cardinalsIndex])) > signedCardinalMagnitude(Vector3.Scale(edges[cardinalsIndex][0].transform.position, cardinals[cardinalsIndex]))))
                    {
                        edges[cardinalsIndex].Clear();
                        edges[cardinalsIndex].Add(obj);
                        continue;
                    }
                    else if (signedCardinalMagnitude(Vector3.Scale(obj.transform.position, cardinals[cardinalsIndex])) == signedCardinalMagnitude(Vector3.Scale(edges[cardinalsIndex][0].transform.position, cardinals[cardinalsIndex])))
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

    int signedCardinalMagnitude(Vector3 input)
    {
       
            float extremity = 0;
            for (int x = 0; x < 3; x++)
            {
                if (Mathf.Abs(extremity) < Mathf.Abs(input[x]))
                {
                    extremity = input[x];
                }
            }
            return (int)extremity;
        
    }

    List<List<GameObject>> copyPaths(List<List<GameObject>> paths)
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

    void popObstacle()
    {
        obstacleWasActive = true;
        for (int x = currentPaths.Count - 1; x >= 0; x--)
        {
            for (int y = currentPaths[x].Count - 1; y >= 0; y--)
            {
                if (Vector3.Distance(player.transform.position, currentPaths[x][y].transform.position) < 35f)
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
            for (int y = fullCurrentPaths[x].Count - 1; y >= 0; y--)
            {
                try
                {
                    tweener.AddTween(fullCurrentPaths[x][y].transform, fullCurrentPaths[x][y].transform.position, new Vector3(fullCurrentPaths[x][y].transform.position.x, -12, fullCurrentPaths[x][y].transform.position.z), 2f);
                    rumbleSource.Play();
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
            pos = VectorUtil.roundVector3(pos + new Vector3((toSpawn.size.x * blockSize), 0, toSpawn.size.z * blockSize));
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

        GameObject obj = Instantiate(toSpawn.prefab, new Vector3(pos.x, levelHeight - 4, pos.z), Quaternion.identity);
        toRise.Add(obj);

        if (toSpawn.containsItem)
        {
            currentItemNum++;
            readyForNextItemSpawn = false;
            pointer.addItemInstance(obj);
        }
    }

    public void popBiome()
    {

        for (int x = -width; x <= width; x++)
        {
            for (int y = 0; y <= length; y++)
            {
                Vector3 pos = player.position + (player.forward.normalized * y * blockSize) + (player.right * x * blockSize);
                pos = VectorUtil.roundVector3(new Vector3(pos.x, levelHeight, pos.z));
                bool edgeCase = y == length ? true : false;
                checkSpawnBlock(pos, edgeCase && readyForNextItemSpawn);
            }
        }
    }

    TerrainBlock getModel(bool canBeItem)
    {

        float result = UnityEngine.Random.Range(0, 1f);

        if (canBeItem && currentItemNum < itemTerrain.Count)
            if (deservesItem || result < itemTerrain[currentItemNum].probability)
            {
                deservesItem = false;
                return itemTerrain[currentItemNum];
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

    public async void spawnLevelLink(object sender, EventArgs e)
    {
        obstacleTime = true;
        currentPaths.Clear();
        currentPaths.Add(new List<GameObject>());

        pointer.gameObject.SetActive(false);

        Vector3 edge = VectorUtil.roundVector3(player.position);
        edge = new Vector3(edge.x, levelHeight, edge.z);

        Vector3 forward = -Vector3.forward;

        await tweener.waitForComplete();

        while (true)
        {
            if (!Physics.CheckBox(edge + (forward * blockSize), new Vector3(4, 4, 4), Quaternion.identity, 1, QueryTriggerInteraction.Collide))
                break;

            edge = VectorUtil.roundVector3(edge + (forward * blockSize));
        }

        edge = new Vector3(edge.x, levelHeight - 3, edge.z);
        GameObject platformLinkInst = Instantiate(platformLink, edge, Quaternion.identity);
        var angle = Vector3.Angle(transform.forward, Vector3.Scale(transform.InverseTransformPoint(forward), new Vector3(1, 0, 1)));
        angle = Vector3.Dot(Vector3.right, transform.InverseTransformPoint(forward)) > 0.0f ? angle : -angle;
        platformLinkInst.transform.eulerAngles = new Vector3(0, angle, 0);

        for (int i = 0; i < platformLinkInst.transform.childCount; i++)
        {
            currentPaths[0].Add(platformLinkInst.transform.GetChild(i).gameObject);
            platformLinkInst.transform.GetChild(i).gameObject.SetActive(false);
        }

        fullCurrentPaths = copyPaths(currentPaths);

        obstacleTime = true;

        dropHazards(platformLinkInst);
    }

    async void dropHazards(GameObject platformLinkInst)
    {
        heartRateManager.endLevel = true;
        dropFallingBlocks = true;
        while (dropFallingBlocks && platformLinkInst)
        {
            Vector3 platformBounds = platformLinkInst.GetComponent<Collider>().bounds.size;
            float spawnPointX = UnityEngine.Random.Range(-platformBounds.x, platformBounds.x);
            float spawnPointZ = UnityEngine.Random.Range(-platformBounds.z, platformBounds.z);
            Vector3 pos = new Vector3(spawnPointX, platformBounds.y + 30, spawnPointZ) + platformLinkInst.transform.position;
            GameObject block = Instantiate(fallingBlockPrefab, pos, Quaternion.identity);
            await Task.Delay(Mathf.CeilToInt(Mathf.Clamp(800f - heartRateManager.heartRate * 40, 10, 1000)));
        }
    }
    



}
