using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class Narration : MonoBehaviour
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
        UiObject.SetActive(false);
        Destroy(theItem);
    }


}
