using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuController : MonoBehaviour

{
    //This script is used to manage the menu system.
    //It is attached to the Start menu canvas and the Levels' pause menu canvas.
    
    //-------------Level pres-----------------
    public bool GameisPause = false;
    public GameObject PauseMenu;
    public Camera cam;

    public GameObject Resume;
    public GameObject Quit;
    public GameObject Reset;

    public GameObject Settings;
    public AudioMixer audioMixer;
    public bool SettingisActive;
    public TMPro.TMP_Text SettingText;

    //-------------Start pres---------------------------
    public TMPro.TMP_Dropdown dropdown;
    public GameObject PlayerPrefab;
    public bool OnStartScene;
    // Start is called before the first frame update
    void Start()
    {
        GameisPause = false;
        sens = PlayerPrefab.GetComponent<Movement>().sensitivity;
        if(SceneManager.GetActiveScene().name == "Level 1")
        {
            OnStartScene = true;
            StartScene.SetActive(true);
            Cursor.lockState = CursorLockMode.None; 
            
        }
        else{
            StartScene.SetActive(false);
            OnStartScene = false;
        }
        
        //DontDestroyOnLoad(this.gameObject);
        //
        //get resolutions of the screen
        // Resolution[] resolutions = Screen.resolutions;
        // //create a list of strings to store the resolutions
        // List<string> resolutionList = new List<string>();
        // //loop through the resolutions
        // for (int i = 0; i < resolutions.Length; i++)
        // {
        //     //add the resolution to the list
        //     resolutionList.Add(resolutions[i].width + " x " + resolutions[i].height);
        // }
        // //set the dropdown to the list
        // Dropdown resolutionDropdown = GameObject.Find("Resolution Dropdown").GetComponent<Dropdown>();
    }

    // Update is called once per frame
    private float sens;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(OnStartScene == false)
            {
                PausetheGame();
            }
        }
        if (GameisPause)
        {

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;        
        }
        
        //debug the current volumn  
        //Debug.Log(AudioListener.volume);
        }

    
    public void AwakeSettings(){
        if (!SettingisActive)
        {
            Resume.SetActive(false);
            Quit.SetActive(false);
            Reset.SetActive(false);
            SettingText.text = "Back";
            Settings.SetActive(true);
            SettingisActive = true;
        }
        else{
            Resume.SetActive(true);
            Quit.SetActive(true);
            Reset.SetActive(true);
            SettingText.text = "Settings";
            Settings.SetActive(false);
            SettingisActive = false;
        }
        
    }
    public GameObject StartMenu;
    public GameObject OptionsMenu;
    public GameObject Title;
    public void GameStart(){
        OnStartScene = false;
        LeanTween.moveLocalY(StartScene, 1500, 1.5f).setEase(LeanTweenType.easeOutQuad);
        Cursor.lockState = CursorLockMode.Locked;
        //load level 1 after 1 second
        Invoke("loadLevel1", 2f);
        
        
    }
    public void applyMouseSenstivity(float value){
        sens = value;
    }
    void loadLevel1(){
        StartScene.SetActive(false);
        //SceneManager.LoadScene("Level 1");
    }
    public void AwakeStartMenu(){
        GameObject.Find("StartClick").SetActive(false);
        // StartMenu.SetActive(true);
        LeanTween.moveLocalY(Title, 200f, 1f).setEase(LeanTweenType.easeOutQuad);
        LeanTween.moveLocalY(StartMenu, 0, 1.5f).setEase(LeanTweenType.easeInBounce);
    }
    public void onClickSettings(){
        // StartMenu.SetActive(false);
        // OptionsMenu.SetActive(true);
        LeanTween.moveLocalY(Title, 300f, 1f).setEase(LeanTweenType.easeOutQuad);
        LeanTween.moveLocalY(StartMenu,-400,1.5f).setEase(LeanTweenType.easeOutQuad);
        LeanTween.moveLocalX(OptionsMenu,0,1.5f).setEase(LeanTweenType.easeOutQuad);
    }
    public void setFullscreen(bool isFullscreen){
        Screen.fullScreen = isFullscreen;
    }
    public void BackButton(){
        // OptionsMenu.SetActive(false);
        // StartMenu.SetActive(true);
        LeanTween.moveLocalY(Title, 200f, 1f).setEase(LeanTweenType.easeOutQuad);
        LeanTween.moveLocalY(StartMenu,0,1.5f).setEase(LeanTweenType.easeOutQuad);
        LeanTween.moveLocalX(OptionsMenu,-1500,1.5f).setEase(LeanTweenType.easeOutQuad);
    }
    public void setVolumn(float vol)
    {
        //AudioListener.volume = vol;
        audioMixer.SetFloat("GameVolumn", vol);
    }
    public void applyFOV(float fovValue){
        cam.fieldOfView = fovValue;
    }
    private void PausetheGame()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0;
        GameisPause = true;
        PlayerPrefab.gameObject.GetComponent<Movement>().sensitivity = 0;

        
    }
    public void ResumetheGame()
    {
        
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
        GameisPause = false;
        PlayerPrefab.gameObject.GetComponent<Movement>().sensitivity = sens;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public GameObject StartScene;
    public void ResettheLevel()
    {
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameisPause = false;
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
    
    public void ReturntoMain()
    {
        SceneManager.LoadScene("Level 1");
        GameisPause = false;
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
    public void QuitGame()
    {
        Application.Quit();
    }

}
