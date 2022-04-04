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
    private List<GameObject> posArray;
    private Vector2 currentPos;
    private float posX;
    private float posZ;
    public float posY;
    public Tweener tweener;
    private List<GameObject> pastPlatforms;
    private List<GameObject> toBeRemoved;
    public Dictionary<GameObject, int> ForestArray;
    public Dictionary<GameObject, int> WinterArray;
    public Dictionary<GameObject, int> HellArray;
    private Vector3 forward;
    private Vector3 left;
    private Vector3 right;
    private bool puzzleUp = false;
    
    private GameObject currentPuzzle;
    public enum biome { Hell = -20, Grass = 0, Ice = 100};
    public biome currentBiome;
    private Del popMethodGroup;
    [SerializeField]
    private List<GameObject> forestKeysList;
    [SerializeField]
    private List<int> forestValuesList;
    [SerializeField]
    private List<GameObject> winterKeysList;
    [SerializeField]
    private List<int> winterValuesList;
    [SerializeField]
    private List<GameObject> hellKeysList;
    [SerializeField]
    private List<int> hellValuesList;

    private Dictionary<GameObject, int> biomeArray;

    public Movement movement;
    

    delegate void Del();


    void Awake()
    {
        instantiateDataStructures();
    }

    void Start()
    {
        currentBiome = biome.Grass;
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
        posArray = new List<GameObject>();
        pastPlatforms = new List<GameObject>();
        toBeRemoved = new List<GameObject>();
        ForestArray = new Dictionary<GameObject, int>();
        WinterArray = new Dictionary<GameObject, int>();
        HellArray = new Dictionary<GameObject, int>();
       
        for (int i = 0; i < forestKeysList.Count; i++)
        {
            ForestArray.Add(forestKeysList[i], forestValuesList[i]);
        }
        for (int i = 0; i < winterKeysList.Count; i++)
        {
            WinterArray.Add(winterKeysList[i], winterValuesList[i]);
        }
        for (int i = 0; i < hellKeysList.Count; i++)
        {
            HellArray.Add(hellKeysList[i], hellValuesList[i]);
        }

        biomeArray = ForestArray;
    }

    void nextBiome(object sender, EventArgs e)
    {
        currentBiome = Enum.GetValues(typeof(biome)).Cast<biome>()
            .Skip(1).First();
        switch (currentBiome)
        {
            case biome.Hell:
                biomeArray = HellArray;
                break;

            case biome.Grass:
                biomeArray = ForestArray;
                break;

            case biome.Ice:
                biomeArray = WinterArray;
                break;
        }
        
        StartCoroutine(newBiomePop());
        
    }

    IEnumerator newBiomePop()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if (player.position.y > (float)currentBiome)
            {
                popStarterArea();
                StartCoroutine(puzzleReset());
                yield break;
            }
        }
    }

    IEnumerator puzzleReset()
    {
        Destroy(currentPuzzle);
        yield return new WaitForSeconds(6f);
        Destroy(currentPuzzle);
        currentPuzzle = null;
        puzzleUp = false;
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

            popBiome();

            tweenerManager();

        }
    }

    void tweenerManager()
    {
        foreach (GameObject obj in posArray)
        {
            tweener.AddTween(obj.transform, obj.transform.position, roundVector3(new Vector3(obj.transform.position.x, Mathf.Clamp(player.position.y - 3, (float)currentBiome, (float)currentBiome + 16), obj.transform.position.z)), 1.5f);
        }

        for (int count = pastPlatforms.Count - 1; count > 0; count--)
        {
            if (Vector3.Distance(player.transform.position, pastPlatforms.ElementAt(count).transform.position) > 35)
            {

                GameObject elem = pastPlatforms.ElementAt(count);
                tweener.AddTween(elem.transform, elem.transform.position, new Vector3(elem.transform.position.x, -20f, elem.transform.position.z), 6f);
                pastPlatforms.RemoveAt(count);
                
            }
           
            
        }

        if (currentPuzzle != null && puzzleUp == true)
        {
            if (Vector3.Distance(player.transform.position, GameObject.FindGameObjectWithTag("puzzle").transform.position) > 60)
            {
                tweener.AddTween(currentPuzzle.transform, currentPuzzle.transform.position, new Vector3(currentPuzzle.transform.position.x, -20f + (float)currentBiome, currentPuzzle.transform.position.z), 3f);
                puzzleUp = false;
            }
        }

     

    }

    void checkPuzzle()
    {

        if (currentPuzzle == null)
        {
            if (Physics.CheckBox(new Vector3(forward.x, (float)currentBiome / 5, forward.z) * 5, new Vector3(12, 1, 12)) == false && puzzleUp == false)
            {
                if (UnityEngine.Random.Range(1, 300) > biomeArray.ElementAt(biomeArray.Count - 1).Value)
                {
                    currentPuzzle = Instantiate(biomeArray.ElementAt(biomeArray.Count - 1).Key, new Vector3(forward.x, -2f, forward.z) + roundVector3(player.forward * 30), Quaternion.identity);
                    tweener.AddTween(currentPuzzle.transform, currentPuzzle.transform.position, new Vector3(currentPuzzle.transform.position.x, (float)currentBiome, currentPuzzle.transform.position.z), 3f);
                    puzzleUp = true;
                }
            }
        }
        else if (currentPuzzle != null && puzzleUp == false)
        {
            if (Vector3.Distance(player.transform.position, new Vector3(currentPuzzle.transform.position.x, (float)currentBiome, currentPuzzle.transform.position.z)) < 35)
            {
                tweener.AddTween(currentPuzzle.transform, currentPuzzle.transform.position, new Vector3(currentPuzzle.transform.position.x, (float)currentBiome, currentPuzzle.transform.position.z), 3f);
                puzzleUp = true;
            }
        }
    }

    void popBiome()
    {

        checkPuzzle();

        if (Physics.CheckBox(new Vector3(forward.x, Mathf.Clamp(player.position.y - 3, (float)currentBiome, (float)currentBiome + 16), forward.z), new Vector3(1, 1, 1)) == false)
        {
            posArray.Add(Instantiate(getModel(biomeArray), new Vector3(forward.x, -2f, forward.z), Quaternion.identity));
        }

        if (Physics.CheckBox(new Vector3(left.x, Mathf.Clamp(player.position.y - 3, (float)currentBiome, (float)currentBiome + 16), left.z), new Vector3(1, 1, 1)) == false)
        {
            posArray.Add(Instantiate(getModel(biomeArray), new Vector3(left.x, -2f, left.z), Quaternion.identity));
        }

        if (Physics.CheckBox(new Vector3(right.x, Mathf.Clamp(player.position.y - 3, (float)currentBiome, (float)currentBiome + 16), right.z), new Vector3(1, 1, 1)) == false)
        {
            posArray.Add(Instantiate(getModel(biomeArray), new Vector3(right.x, -2f, right.z), Quaternion.identity));
        }
        
    }
 

    public void popStarterArea()
    {

        foreach (var obj in posArray)
        {
            pastPlatforms.Add(obj);
        }

        posArray.Clear();

        posArray.Add(Instantiate(getModel(biomeArray), roundVector3(new Vector3(player.position.x + 4, -2f, player.position.z - 4)), Quaternion.identity));
        posArray.Add(Instantiate(getModel(biomeArray), roundVector3(new Vector3(player.position.x + 4, -2f, player.position.z)), Quaternion.identity));
        posArray.Add(Instantiate(getModel(biomeArray), roundVector3(new Vector3(player.position.x + 4, -2f, player.position.z + 4)), Quaternion.identity));
        posArray.Add(Instantiate(getModel(biomeArray), roundVector3(new Vector3(player.position.x, -2f, player.position.z - 4)), Quaternion.identity));
        posArray.Add(Instantiate(getModel(biomeArray), roundVector3(new Vector3(player.position.x, -2f, player.position.z)), Quaternion.identity));
        posArray.Add(Instantiate(getModel(biomeArray), roundVector3(new Vector3(player.position.x, -2f, player.position.z + 4)), Quaternion.identity));
        posArray.Add(Instantiate(getModel(biomeArray), roundVector3(new Vector3(player.position.x - 4, -2f, player.position.z - 4)), Quaternion.identity));
        posArray.Add(Instantiate(getModel(biomeArray), roundVector3(new Vector3(player.position.x - 4, -2f, player.position.z)), Quaternion.identity));
        posArray.Add(Instantiate(getModel(biomeArray), roundVector3(new Vector3(player.position.x - 4, -2f, player.position.z + 4)), Quaternion.identity));

        tweenerManager();
    }

    GameObject getModel(Dictionary<GameObject, int> biomeArray)
    {
        while (true)
        {
            GameObject potentialPop = biomeArray.ElementAt(UnityEngine.Random.Range(0, biomeArray.Count)).Key;
            if (biomeArray[potentialPop] < UnityEngine.Random.Range(1, 10))
            {
                return potentialPop;
            }
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
