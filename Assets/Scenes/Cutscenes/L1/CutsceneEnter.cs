using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneEnter : MonoBehaviour
{
    public GameObject thePlayer;
    public GameObject mainCam;
    public GameObject cutsceneCam;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("TIME"))
        {
         GameObject.FindGameObjectWithTag("TIME").GetComponent<BoxCollider>().enabled = false;
        cutsceneCam.SetActive(true);
        mainCam.SetActive(false);
            StartCoroutine(FinishCut());
            thePlayer.SetActive(false);

        }
    }

    IEnumerator FinishCut()
    {
        yield return new WaitForSeconds(2);
        thePlayer.SetActive(true);
        mainCam.SetActive(true);
        cutsceneCam.SetActive(false);
    }
}
