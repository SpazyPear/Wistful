using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : Item
{
    UIManager uiManager;
    GameObject openBookInst;
    bool isHolding;
    // Start is called before the first frame update
    void Start()
    {
        transform.eulerAngles = Vector3.zero;

        openBookInst = Instantiate((Resources.Load("OpenBook") as GameObject), Vector3.zero, Quaternion.identity);
        openBookInst.transform.eulerAngles += new Vector3(0, 180, 180);
        openBookInst.transform.localScale = new Vector3(0.0002f, 0.0002f, 0.0002f);
        openBookInst.transform.SetParent(transform, true);
        openBookInst.transform.localPosition = new Vector3(0, 1f, 3);
        openBookInst.SetActive(false);

        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        uiManager.updateHoldPrompt("Press 1 or 2 To Switch Items");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            isHolding = !isHolding;
            openBookInst.SetActive(isHolding);

            if (isHolding)
            {
                uiManager.updateHoldPrompt("");

            }
            else
            {
                uiManager.updateHoldPrompt("Press 1 or 2 To Switch Items");
            }
        }
    }
}
