using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnGoToMenu()
    {
        GameManager.Instance.changeScene(StageName.MENU);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
