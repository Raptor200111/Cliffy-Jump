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
    public List<Characters> characters = new List<Characters>();
    public Characters player;
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
                player = characters[0];
            }
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
