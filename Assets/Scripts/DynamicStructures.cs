using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.Timeline;
using UnityEngine.UIElements;
using static DynamicStructures;

public class DynamicStructures : MonoBehaviour
{
    Vector3[,] positionsArray;
    Animator animator;
    GameObject[] allChildren;
    int numberOfChildren = 17;
    int numberOfScreens = 3;
    int screen = 0;

    public List<GameObject> prefabs;
    public TextAsset jsonFile;

    [System.Serializable]
    public class World
    {
        public List<Level> levels;
    }
    [System.Serializable]
    public class Level
    {
        public List<Block> blocks;
    }
    [System.Serializable]
    public class Block
    {
        public int gameObjType;
        public Pos pos;
    }
    [System.Serializable]
    public class Pos
    {
        public float x;
        public float y;
        public float z;
    }
    
    public World world = new World();


    void Start()
    {
        animator = GetComponent<Animator>();
        try
        {
            world = JsonUtility.FromJson<World>(jsonFile.text);

            numberOfScreens = world.levels.Count;
            numberOfChildren = world.levels[screen].blocks.Count;

            allChildren = new GameObject[numberOfChildren];
            Vector3 pos000 = new Vector3(0, 0, 0);
            for (int i = 0; i < numberOfChildren; i++)
            {
                allChildren[i] = Instantiate(prefabs[world.levels[screen].blocks[i].gameObjType], pos000, Quaternion.identity, this.transform);
                allChildren[i].transform.localPosition = new Vector3(world.levels[screen].blocks[i].pos.x, world.levels[screen].blocks[i].pos.y, world.levels[screen].blocks[i].pos.z);
            }

            animator.SetTrigger("rise");
        }
        catch 
        {
            UnityEngine.Debug.Log("Json not attached or doesnt exist");
        }
    }

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
            allChildren[i].transform.localPosition = new Vector3(world.levels[screen].blocks[i].pos.x, world.levels[screen].blocks[i].pos.y, world.levels[screen].blocks[i].pos.z);
        }
        animator.SetTrigger("rise");
    }
}
