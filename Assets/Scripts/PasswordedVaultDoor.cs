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

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim.SetBool("isOpening", false);
    }

    // Update is called once per frame
    void Update()
    {
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
                    anim.SetBool("isOpening", true);
                } else
                {
                    audioSource.clip = negativeTone;
                    audioSource.Play();
                    passwordUITriggered = false;
                }
                Destroy(currentPasswordUI);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E) && !passwordUITriggered)
            {
                currentPasswordUI = Instantiate(passwordUIPrefab);
                passwordUITriggered = true;
                currentPasswordField = currentPasswordUI.rootVisualElement.Q<TextField>("pw-field");
                currentPasswordField.Focus();

            }
        }
    }
}