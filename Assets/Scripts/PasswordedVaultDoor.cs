using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class PasswordedVaultDoor : VaultDoor
{
    public UIDocument passwordUIPrefab;
    private UIDocument currentPasswordUI;
    private bool passwordUITriggered = false;
    private TextField currentPasswordField;

    public AudioSource audioSource;
    public AudioClip successChime;
    public AudioClip negativeTone;
    bool collidingWithPlayer;

    // Start is called before the first frame update
    void Start()
    {
        //anim.SetBool("isOpening", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !passwordUITriggered && collidingWithPlayer)
        {
            collidingWithPlayer = false;
            currentPasswordUI = Instantiate(passwordUIPrefab);
            passwordUITriggered = true;
            currentPasswordField = currentPasswordUI.rootVisualElement.Q<TextField>("pw-field");
            currentPasswordField.Focus();

        }
        if (passwordUITriggered) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Destroy(currentPasswordUI);
                passwordUITriggered = false;
            }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (currentPasswordField.text.ToLower() == "hunter")
                {
                    audioSource.clip = successChime;
                    audioSource.Play();
                    Destroy(currentPasswordUI);
                    base.toggleDoor();
                } 
                else
                {
                    audioSource.clip = negativeTone;
                    audioSource.Play();
                    passwordUITriggered = false;
                }
                Destroy(currentPasswordUI);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            collidingWithPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            collidingWithPlayer = false;
        }
    }
}