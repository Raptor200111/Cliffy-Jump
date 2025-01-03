using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public TMP_Text numCoinsText;
    public TMP_Text levelProgress;
    public GameObject[] NumStars;
    [SerializeField] private GameObject resumePanel;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private WorldManager worldManager;

    private void Start()
    {
        SetVisibilityOnPause(false);
        numCoinsText.text = $"{GameManager.Instance.CoinsCollected}";
        if(worldManager == null)
        {
            Debug.LogWarning("worldManager not assigned");
            return;
        }
        if(WorldManager.Instance.CurrentScreen < 0 || worldManager.TotalNumScreens <= 0)
        {
            levelProgress.text = $"{0} %";
        }
        else {
            float progress = WorldManager.Instance.CurrentScreen / (float)WorldManager.Instance.TotalNumScreens;
            levelProgress.text = $"{progress}  %";
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.OnCoinsChanged += UpdateCoins;
        WorldManager.Instance.OnLevelProgressChanged += UpdateLevelProgress;
        GameManager.Instance.OnLevelScoreChanged += UpdateStars;
    }

    
    private void OnDisable()
    {
        GameManager.Instance.OnCoinsChanged -= UpdateCoins;
        WorldManager.Instance.OnLevelProgressChanged -= UpdateLevelProgress;
        GameManager.Instance.OnLevelScoreChanged -= UpdateStars;
    }

    public void OnGoToMenu()
    {
        GameManager.Instance.changeScene(StageName.MENU);
    }

    public void UpdateCoins(int coins)
    {
        if (numCoinsText != null)
        {
            numCoinsText.text = $"{coins}";
        }
    }
    public void UpdateStars(int stars)
    {
        if(NumStars != null && NumStars.Length > stars)
        {
            for (int i = 0; i< stars; i++)
            {
                NumStars[i].SetActive(true);
            }
        }
    }

    public void UpdateLevelProgress(float progress)
    {
        if (levelProgress != null)
        {
            levelProgress.text = $"{progress} %";
        }
    }

    public void Pause()
    {
        GameManager.Instance.PauseGame();
        SetVisibilityOnPause(true);
    }

    public void Resume()
    {
        GameManager.Instance.ResumeGame();
        SetVisibilityOnPause(false);
    }

    private void SetVisibilityOnPause(bool isOnPause)
    {
        pauseButton.SetActive(!isOnPause);
        resumePanel.SetActive(isOnPause);
    }

}
