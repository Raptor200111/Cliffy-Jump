using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //enum State { Loading, Moving, Jumping };

    public static GameManager Instance;
    public List<Characters> characters = new List<Characters>();

    public Player player;
    public DynamicStructures dynamicStructures;

    public void Awake()
    {
        if (GameManager.Instance == null) 
        { 
            GameManager.Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DoneRising()
    {
        player.PlayerStart();
    }

    public void ScreenComplete()
    {
        player.PlayerStop();
        dynamicStructures.HiddenObjectsChange();
    }

    public void WorldComplete()
    {
        UnityEngine.Debug.Log("World Complete");
    }
}