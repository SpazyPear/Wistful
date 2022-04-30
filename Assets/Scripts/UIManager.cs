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

    public Image faderImage;

    [SerializeField]
    int heartRate;

    // Start is called before the first frame update
    void Start()
    {
        collectedObjectText.enabled = false;
        heartRate = 0;
        if (!findObject3Text.enabled)
        {
            InvokeRepeating("UpdateHeartBeat", 2, 0.8f);
        }
        else
        {
            InvokeRepeating("UpdateHeartBeat", 2, 0.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (heartRate > 120)
        {
            Debug.Log("dead");
        }
    }

    public void HideText()
    {
        collectedObjectText.enabled = false;
    }

    void UpdateHeartBeat()
    {
        if (!findObject3Text.enabled)
        {
            heartRate = UnityEngine.Random.Range(80, 110);
        }
        else
        {
            heartRate = UnityEngine.Random.Range(60, 100);
        }
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
