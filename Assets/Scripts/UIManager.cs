using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{
    Color32 c = new Color32(172, 222, 94, 255);

    public Slider rocketFuelBar;
    public GameObject rocketFuelContainer;
    public Image rocketFuelBackground;

    public TMP_Text collectedObjectText;
    public Text findObject1Text;
    public Text findObject2Text;
    public Text findObject3Text;
    public Text findObject4Text;

    public Text heartRateText;

    GameObject player;
    public Image faderImage;

    [SerializeField]
    static int heartRate;

    public TMP_Text interactPrompt;

    public TMP_Text holdPrompt;

    public PopUpManager popUpManager;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        interactPrompt = GameObject.FindGameObjectWithTag("InteractPrompt").GetComponent<TMP_Text>();
        fadeIn(2f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdateHeartBeat()
    {
        heartRate++;
        heartRateText.text = heartRate.ToString();
    }

    public void updateInteractPrompt(string newContent)
    {
        interactPrompt.text = newContent;
    }

    public void updateHoldPrompt(string newContent)
    {
        holdPrompt.text = newContent;
    }


    public void UpdateRocketBar(float value)
    {
        rocketFuelBar.value = value;
        rocketFuelBackground.color = Color.Lerp(Color.red, c, value);
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

    public void GoToVaultlvl2()
    {
        collectedObjectText.text = "Go to the Vault";
        StartCoroutine(HideText());
    }

    public IEnumerator HideText()
    {
        yield return new WaitForSeconds(2.7f);
        collectedObjectText.text = "";
    }
}
