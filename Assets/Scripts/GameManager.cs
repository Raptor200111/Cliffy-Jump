using System;
using System.Collections;
using System.Collections.Generic;
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

    public int coinsCollected { get; private set; } = 0;
    public event Action<int> OnCoinsChanged;

    public int HighScore { get; private set; }
    private int[] maxLevelProgress = new int[2];
    public event Action<int> OnLevelProgressChanged;

    [SerializeField] private List<Characters> characters = new List<Characters>();
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject CoinPrefab;
    [SerializeField] private GameObject StarPrefab;
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
            if (characters != null)
            {
                player = characters[PlayerPrefs.GetInt("CharIndex", 0)].character;
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
        coinsCollected += 1;
        OnCoinsChanged?.Invoke(coinsCollected);
    }

    public void SaveLevelProgress(int levelIndex, int progress)
    {
        if (progress > maxLevelProgress[levelIndex])
        {
            String varName = "ProgressLevel" + levelIndex + 1;
            maxLevelProgress[levelIndex] = progress;
            PlayerPrefs.SetInt(varName, progress);
            OnLevelProgressChanged?.Invoke(maxLevelProgress[levelIndex]);
        }
    }

    public int GetMaxLevelProgress(int levelIndex)
    {
        return maxLevelProgress[levelIndex];
    }

    public void SetSelectedPlayer(int i_indexPlayer)
    {
        if (i_indexPlayer > 0 && i_indexPlayer < characters.Count)
        {
            player = characters[i_indexPlayer].character;
            PlayerPrefs.SetInt("CharIndex", i_indexPlayer);
            changeScene(StageName.MENU);
        }
    }

    public Transform GetPlayerTransform()
    {
        return player.transform;
    }

    public List<Characters> GetCharacters() { return characters; }

    public GameObject GetStarPrefab() { return StarPrefab; }
    public GameObject GetCoinPrefab() { return CoinPrefab; }
    // Update is called once per frame
    void Update()
    {

    }
}