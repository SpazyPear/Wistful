using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyManager : MonoBehaviour
{
    public GameObject StarField;
    public Material skyMaterial;
    public AnimationCurve starCurve;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(spin(10f));
        }
    }

    IEnumerator spin(float duration)
    {
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / duration;
            StarField.transform.eulerAngles += new Vector3(0, starCurve.Evaluate(timer) / 0.1f, 0);
            skyMaterial.SetTextureOffset("_MainTex", new Vector2(starCurve.Evaluate(timer), 0));
            yield return null;
        }
    }

}
