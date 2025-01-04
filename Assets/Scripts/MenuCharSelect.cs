using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using Unity.VisualScripting;

public class MenuCharSelect : MonoBehaviour
{
    private int oldIndex;
    [SerializeField] private GameObject charImage;
    [SerializeField] private TMP_Text charName;
    [SerializeField] private GameObject[] charsToDisplay;
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        oldIndex = PlayerPrefs.GetInt("PlayerDataIndex", 0);
        charsToDisplay = GameManager.Instance.Characters.ToArray();
        if (GameManager.Instance.Characters != null) {
            charsToDisplay =new  GameObject[GameManager.Instance.Characters.Count];
            for( int i = 0; i< GameManager.Instance.Characters.Count; i++)
            {
                GameObject a = Instantiate(GameManager.Instance.Characters[i], charImage.transform);
                a.transform.localScale = a.transform.localScale * 125f;
                a.name = GameManager.Instance.Characters[i].name;
                charsToDisplay[i] = a;
            } 
        }
        ChangeChar(oldIndex);
 
    }

    private void ChangeChar(int newIndex) 
    {
        if(oldIndex < 0)
        {
            oldIndex = 0;
        }
        if(newIndex < 0) { newIndex = 0; }
        charsToDisplay[oldIndex].SetActive(false);
        charsToDisplay[newIndex].SetActive(true);
        charName.text = charsToDisplay[newIndex].name;
        oldIndex = newIndex;
    }

    public void NextChar() 
    {
        int newIndex = oldIndex+1;
        if(newIndex > charsToDisplay.Length-1) {  newIndex = 0; } 
        ChangeChar(newIndex);
    }


    public void PreviousChar()
    {
        int newIndex = oldIndex-1;
        if(newIndex < 0)
        {
            newIndex = charsToDisplay.Length - 1;
        }

        ChangeChar(newIndex);
    }


    public void SelectChar()
    {
        PlayerPrefs.SetInt("PlayerSelected", 0);
        gameManager.SetSelectedPlayer(oldIndex); 
        if(GameManager.Instance.actualLevel == 1)
        {
            GameManager.Instance.changeScene(StageName.LVL_1);

        }
        else
        {
            GameManager.Instance.changeScene(StageName.LVL_2);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
