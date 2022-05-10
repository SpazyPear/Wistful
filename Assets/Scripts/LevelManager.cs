using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public UIManager uiManager;
    public PopUpManager popUpManager;

    public event EventHandler onVaultOpened;

    int currentLevel = 0;
    bool transitioning;

    private void Awake()
    {
       // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        onVaultOpened += popUpManager.dropBlocks;
        onVaultOpened += popUpManager.spawnLevelLink;
        currentLevel = SceneManager.GetActiveScene().buildIndex;
    }

    public async Task<int> nextLevel()
    {
        if (!transitioning)
        {
            transitioning = true;
            await uiManager.fadeOut(2f);
            currentLevel++;
            SceneManager.LoadSceneAsync(currentLevel, LoadSceneMode.Single);
            transitioning = false;

        }
        return currentLevel;
    }

    public void invokeVaultOpened()
    {
        onVaultOpened?.Invoke(this, new EventArgs());
    }
}
