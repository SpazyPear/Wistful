using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public Slider rocketFuelBar;
    public GameObject rocketFuelContainer;
    public Image rocketFuelBackground;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
