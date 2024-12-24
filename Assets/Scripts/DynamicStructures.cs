using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

public class DynamicStructures : MonoBehaviour
{
    public GameObject prefab;
    Vector3[,] positionsArray;
    Animator animator;
    GameObject[] allChildren;
    const int numberOfChildren = 17;
    const int numberOfScreens = 3;
    int screen = 0;

    // Start is called before the first frame update
    void Start()
    {
        allChildren = new GameObject[numberOfChildren];

        positionsArray = new Vector3[numberOfScreens, numberOfChildren] {
            { new Vector3(-12, 0, -4), new Vector3(-10, 0, -4), new Vector3(-8, 0, -4), new Vector3(-6, 0, -4), new Vector3(-4, 0, -4), new Vector3(-2, 0, -4), new Vector3(0, 0, -4), new Vector3(2, 0, -4), new Vector3(4, 0, -4), new Vector3(6, 0, -4), new Vector3(6, 0, -2), new Vector3(6, 0, 0), new Vector3(6, 0, 2), new Vector3(6, 0, 4), new Vector3(8, 0, 4), new Vector3(10, 0, 4), new Vector3(12, 0, 4) },
            { new Vector3(-12, 0, -4), new Vector3(-12, 0, -2), new Vector3(-12, 0, 0), new Vector3(-12, 0, 2), new Vector3(-12, 0, 4), new Vector3(-10, 0, 4), new Vector3(-8, 0, 4), new Vector3(-6, -2, 4), new Vector3(-4, 0, 4), new Vector3(-2, 0, 4), new Vector3(0, 0, 4), new Vector3(2, -2, 4), new Vector3(4, 0, 4), new Vector3(6, 0, 4), new Vector3(8, 0, 4), new Vector3(10, -2, 4), new Vector3(12, 0, 4) },
            { new Vector3(0, 0, 0), new Vector3(2, 0, 0), new Vector3(2, 0, 2), new Vector3(-2, 0, 0), new Vector3(-2, 0, -2), new Vector3(-2, 0, -4), new Vector3(-2, 0, -6), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0) } };
        
        Vector3 pos000 = new Vector3(0, 0, 0);
        for (int i = 0; i < numberOfChildren; i++)
        {
            allChildren[i] = Instantiate(prefab, pos000, Quaternion.identity, this.transform);
            
            allChildren[i].transform.localPosition = positionsArray[screen, i];
        }

        animator = GetComponent<Animator>();
        animator.SetTrigger("rise");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space)) 
        {
            animator.SetTrigger("hide");
        }
    }

    public void HiddenObjectsChange()
    {
        screen = (screen + 1) % numberOfScreens;
        
        for (int i = 0; i < allChildren.Length; i++)
        {
            allChildren[i].transform.localPosition = positionsArray[screen,i];
        }
        animator.SetTrigger("rise");
    }
}
