using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

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

    public const int width = 4;
    public const int length = 2;
    public const int blockSize = 4;
    public int currentItemNum = 0;
    public bool deservesItem;

    public GameObject vaultPrefab;
    public float[] levelHeights = { 0 };
    public int currentLevel;

    private List<TerrainBlock> currentLevelTerrain;

    public float vaultSpawnProb = 0.01f;

    public Movement movement;


    void Awake()
    {
        instantiateDataStructures();
    }

    void Start()
    {
        currentLevel = 0;
        posX = player.position.x;
        posZ = player.position.z;
    }

    void Update()
    {
        posChangedInstantiate();
    }

    void instantiateDataStructures()
    {
        currentLevelTerrain = gameTerrain[0].blocks;
        currentLevelTerrain.Add(itemTerrain[currentLevel].blocks[currentItemNum]);
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
        
        StartCoroutine(newBiomePop());
        
    }

    IEnumerator newBiomePop()
    {
        while (true)
        {
            yield return null;
            if (player.position.y > levelHeights[currentLevel])
            {
                popStarterArea();
                StartCoroutine(puzzleReset());
                yield break;
            }
        }
    }

    IEnumerator puzzleReset()
    {
        Destroy(vaultPrefab);
        yield return new WaitForSeconds(6f);
        Destroy(vaultPrefab);
        vaultPrefab = null;
        vaultUp = false;
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

    void checkIfVaultSpawn()
    {

        if (vaultPrefab != null)
        {
            Vector3 pos = roundVector3(player.position + (player.forward.normalized * 4) + (player.right * 4) + new Vector3(0, -player.position.y - 10, 0)); // doesn't take into account size

            if (Physics.CheckBox(pos, new Vector3(12, 1, 12)) == false && vaultUp == false)
            {
                if (UnityEngine.Random.Range(0, 1f) < vaultSpawnProb)
                {
                    vaultPrefab = Instantiate(vaultPrefab, new Vector3(pos.x, -2f, pos.z) + roundVector3(player.forward * 30), Quaternion.identity);
                    tweener.AddTween(vaultPrefab.transform, vaultPrefab.transform.position, new Vector3(vaultPrefab.transform.position.x, levelHeights[currentLevel], vaultPrefab.transform.position.z), 3f);
                    vaultUp = true;
                }
            }
        }
    }
    
    void createDebugSphere(Vector3 pos, Vector3 scale)
    {
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
        obj.transform.localScale = scale;
        Destroy(obj, 2f);
    }

    void popBiome() // Make recusrive, if something doesn't fit, find somewhere else or return something smaller.
    {

        checkIfVaultSpawn();

        TerrainBlock toSpawn;

        for (int x = -width/2; x < width/2; x++)
        {
            for (int y = 0; y < length; y++)
            {
                toSpawn = getModel();
                Vector3 pos = roundVector3(player.position + (player.forward.normalized * y * blockSize) + (player.right * x * blockSize) + toSpawn.size);

                if (Physics.CheckBox(roundVector3(new Vector3(pos.x, levelHeights[currentLevel], pos.z) + toSpawn.size), toSpawn.size))
                {
                    if (toSpawn.containsItem)
                        deservesItem = true;
                    continue;
                }

                posArray.Add(Instantiate(toSpawn.prefab, new Vector3(pos.x, levelHeights[currentLevel] - 4, pos.z), Quaternion.identity));

                if (toSpawn.containsItem) 
                    nextItem(toSpawn);

            }
        }
    }
 

    public void popStarterArea()
    {

        foreach (var obj in posArray)
        {
            pastPlatforms.Add(obj);
        }

        posArray.Clear();

        posArray.Add(Instantiate(getModel().prefab, roundVector3(new Vector3(player.position.x + 4, -2f, player.position.z - 4)), Quaternion.identity));
        posArray.Add(Instantiate(getModel().prefab, roundVector3(new Vector3(player.position.x + 4, -2f, player.position.z)), Quaternion.identity));
        posArray.Add(Instantiate(getModel().prefab, roundVector3(new Vector3(player.position.x + 4, -2f, player.position.z + 4)), Quaternion.identity));
        posArray.Add(Instantiate(getModel().prefab, roundVector3(new Vector3(player.position.x, -2f, player.position.z - 4)), Quaternion.identity));
        posArray.Add(Instantiate(getModel().prefab, roundVector3(new Vector3(player.position.x, -2f, player.position.z)), Quaternion.identity));
        posArray.Add(Instantiate(getModel().prefab, roundVector3(new Vector3(player.position.x, -2f, player.position.z + 4)), Quaternion.identity));
        posArray.Add(Instantiate(getModel().prefab, roundVector3(new Vector3(player.position.x - 4, -2f, player.position.z - 4)), Quaternion.identity));
        posArray.Add(Instantiate(getModel().prefab, roundVector3(new Vector3(player.position.x - 4, -2f, player.position.z)), Quaternion.identity));
        posArray.Add(Instantiate(getModel().prefab, roundVector3(new Vector3(player.position.x - 4, -2f, player.position.z + 4)), Quaternion.identity));

        tweenerManager();
    }

    void nextItem(TerrainBlock item)
    {
        if (currentItemNum < itemTerrain[currentLevel].blocks.Count - 1)
        {
            currentItemNum++;
            currentLevelTerrain.Add(itemTerrain[currentLevel].blocks.ElementAt(currentItemNum));
        }
        deservesItem = false;
        currentLevelTerrain.Remove(item);
        normalizeProbabilities(ref currentLevelTerrain);
    }

    TerrainBlock getModel()
    {
        if (deservesItem) 
            return currentLevelTerrain.ElementAt(currentLevelTerrain.Count - 1);

        int index = 0;
        float result = UnityEngine.Random.Range(0, 1f);
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
