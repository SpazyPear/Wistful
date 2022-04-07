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
    private Vector2 currentPos;
    private float posX;
    private float posZ;
    public float posY;
    public Tweener tweener;
    private List<GameObject> pastPlatforms = new List<GameObject>();
    private List<GameObject> toBeRemoved = new List<GameObject>();
    [SerializeField]
    private List<LevelTerrain> gameTerrain = new List<LevelTerrain>();

    private Vector3 forward;
    private Vector3 left;
    private Vector3 right;
    private bool vaultUp = false;


    
    public GameObject vaultPrefab;
    public float[] levelHeights = { 0 };
    public int currentLevel;
    private Del popMethodGroup;

    private List<TerrainBlock> currentLevelTerrain;

    public float vaultSpawnProb = 0.01f;

    public Movement movement;

    delegate void Del();


    void Awake()
    {
        instantiateDataStructures();
    }

    void Start()
    {
        currentLevel = 0;
        movement.nextBiomeEvent += nextBiome;
        posX = player.position.x;
        posZ = player.position.z;
        Del builder = posChangedInstantiate;
        popMethodGroup = builder;

    }

    void Update()
    {
        popMethodGroup();
    }

    void instantiateDataStructures()
    {

        for (int x = 0; x < gameTerrain.Count; x++) {

            // Normalise probability values between 0 and 1
            normalizeProbabilities(ref gameTerrain[x].blocks);
            
        }

        currentLevelTerrain = gameTerrain[0].blocks;
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

            forward = roundVector3(player.forward * 5 + player.position);
            left = roundVector3(player.right * 2 + player.forward * 5 + player.position);
            right = roundVector3(-player.right * 2 + player.forward * 5 + player.position);
            /*Destroy(Instantiate(prefab, forward + Vector3.up, Quaternion.identity), 2f);
            Destroy(Instantiate(prefab, left + Vector3.up, Quaternion.identity), 2f);
            Destroy(Instantiate(prefab, right + Vector3.up, Quaternion.identity), 2f);*/


            popBiome();

            tweenerManager();

        }
    }

    void tweenerManager()
    {
        foreach (GameObject obj in posArray)
        {
            tweener.AddTween(obj.transform, obj.transform.position, roundVector3(new Vector3(obj.transform.position.x, Mathf.Clamp(player.position.y - 3, levelHeights[currentLevel], levelHeights[currentLevel] + 16), obj.transform.position.z)), 1.5f);
        }

        /*for (int count = pastPlatforms.Count - 1; count > 0; count--) //Removes blocks you're far from
        {
            if (Vector3.Distance(player.transform.position, pastPlatforms.ElementAt(count).transform.position) > 35)
            {

                GameObject elem = pastPlatforms.ElementAt(count);
                tweener.AddTween(elem.transform, elem.transform.position, new Vector3(elem.transform.position.x, -20f, elem.transform.position.z), 6f);
                pastPlatforms.RemoveAt(count);
                
            }
           
            
        }*/

        if (vaultPrefab != null && vaultUp == true)
        {
            if (Vector3.Distance(player.transform.position, GameObject.FindGameObjectWithTag("puzzle").transform.position) > 60)
            {
                tweener.AddTween(vaultPrefab.transform, vaultPrefab.transform.position, new Vector3(vaultPrefab.transform.position.x, -20f + levelHeights[currentLevel], vaultPrefab.transform.position.z), 3f);
                vaultUp = false;
            }
        }

     

    }

    void checkIfVaultSpawn()
    {

        if (vaultPrefab != null)
        {
            if (Physics.CheckBox(new Vector3(forward.x, levelHeights[currentLevel] / 5, forward.z) * 5, new Vector3(12, 1, 12)) == false && vaultUp == false)
            {
                if (UnityEngine.Random.Range(0, 1f) < vaultSpawnProb)
                {
                    vaultPrefab = Instantiate(vaultPrefab, new Vector3(forward.x, -2f, forward.z) + roundVector3(player.forward * 30), Quaternion.identity);
                    tweener.AddTween(vaultPrefab.transform, vaultPrefab.transform.position, new Vector3(vaultPrefab.transform.position.x, levelHeights[currentLevel], vaultPrefab.transform.position.z), 3f);
                    vaultUp = true;
                }
            }
        }
      /*  else if (vaultPrefab != null && vaultUp == false)
        {
            if (Vector3.Distance(player.transform.position, new Vector3(vaultPrefab.transform.position.x, levelHeights[currentLevel], vaultPrefab.transform.position.z)) < 35)
            {
                tweener.AddTween(vaultPrefab.transform, vaultPrefab.transform.position, new Vector3(vaultPrefab.transform.position.x, levelHeights[currentLevel], vaultPrefab.transform.position.z), 3f);
                vaultUp = true;
            }
        }*/
    }

    void popBiome() // Make recusrive, if something doesn't fit, find somewhere else or return something smaller.
    {

        checkIfVaultSpawn();

        TerrainBlock toSpawn = getModel();

        if (Physics.CheckBox(new Vector3(forward.x + toSpawn.size.x, Mathf.Clamp(player.position.y - 3, levelHeights[currentLevel], levelHeights[currentLevel] + 16), forward.z + toSpawn.size.z), toSpawn.size) == false)
        {
            posArray.Add(Instantiate(toSpawn.prefab, new Vector3(forward.x + toSpawn.size.x, -2f, forward.z + toSpawn.size.z - 1), Quaternion.identity));
            if (toSpawn.containsItem)
                toSpawn.canSpawn = false;
        }
        toSpawn = getModel();
        if (Physics.CheckBox(new Vector3(left.x + toSpawn.size.x, Mathf.Clamp(player.position.y - 3, levelHeights[currentLevel], levelHeights[currentLevel] + 16), left.z + toSpawn.size.z), toSpawn.size) == false)
        {
            posArray.Add(Instantiate(toSpawn.prefab, new Vector3(left.x + toSpawn.size.x, -2f, left.z + toSpawn.size.z - 1), Quaternion.identity));
            if (toSpawn.containsItem)
                toSpawn.canSpawn = false;
        }
        toSpawn = getModel();
        if (Physics.CheckBox(new Vector3(right.x + toSpawn.size.x, Mathf.Clamp(player.position.y - 3, levelHeights[currentLevel], levelHeights[currentLevel] + 16), right.z + toSpawn.size.z), toSpawn.size) == false)
        {
            posArray.Add(Instantiate(toSpawn.prefab, new Vector3(right.x + toSpawn.size.x, -2f, right.z + toSpawn.size.z), Quaternion.identity));
            if (toSpawn.containsItem)
                toSpawn.canSpawn = false;
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

    TerrainBlock getModel()
    {
        int index = 0;
        float result = UnityEngine.Random.Range(0, 1f);
        while (true)
        {
            result -= currentLevelTerrain.ElementAt(index).probability;
            if (result < 0)
                if (currentLevelTerrain.ElementAt(index).canSpawn != false)
                    return currentLevelTerrain.ElementAt(index);
                else
                {
                    return getModel();
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
