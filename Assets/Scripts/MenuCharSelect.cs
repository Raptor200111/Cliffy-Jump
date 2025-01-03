using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

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
        
        ChangeChar(oldIndex);
    }

    private void ChangeChar(int newIndex) 
    {
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
        gameManager.SetSelectedPlayer(oldIndex);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
