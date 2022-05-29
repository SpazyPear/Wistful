using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCutscene : MonoBehaviour
{

    [SerializeField] GameObject _cutscene;
    //public GameObject thePlayer;
    public GameObject PlayerPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TIME"))
        {
            _cutscene.SetActive(true);
            PlayerPrefab.gameObject.GetComponent<Movement>().enabled = false;
            StartCoroutine(FinishCut());
            StartCoroutine(MovementEnable());
        }
    }

    IEnumerator FinishCut()
    {
        yield return new WaitForSeconds(1);
        GameObject.FindGameObjectWithTag("TIME").GetComponent<BoxCollider>().enabled = false;
    }

    IEnumerator MovementEnable()
    {
        yield return new WaitForSeconds(39);
        PlayerPrefab.gameObject.GetComponent<Movement>().enabled = true;
    }
}
