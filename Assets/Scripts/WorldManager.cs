using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    //enum State { Loading, Moving, Jumping };

    public static WorldManager Instance;
    public List<Characters> characters = new List<Characters>();

    public Player player;
    public DynamicStructures dynamicStructures;

    public int lives = 3;

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
        player.PlayerStart();
    }

    public void ScreenComplete()
    {
        player.PlayerStop();
        dynamicStructures.NextScreen();
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
            //yield return new WaitForSeconds(1);
            player.PlayerStart();
        }
    }
}