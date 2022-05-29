using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class NarrationWindow : MonoBehaviour
{

    public GameObject UiObject;
    public GameObject theItem;
    // Start is called before the first frame update
    
    void Start()
    {
        UiObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            UiObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        StartCoroutine(WaitBeforeDisappear());
        
    }

    IEnumerator WaitBeforeDisappear()
    {
        yield return new WaitForSeconds(6);
        UiObject.SetActive(false);
        Destroy(theItem);
    }
}
