using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public bool GameisPause = false;
    public GameObject PauseMenu;
    // Start is called before the first frame update
    void Start()
    {
        
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
            Cursor.lockState = CursorLockMode.None;        }
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
