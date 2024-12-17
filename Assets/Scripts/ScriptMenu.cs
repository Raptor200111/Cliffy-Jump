using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class ScriptMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject popUp;
    public int indexCharacter;
    [SerializeField] private TMP_Text character_name;

    private GameManager gameManager;
    public int level;
    void Start()
    {
        gameManager = GameManager.Instance;
        level = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    public void selectLevel(int i_level)
    {
        level = i_level;
    }

    public void playLevel()
    {
        SceneManager.LoadScene(level);
    }
    public void goToMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void ShowConfigPopUp()
    {
        popUp.SetActive(true);
    }

    public void showSelectPlayer()
    {
        SceneManager.LoadScene(3);
    }
    public void ExitConfigPopUp()
    {
        popUp.SetActive(false);
    }
}
