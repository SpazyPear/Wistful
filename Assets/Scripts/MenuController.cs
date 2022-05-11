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
    
    #region Variables
    public bool GameisPause = false;
    public static bool isPromptActive = false;
    public GameObject PauseMenu;
    public Camera cam;
    public static string NewPromptText = "";
    public GameObject Resume;
    public GameObject Quit;
    public GameObject Reset;



    public GameObject Settings;
    public AudioMixer audioMixer;
    public bool SettingisActive;
    public TMPro.TMP_Text SettingText;
    public TMPro.TMP_Dropdown dropdown;
    public GameObject PlayerPrefab;
    public bool OnStartScene;
    private float sens;
    public GameObject StartMenu;
    public GameObject OptionsMenu;
    public GameObject Title;
    public GameObject StartScene;
    public GameObject PromptMenu;
    public TMPro.TMP_Text PromptText;
    #endregion
    #region Start&Update
    void Start()
    {
        PromptMenu.SetActive(false);
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

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
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
        if(isPromptActive){
            SetPromptText(NewPromptText);
            activatePrompt();
        }
        else{
            LeanTween.scale(PromptMenu, new Vector3(0,0,0), 0.5f).setEase(LeanTweenType.easeOutQuad);
            Invoke("deactivatePrompt", 0.5f);
        }
        if(isPromptActive && Input.GetKeyDown(KeyCode.Return)){
            isPromptActive = false;
        }
        
    }
    #endregion
    #region Menu functions
    public void GameStart(){
        OnStartScene = false;
        LeanTween.moveLocalY(StartScene, 1500, 1.5f).setEase(LeanTweenType.easeOutQuad);
        Cursor.lockState = CursorLockMode.Locked;
        //load level 1 after 1 second
        Invoke("transitionAnimation", 1.5f);
        Invoke("loadLevel1", 3f);
    }
    public void disablethings(){
        StartMenu.SetActive(false);
        OptionsMenu.SetActive(false);
        Title.SetActive(false);
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
    public void transitionAnimation(){
        disablethings();
        NewPromptText = "Block generation is now active";
        //isPromptActive = true;
        SetPromptText("Block generation is in progress...test text");
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
    #endregion
    #region Button Functions
    public void ResumetheGame()
    {
        
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
        GameisPause = false;
        PlayerPrefab.gameObject.GetComponent<Movement>().sensitivity = sens;
        Cursor.lockState = CursorLockMode.Locked;
    }
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
    #endregion
    #region prompt Functions
    void activatePrompt(){
        PromptMenu.SetActive(true);
        LeanTween.scale(PromptMenu, new Vector3(1,1,1), 0.5f).setEase(LeanTweenType.easeOutQuad);
    }
    void deactivatePrompt(){
        
        PromptMenu.SetActive(false);
    }
     
    public void SetPromptText(string text){
        PromptText.text = text;
    }
    #endregion
}
