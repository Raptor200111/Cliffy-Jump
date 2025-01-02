using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum StageName
{
    MENU,
    LVL_1,
    LVL_2,
    PLYR_SELECT
    /*    INSTRUCTIONS,
        CREDITS*/
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsGamePaused { get; private set; } = false;
    public bool IsGameOver { get; private set; } = false;

    public int CoinsCollected { get; private set; } = 0;
    public event Action<int> OnCoinsChanged;

    public float[] MaxLevelProgress { get; private set; } = new float[2] { 0f, 0f };
    public int[] MaxLevelScore { get; private set; } = new int[2] { 0, 0 };


    [field: SerializeField] public List<PlayerModelData> PlayersList { get; private set; }
    [field: SerializeField] public GameObject Player { get; private set; }
    [field: SerializeField] public GameObject CoinPrefab { get; private set; }
    [field: SerializeField] public GameObject StarPrefab { get; private set; }

    public StageName stageName { get; private set; } = StageName.MENU;
    private SoundManager soundManager;
    // Start is called before the first frame update
    public void Awake()
    {
        if (GameManager.Instance == null)
        {
            GameManager.Instance = this;
            stageName = StageName.MENU;
            soundManager = SoundManager.Instance;
            if (Player == null)
            {
                Debug.LogError("Player not assigned");
            }
            else if(Player.GetComponent<Player>() == null)
            {
                Debug.LogError("Player's Script not assigned");
            }
            else
            {
                //TO DO: Create Animations
                Player.GetComponent<Player>().LoadModel(PlayersList[PlayerPrefs.GetInt("PlayerDataIndex", 0)]);
            }

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (soundManager == null)
        {
            soundManager = SoundManager.Instance;
            if (soundManager == null)
            {
                Debug.LogWarning("Gamemanager's soundManager null");
                return;
            }
        }
        soundManager.SetBackgroundMusic(stageName);

    }

    public void changeScene(StageName stage) {
        stageName = stage;
        switch (stage)
        {
            case StageName.MENU:
                SceneManager.LoadScene(0);
                soundManager.SetBackgroundMusic(StageName.MENU);
                break;
            case StageName.LVL_1:
                SceneManager.LoadScene(1);
                break;
            case StageName.LVL_2:
                SceneManager.LoadScene(2);
                break;
            case StageName.PLYR_SELECT:
                SceneManager.LoadScene(3);
                break;
        }
    }

    public void PauseGame()
    {
        IsGamePaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        IsGamePaused = false;
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    public void AddCoin()
    {
        CoinsCollected += 1;
        OnCoinsChanged?.Invoke(CoinsCollected);
    }

    public void SaveLevelScore(int score)
    {
        if (stageName != StageName.LVL_1 || stageName != StageName.LVL_2) { return; }
        int levelIndex = (int)stageName - 1;

        if (score > MaxLevelScore[levelIndex])
        {
            String varName = "MaxLevelScore" + (int)stageName;
            MaxLevelScore[levelIndex] = score;
            PlayerPrefs.SetInt(varName, score);
            //OnLevelProgressChanged?.Invoke(maxLevelProgress[levelIndex]);
        }
    }

    public void SaveLevelProgress(float progress)
    {
        if (stageName != StageName.LVL_1 || stageName != StageName.LVL_2) { return; }
        int levelIndex = (int)stageName -1;
        
        if (progress > MaxLevelProgress[levelIndex])
        {
            String varName = "ProgressLevel" + levelIndex + 1;
            MaxLevelProgress[levelIndex] = progress;
            PlayerPrefs.SetFloat(varName, progress);
            //OnLevelProgressChanged?.Invoke(maxLevelProgress[levelIndex]);
        }
    }

    public void SetSelectedPlayer(int i_indexPlayer)
    {
        if (i_indexPlayer > 0 && i_indexPlayer < PlayersList.Count)
        {
            Player.GetComponent<Player>().LoadModel(PlayersList[i_indexPlayer]);
            PlayerPrefs.SetInt("PlayerDataIndex", i_indexPlayer);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}