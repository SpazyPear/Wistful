using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MenuController : MonoBehaviour
{
    public bool GameisPause = false;
    public GameObject PauseMenu;
    public Camera cam;

    public GameObject Resume;
    public GameObject Quit;
    public GameObject Reset;

    public GameObject Settings;
    public AudioMixer audioMixer;
    public bool SettingisActive;

    public 
    // Start is called before the first frame update
    void Start()
    {
        GameisPause = false;
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
