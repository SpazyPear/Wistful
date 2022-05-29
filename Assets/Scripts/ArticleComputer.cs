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

    //Bools
    private bool loginTriggered = false; //Keeps track of whether the login page is open
    private bool emailTriggered = false; //Keeps track of whether the email is open

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
                GameObject.FindGameObjectWithTag("DropOutWall").SetActive(false);

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
}