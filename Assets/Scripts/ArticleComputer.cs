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

    //Bools
    private bool loginTriggered = false; //Keeps track of whether the login page is open
    private bool emailTriggered = false; //Keeps track of whether the email is open

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E) && !loginTriggered)
            {
                currentLoginPage = Instantiate(loginPagePrefab);
                loginTriggered = true;
                currentPasswordField = currentLoginPage.rootVisualElement.Q<TextField>();
                currentPasswordField.Focus();
            }
        }
    }

    private void Update()
    {
        if(loginTriggered && Input.GetKeyDown(KeyCode.Escape)) //If on the login page and hit esc
        {
            //remove login page
            loginTriggered = false;
            if (currentLoginPage)
            {
                Destroy(currentLoginPage);
            }
        }

        if (loginTriggered && Input.GetKeyDown(KeyCode.Return) || loginTriggered && Input.GetKeyDown(KeyCode.KeypadEnter)) //If on the login page and hit enter
        {
            if (currentPasswordField.text.ToLower() == "m0ixvq8w" || currentPasswordField.text.ToLower() == "moixvq8w") //if correct
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
            }
        }

        if (emailTriggered && Input.GetKeyDown(KeyCode.Escape)) //If on the email page and hit esc
        {
            emailTriggered = false;
            if (currentEmailPage)
            {
                Destroy(currentEmailPage);
            }
        }
    }
}
