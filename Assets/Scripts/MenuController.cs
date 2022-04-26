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
    //-------------Start pres---------------------------

    // Start is called before the first frame update
    void Start()
    {
        GameisPause = false;
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
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PausetheGame();
        }
        if (GameisPause)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;        
        }
        //debug the current volumn  
        Debug.Log(AudioListener.volume);
        }
    public void AwakeSettings(){
        if (!SettingisActive)
        {
            Resume.SetActive(false);
            Quit.SetActive(false);
            Reset.SetActive(false);
            Settings.SetActive(true);
            SettingisActive = true;
        }
        else{
            Resume.SetActive(true);
            Quit.SetActive(true);
            Reset.SetActive(true);
            Settings.SetActive(false);
            SettingisActive = false;
        }
        
    }
    public GameObject StartMenu;
    public GameObject OptionsMenu;
    public GameObject Title;
    public GameObject ReadyforCutScene;
    public void GameStart(){
        LeanTween.scale(ReadyforCutScene, new Vector3(1, 1, 1), 1f);
        //load level 1 after 1 second
        Invoke("loadLevel1", 1f);
        
    }
    void loadLevel1(){
        SceneManager.LoadScene(1);
    }
    private Vector3 TitleOriginPos;
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
    }
    public void ResumetheGame()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
        GameisPause = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ResettheLevel()
    {
        /*PauseMenu.SetActive(false);
        Time.timeScale = 1;
        GameisPause = false;
        SceneManager.LoadScene("Level 1");*/
    }
    
    public void ReturntoMain()
    {
        //
    }
    public void QuitGame()
    {
        Application.Quit();
    }

}
