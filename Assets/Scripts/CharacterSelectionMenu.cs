using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using Unity.VisualScripting;

public class CharacterSelectionMenu : MonoBehaviour
{
    private int oldIndex;
    [SerializeField] private GameObject characterImage;
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private GameObject[] charactersToDisplay;
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        oldIndex = PlayerPrefs.GetInt("PlayerDataIndex", 0);
        var aux = GameManager.Instance.Characters.ToArray();
        if (aux != null) {
            charactersToDisplay = new  GameObject[aux.Length];
            for( int i = 0; i< aux.Length; i++)
            {
                GameObject a = Instantiate(aux[i], characterImage.transform);
                a.transform.localScale = a.transform.localScale * 125f;
                a.name = aux[i].name;
                charactersToDisplay[i] = a;
                charactersToDisplay[i].SetActive(false);
            }
        }

        ChangeChar(oldIndex);
    }

    private void ChangeChar(int newIndex) 
    {
        charactersToDisplay[oldIndex].SetActive(false);
        charactersToDisplay[newIndex].SetActive(true);
        characterName.text = charactersToDisplay[newIndex].name;
        oldIndex = newIndex;
    }

    public void NextChar() 
    {
        int newIndex = oldIndex+1;
        if(newIndex > charactersToDisplay.Length-1) {  newIndex = 0; } 
        ChangeChar(newIndex);
    }


    public void PreviousChar()
    {
        int newIndex = oldIndex-1;
        if(newIndex < 0)
        {
            newIndex = charactersToDisplay.Length - 1;
        }

        ChangeChar(newIndex);
    }


    public void SelectChar()
    {
        PlayerPrefs.SetInt("PlayerSelected", 0);
        gameManager.SetSelectedPlayer(oldIndex);

        //ToDo: Change this to not level, and check if the player has chosen a character before or not
        if (GameManager.Instance.actualLevel == 1)
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
