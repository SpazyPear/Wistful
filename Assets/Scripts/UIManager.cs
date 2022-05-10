using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public Slider rocketFuelBar;
    public GameObject rocketFuelContainer;
    public Image rocketFuelBackground;
    public Text findObject1Text;
    public Text findObject2Text;
    public Text findObject3Text;
    public Text findObject4Text;

    public Text collectedObjectText;

    public Text heartRateText;

    GameObject player;
    public Image faderImage;

    [SerializeField]
    int heartRate;

    public FallingBlocks fallingBlockSpawner;

    public PopUpManager popUpManager;

    public GameObject grassForFalling;

    public GameObject platformLink;

    // Start is called before the first frame update
    void Start()
    {
        fallingBlockSpawner = GameObject.Find("FallingBlockSpawner").GetComponent<FallingBlocks>();
        player = GameObject.Find("Player");
        /*collectedObjectText.enabled = false;
        if (collectedObjectText)
        {
            collectedObjectText.enabled = false;
        }*/
        heartRate = 0;
        InvokeRepeating("UpdateHeartBeat", 2, 5f);
        if (heartRate > 400)
        {
            InvokeRepeating("UpdateHeartBeat", 2, 2.5f);
        }
        platformLink = popUpManager.platformLink;
        fadeIn(2f);
    }

    // Update is called once per frame
    void Update()
    {
        if ((heartRate % 5 == 0 || heartRate % 10 == 0) && heartRate > 0)
        {
            Vector3 platformBounds = platformLink.GetComponent<Collider>().bounds.size;
            float spawnPointx = Random.Range(-platformBounds.x/2f, platformBounds.x/2f);
            float spawnPointZ = Random.Range(-platformBounds.z/2f, -platformBounds.z/2f);
            Vector3 pos = new Vector3(spawnPointx, platformBounds.y + 40, spawnPointZ) + platformLink.transform.position;
            fallingBlockSpawner.SpawnBlock(pos);
            //spawn a falling object
            if (heartRate >= 400 && heartRate % 3 == 0)
            {
                //spawn a falling object but quicker
                fallingBlockSpawner.spawnRate = 10000f;
                fallingBlockSpawner.SpawnBlock(pos);
            }
            /*if(cutscene is on)
            {
                stop spawning
            }*/
        }
        else
        {
            fallingBlockSpawner.blockSpawned = false;
        }
        if (heartRate > 1200)
        {
            player.SetActive(false); //temporary death placeholder
            Debug.Log("Dead");
        }
    }

    public void HideText()
    {
        collectedObjectText.enabled = false;
    }

    void UpdateHeartBeat()
    {
        heartRate++;
        heartRateText.text = heartRate.ToString();
    }


    public void UpdateRocketBar(float value)
    {
        rocketFuelBar.value = value;
        rocketFuelBackground.color = Color.Lerp(Color.red, Color.green, value);
    }

    public void toggleRocketBar(bool on)
    {
        rocketFuelContainer.SetActive(on);
    }

    public async Task fadeIn(float duration)
    {
        float timer = duration;
        while (timer > 0)
        {
            faderImage.color = new Color(faderImage.color.r, faderImage.color.g, faderImage.color.b, timer / duration);
            timer -= Time.deltaTime;
            await Task.Yield();
        }
        faderImage.color = new Color(faderImage.color.r, faderImage.color.g, faderImage.color.b, 0);
    }

    public async Task fadeOut(float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            faderImage.color = new Color(faderImage.color.r, faderImage.color.g, faderImage.color.b, timer / duration);
            timer += Time.deltaTime;
            await Task.Yield();
        }
        faderImage.color = new Color(faderImage.color.r, faderImage.color.g, faderImage.color.b, 1);
    }

    public void setFade(float value)
    {
       faderImage.color = new Color(faderImage.color.r, faderImage.color.g, faderImage.color.b, value);
    }
}
