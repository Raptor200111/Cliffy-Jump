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

    // Game data
    public int CurrentScore { get; private set; } = 0;
    public int HighScore { get; private set; }

    [SerializeField] private List<Characters> characters = new List<Characters>();
    [SerializeField] private GameObject player;
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

    public void RestartLevel()
    {
        changeScene(stageName);
    }

    public void AddScore(int points)
    {
        CurrentScore += points;
        Debug.Log($"Score: {CurrentScore}");
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

    public Transform GetPlayerTransform()
    {
        return player.transform;
    }

    //only called from selectplayer 
    public void SetPlayer(int i_indexPlayer)
    {
        if (i_indexPlayer > 0 && i_indexPlayer < characters.Count)
        {
            player = characters[i_indexPlayer].character;
            PlayerPrefs.SetInt("CharIndex", i_indexPlayer);
            changeScene(StageName.MENU);
        }
    }

    public List<Characters> GetCharacters() { return characters; }
    // Update is called once per frame
    void Update()
    {
        
    }
}
