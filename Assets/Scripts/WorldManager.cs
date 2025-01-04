using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DynamicStructures;

public class WorldManager : MonoBehaviour
{
    //enum State { Loading, Moving, Jumping };

    public static WorldManager Instance;
    [SerializeField] public Player player;
    [SerializeField] public DynamicStructures dynamicStructures;
    [SerializeField] public DynamicDetails dynamicDetails;
    [field: SerializeField] public GameObject CoinPrefab { get; private set; }
    [field: SerializeField] public GameObject StarPrefab { get; private set; }

    public int CurrentScreen { get; private set; } = 0;
    public event Action<float> OnLevelProgressChanged;

    public int TotalNumScreens { get; private set; } = 0;

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
        dynamicDetails.CreateDetails(dynamicStructures.screen);
        player.PlayerStart();
    }

    public void DestroyDetails()
    {
        dynamicDetails.DestroyDetails();
    }
    public void ScreenComplete()
    {
        dynamicDetails.DestroyDetails();
        player.PlayerStop('f');
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
        SoundManager.PlaySound(SoundType.WIN);
        GameManager.Instance.SaveLevelProgress(CurrentScreen/(float)TotalNumScreens);
        GameManager.Instance.changeScene(StageName.MENU);
    }

    public void PlayerDeath()
    {
        dynamicStructures.ResetWorld();
        //player.PlayerStart();
    }

    public void ReStart(WorldInfo worldInfo)
    {
        dynamicStructures.screen = 0;
        player = worldInfo.Player.GetComponent<Player>();
        dynamicStructures.ResetWorldInfo(worldInfo);
        dynamicDetails.ResetWorldInfo(worldInfo);
    }

    public GameObject GetModelData()
    {
        return GameManager.Instance.Characters[PlayerPrefs.GetInt("PlayerDataIndex", 0)];
    }

}