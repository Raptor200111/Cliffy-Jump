using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum StageName
{
    MENU,
    LVL_1,
    LVL_2,
    CREDITS

}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsGamePaused { get; private set; } = false;
    public bool IsGameOver { get; private set; } = false;

    public int CoinsCollected { get; private set; } = 0;
    public event Action<int> OnCoinsChanged;

    public int CurrentWorldScore { get; private set; } = 0;
    public event Action<int> OnLevelScoreChanged;
    public int actualLevel;
    public List<PlayerModelData> playerModels;
    [field: SerializeField] public List<GameObject> Characters { get; private set; }
    [field: SerializeField] public List<WorldInfo> WorldInfos { get; private set; }
    public float[] MaxLevelProgress { get; private set; } = new float[2] { 0f, 0f };
    public int[] MaxLevelScore { get; private set; } = new int[2] { 0, 0 };

    //[field: SerializeField] public List<GameObject> Characters {get; private set; }
    public StageName stageName { get; private set; } = StageName.MENU;
    private SoundManager soundManager;

    public Vector3[] StartPos = new Vector3[2];
    // Start is called before the first frame update
    public void Awake()
    {
        if (GameManager.Instance == null)
        {
            GameManager.Instance = this;
            stageName = StageName.MENU;
            soundManager = SoundManager.Instance;
            CoinsCollected = PlayerPrefs.GetInt("Coins", 0);
            Characters = new List<GameObject>();
            foreach (PlayerModelData playerModel in playerModels)
            {
                GameObject a = Instantiate(playerModel.modelPrefab, this.transform);
                a.name = playerModel.modelName;
                a.SetActive(false);
                Characters.Add(a);
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
    private void Update()
    {        
        if (Input.GetKeyDown(KeyCode.Alpha1)) { 
                changeScene(StageName.LVL_1);

            if (WorldManager.Instance != null )
            {
                GameObject startpos = new GameObject();
                WorldManager.Instance.ReStart(WorldInfos[0]);
                startpos.transform.position = StartPos[0];
                Player.Instance.startPoint = startpos;
                Player.Instance.ResTart();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            changeScene(StageName.LVL_2);

            if (WorldManager.Instance != null)
            {
                GameObject startpos = new GameObject();
                WorldManager.Instance.ReStart(WorldInfos[1]);
                startpos.transform.position = StartPos[1];
                Player.Instance.startPoint = startpos;
                Player.Instance.ResTart();
            }
        }
    }

    public void changeScene(StageName stage) {
        stageName = stage;
        switch (stage)
        {
            case StageName.MENU:
                SceneManager.LoadScene(0);
                soundManager.SetBackgroundMusic(StageName.MENU);
                if (Player.Instance != null)
                {
                    Player.Instance.enabled = false;
                }
                    break;
            case StageName.LVL_1:
                SceneManager.LoadScene(1);
                if(Player.Instance != null)
                {
                    GameObject startpos = new GameObject();
                    startpos.transform.position = StartPos[0];
                    Player.Instance.startPoint = startpos;
                    Player.Instance.ResTart();
                    Player.Instance.enabled = true;
                }
                soundManager.SetBackgroundMusic(StageName.LVL_1);
                break;
            case StageName.LVL_2:
                SceneManager.LoadScene(2);
                if (Player.Instance != null)
                {
                    GameObject startpos = new GameObject();
                    startpos.transform.position = StartPos[1];
                    Player.Instance.startPoint = startpos;
                    Player.Instance.ResTart();
                    Player.Instance.enabled = true;

                }
                soundManager.SetBackgroundMusic(StageName.LVL_2);
                break;
            case StageName.CREDITS:
                SceneManager.LoadScene(3);
                soundManager.SetBackgroundMusic(StageName.CREDITS);
                Player.Instance.enabled = false;
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

    public void AddCoin(GameObject coin)
    {
        CoinsCollected += 1;
        coin.GetComponent<Collectible>().Disappear();
        PlayerPrefs.SetInt("Coins", CoinsCollected);
        OnCoinsChanged?.Invoke(CoinsCollected);
    }

    public void AddStar(GameObject star)
    {
        CurrentWorldScore += 1;
        star.GetComponent<Collectible>().Disappear();
        OnLevelScoreChanged?.Invoke(CurrentWorldScore);
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
        PlayerPrefs.SetInt("PlayerDataIndex", i_indexPlayer);
    }
}