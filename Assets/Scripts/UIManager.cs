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

    //public Text collectedObjectText;

    public Text heartRateText;

    GameObject player;
    public Image faderImage;

    [SerializeField]
    static int heartRate;

    public PopUpManager popUpManager;

    public GameObject grassForFalling;

    public GameObject platformLink;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        /*collectedObjectText.enabled = false;
        if (collectedObjectText)
        {
            collectedObjectText.enabled = false;
        }*/
        heartRate = 0;
            InvokeRepeating("UpdateHeartBeat", 2, 5f);
            if (heartRate > 200)
            {
                InvokeRepeating("UpdateHeartBeat", 2, 2.5f);
            }
        platformLink = popUpManager.platformLink;
        fadeIn(2f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*public void HideText()
    {
        collectedObjectText.enabled = false;
    }*/

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
