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
        oldIndex = PlayerPrefs.GetInt("CharIndex", 0);
        List<Characters> charactersList = gameManager.GetCharacters();
        if (gameManager.GetCharacters() == null)
        {
            Debug.LogWarning("Select Char scene characters is null");
        }
        else if(charactersList.Count == 0)
        {
            Debug.LogWarning("gameManager's characters is empty");
        }        
        else
        {
            charsToDisplay = new GameObject[charactersList.Count];
            for (int i = 0; i< charactersList.Count; i++)
            {
                GameObject chars = charactersList[i].character;
                GameObject newInstance = Instantiate(chars, charImage.transform);

                newInstance.transform.localPosition = Vector3.zero;
                newInstance.transform.localScale *= 15f;
                newInstance.transform.localRotation = Quaternion.Euler(new Vector3(0f,180f,0f));
                newInstance.SetActive(false);
                charsToDisplay[i] = newInstance;
            }
        }
        if (oldIndex > charsToDisplay.Length -1)
        {
            oldIndex = 0;
        }
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
    public void OnExitButton()
    {
        gameManager.changeScene(StageName.MENU);
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
