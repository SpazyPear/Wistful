using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class ArticleComputer : MonoBehaviour
{
    //UIDocs
    public UIDocument loginPagePrefab;
    private UIDocument currentLoginPage;
    public UIDocument emailPrefab;
    private UIDocument currentEmailPage;

    //UIElements
    private TextField currentPasswordField; //Password field on login page
    private Button nextEmailBtn;
    private Button prevEmailBtn;
    private Label from;
    private Label cc;
    private Label line1;
    private Label line2;
    private Label line3;

    //Bools
    private bool loginTriggered = false; //Keeps track of whether the login page is open
    private bool emailTriggered = false; //Keeps track of whether the email is open

    int emailNum = 1;

    Movement movement;

    PopUpManager popUpManager;
    UIManager uIManager;

    private void Start()
    {
        popUpManager = GameObject.Find("PopUpManager").GetComponent<PopUpManager>();
        uIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>().updateInteractPrompt("Press E To Interact");

            if (Input.GetKeyDown(KeyCode.E) && !loginTriggered)
            {
                movement = other.GetComponent<Movement>();
                movement.canMove = false;
                currentLoginPage = Instantiate(loginPagePrefab);
                loginTriggered = true;
                uIManager.findObject3Text.enabled = false;
                currentPasswordField = currentLoginPage.rootVisualElement.Q<TextField>();
                currentPasswordField.Focus();
                GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>().updateInteractPrompt("");
            }
        }
    }

    private void Update()
    {
        if (loginTriggered && Input.GetKeyDown(KeyCode.Escape)) //If on the login page and hit esc
        {
            //remove login page
            loginTriggered = false;
            if (currentLoginPage)
            {
                Destroy(currentLoginPage);
                movement.canMove = true;
            }
        }

        if (loginTriggered && Input.GetKeyDown(KeyCode.Return) || loginTriggered && Input.GetKeyDown(KeyCode.KeypadEnter)) //If on the login page and hit enter
        {
            if (currentPasswordField.text == "m0iXvq8W") //if correct
            {
                //remove login page
                loginTriggered = false;
                if (currentLoginPage)
                {
                    Destroy(currentLoginPage);
                }

                //get email page up
                currentEmailPage = Instantiate(emailPrefab);
                emailTriggered = true;
                from = currentEmailPage.rootVisualElement.Q<Label>("from");
                cc = currentEmailPage.rootVisualElement.Q<Label>("cc");
                line1 = currentEmailPage.rootVisualElement.Q<Label>("line1");
                line2 = currentEmailPage.rootVisualElement.Q<Label>("line2");
                line3 = currentEmailPage.rootVisualElement.Q<Label>("line3");
                nextEmailBtn = currentEmailPage.rootVisualElement.Q<Button>("nextButton");
                nextEmailBtn.clickable.clicked += () =>
                {
                    Debug.Log("Clicked next");
                    if (emailNum > 0 && emailNum < 3)
                    {
                        emailNum++;
                        OnEmailChange(emailNum);
                    }
                };
                prevEmailBtn = currentEmailPage.rootVisualElement.Q<Button>("prevButton");
                prevEmailBtn.clickable.clicked += () =>
                {
                    Debug.Log("Clicked prev");
                    if (emailNum > 1 && emailNum < 4)
                    {
                        emailNum--;
                        OnEmailChange(emailNum);

                    }
                };
                GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>().updateHoldPrompt("");
            }
        }

        if (emailTriggered && Input.GetKeyDown(KeyCode.Escape)) //If on the email page and hit esc
        {
            emailTriggered = false;
            if (currentEmailPage)
            {
                Destroy(currentEmailPage);
                movement.canMove = true;
                popUpManager.readyForNextItemSpawn = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>().updateInteractPrompt("");
        }
    }

    private void OnEmailChange(int emailNum)
    {

        switch(emailNum)
        {
            case 1:
                from.text = "From: JAXA President";
                cc.text = "CC: RFSA Director, ESA Director General";
                line1.text = "Got results back and they're worse than we thought. Earth has 10 years left, max.";
                line2.text = "We need to start pouring resources into space exploration and deep space travel.";
                line3.text = "If we don't find a habitable planet soon and start evacuating, humanity is screwed.";
                break;
            case 2:
                from.text = "From: RFSA Director";
                cc.text = "CC: JAXA President, ESA Director General";
                line1.text = "We might be out of time already. Our current sh??s would take at least tw? years to leave the sol????ystem.";
                line2.text = "Recr??tment is important but we??????ur people surveyi?????thin months.";
                line3.text = "??i??????in time. W??????????????????????????????ough resources to survive that long????suicide mission.";
                break;
            case 3:
                from.text = "From: ESA Director General";
                cc.text = "CC: JAXA President, RFSA Director";
                line1.text = "???????????????????????????????";
                line2.text = "?????????????????????????????????????????????????????????";
                line3.text = "??????????help us????????????????goodbye and good luck";
                break;
            default:
                emailTriggered = false;
                if (currentEmailPage)
                {
                    Destroy(currentEmailPage);
                    movement.canMove = true;
                    popUpManager.readyForNextItemSpawn = true;
                }
                break;
        }
    }
}