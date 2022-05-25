using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCutscene : MonoBehaviour
{

    [SerializeField] GameObject _cutscene;
    //public GameObject thePlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TIME"))
        {
            _cutscene.SetActive(true);
            //thePlayer.SetActive(false);
            StartCoroutine(FinishCut());
        }
    }

    IEnumerator FinishCut()
    {
        yield return new WaitForSeconds(5);
        GameObject.FindGameObjectWithTag("TIME").GetComponent<BoxCollider>().enabled = false;
        //thePlayer.SetActive(true);


    }
}
