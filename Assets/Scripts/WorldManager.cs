using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DynamicStructures;

public class WorldManager : MonoBehaviour
{
    //enum State { Loading, Moving, Jumping };

    public static WorldManager Instance;
    [SerializeField] public List<GameObject> Characters;
    [SerializeField] public Player player;
    [SerializeField] public DynamicStructures dynamicStructures;
    [SerializeField] public DynamicDetails dynamicDetails;
    [field: SerializeField] public GameObject CoinPrefab { get; private set; }
    [field: SerializeField] public GameObject StarPrefab { get; private set; }

    public int CurrentScreen { get; private set; } = 0;
    public event Action<float> OnLevelProgressChanged;

    public int TotalNumScreens { get; private set; } = 0;
    public int lives { get; private set; } = 3;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DoneRising()
    {
        //dynamicDetails.CreateDetails(dynamicStructures.screen);
        player.PlayerStart();
    }

    public void DestroyDetails()
    {
        //dynamicDetails.DestroyDetails();
    }
    public void ScreenComplete()
    {
        //dynamicDetails.DestroyDetails();
        player.PlayerStop();
        dynamicStructures.NextScreen();
        UpdateLevelProgress();
    }

    public void UpdateLevelProgress()
    {
        CurrentScreen += 1;
        OnLevelProgressChanged?.Invoke(CurrentScreen/(float)TotalNumScreens);
        //GameManager.Instance.SaveLevelProgress(currentWorldProgress);
    }


    public void WorldComplete()
    {
        UnityEngine.Debug.Log("World Complete");
    }

    public void PlayerDeath()
    {
        lives--;

        if (lives == 0)
        {
            UnityEngine.Debug.Log("World Death");
        }
        else
        {
            player.PlayerStart();
        }
    }

    public GameObject GetModelData()
    {
        return Characters[PlayerPrefs.GetInt("PlayerDataIndex", 0)];
    }
}