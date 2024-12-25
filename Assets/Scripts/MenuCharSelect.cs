using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuCharSelect : MonoBehaviour
{
    private int index;
    [SerializeField] private GameObject charImage;
    [SerializeField] private TMP_Text charName;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        index = PlayerPrefs.GetInt("CharIndex");
        if (index > gameManager.characters.Count -1)
        {
            index = 0;
        }
        ChangeChar();
    }

    private void ChangeChar() 
    {
        PlayerPrefs.GetInt("CharIndex", index);

        GameObject newPrefab = gameManager.characters[index].imageDisplay;
        charName.text = gameManager.characters[index].nameDisplay;

        foreach (Transform child in charImage.transform)
        {
            Destroy(child.gameObject);
        }

        // Instantiate the new prefab as a child of imageObject
        GameObject newInstance = Instantiate(newPrefab, charImage.transform);

        // Optionally, reset the position, rotation, and scale of the new instance
        newInstance.transform.localPosition = Vector3.zero;
        newInstance.transform.localRotation = Quaternion.identity;
        newInstance.transform.localScale = new Vector3(10, 10, 10);

    }

    public void NextChar() 
    {
        if (index == gameManager.characters.Count - 1)
        {
            index = 0;
        }
        else
        {
            index++;
        }
        ChangeChar();
    }


    public void PreviousChar()
    {
        if (index == 0)
        {
            index = gameManager.characters.Count - 1;
        }
        else
        {
            index--;
        }
        ChangeChar();
    }


    public void SelectChar()
    {
        gameManager.player = gameManager.characters[index];
        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
