using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StageName
{
    MENU
/*    LVL_1,
    LVL_2,
    PLYR_SELECT,
    INSTRUCTIONS,
    CREDITS*/
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private List<Characters> characters = new List<Characters>();
    [SerializeField] private GameObject player;
    public StageName stageName;
    private SoundManager soundManager;
    // Start is called before the first frame update
    public void Awake()
    {
        if (GameManager.Instance == null) 
        { 
            GameManager.Instance = this;
            stageName = StageName.MENU;
            soundManager = SoundManager.Instance;
            soundManager.SetBackgroundMusic(stageName);
            if (characters != null)
            {
                player = characters[0].character;
            }
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Transform GetPlayerTransform()
    {
        return player.transform;
    }

    public void SetPlayer(int i_indexPlayer)
    {
        player = characters[i_indexPlayer].character;
    }

    public List<Characters> GetCharacters() { return characters; }
    // Update is called once per frame
    void Update()
    {
        
    }
}
