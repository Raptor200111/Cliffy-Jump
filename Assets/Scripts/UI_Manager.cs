using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public TMP_Text numCoinsText;
    public TMP_Text levelProgress;
    [SerializeField] private GameObject resumePanel;
    [SerializeField] private GameObject pauseButton;

    private void Start()
    {
        SetVisibilityOnPause(false);
    }

    private void OnEnable()
    {
        GameManager.Instance.OnCoinsChanged += UpdateCoins;
        GameManager.Instance.OnLevelProgressChanged += UpdateLevelProgress;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnCoinsChanged -= UpdateCoins;
        GameManager.Instance.OnLevelProgressChanged -= UpdateLevelProgress;
    }

    public void UpdateCoins(int coins)
    {
        if (numCoinsText != null)
        {
            numCoinsText.text = $"{coins}";
        }
    }

    public void UpdateLevelProgress(int progress)
    {
        if (levelProgress != null)
        {
            levelProgress.text = $"{progress}";
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
