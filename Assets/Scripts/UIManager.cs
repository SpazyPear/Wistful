using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    int heartRate;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        collectedObjectText.enabled = false;
        heartRate = 0;
        InvokeRepeating("UpdateHeartBeat", 2, 5f);
        if(heartRate > 250)
        {
        InvokeRepeating("UpdateHeartBeat", 2, 2.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((heartRate % 5 == 0 || heartRate % 10 == 0))
        {
            if (heartRate > 0 && heartRate < 120)
            {
                //spawn a falling object
            }
            if (heartRate >= 120)
            {
                //spawn a falling object but quicker
            }
            /*if(cutscene is on)
            {
                stop spawning
            }*/
        }
        if (heartRate > 500)
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
}
