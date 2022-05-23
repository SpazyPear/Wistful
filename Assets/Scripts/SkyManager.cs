using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyManager : MonoBehaviour
{
    public GameObject StarField;
    public Material skyMaterial;
    public AnimationCurve starCurve;
    public GameObject rendsParent;
    List<SpriteRenderer> rends = new List<SpriteRenderer>();
    public List<Sprite> correctSprites = new List<Sprite>();
    public Sprite XSprite;
    public List<int> correctRots = new List<int>();
    public PopUpManager popUpManager;

    public UIManager uIManager;
    bool isSpinning;


    // Start is called before the first frame update
    void Start()
    {
        uIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        for (int x = 0; x < rendsParent.transform.childCount; x++)
        {
            rends.Add(rendsParent.transform.GetChild(x).GetComponent<SpriteRenderer>());
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator spin(float duration, bool rotsCorrect)
    {
        if (!isSpinning)
        {
            isSpinning = true;
            float timer = 0;
            bool constUpdated = false;
            float initialOffset = skyMaterial.GetTextureOffset("_MainTex").x;
            while (timer < 1)
            {
                if (timer > 0.6 && !constUpdated)
                {
                    displayAnswer(rotsCorrect);

                    constUpdated = true;
                }

                timer += Time.deltaTime / duration;
                float offset = starCurve.Evaluate(timer) / 6f;
                StarField.transform.eulerAngles += new Vector3(0, offset / 0.03f, 0);
                skyMaterial.SetTextureOffset("_MainTex", new Vector2(initialOffset + offset * 6, 0));
                yield return null;
            }
            popUpManager.readyForNextItemSpawn = true;
            isSpinning = false;
        }
    }

    void displayAnswer(bool correct)
    {
        if (correct)
        {
            for (int x = 0; x < rends.Count; x++)
            {
                rends[x].sprite = correctSprites[x];
                popUpManager.readyForNextItemSpawn = true;
            }
            uIManager.GoToVaultlvl2();
        }
        else
        {
            rends[0].sprite = XSprite;

            for (int x = 1; x < rends.Count; x++)
            {
                rends[x].sprite = null;
            }
        }
    }

}
