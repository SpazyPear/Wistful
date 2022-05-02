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

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        onVaultOpened += popUpManager.dropBlocks;
        onVaultOpened += popUpManager.spawnPlatformLink;
    }

    public async Task<int> nextLevel()
    {
        await uiManager.fadeOut(2f);
        currentLevel++;
        SceneManager.LoadSceneAsync(currentLevel, LoadSceneMode.Single);
        //await uiManager.fadeIn(2f);
        return currentLevel;
    }

    public void invokeVaultOpened()
    {
        onVaultOpened?.Invoke(this, new EventArgs());
    }
}
