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
using static UnityEngine.Rendering.DebugUI.Table;

public class DynamicStructures : MonoBehaviour
{
    Vector3[,] positionsArray;
    Animator animator;

    GameObject[][] allObjects;

    int numberOfScreens = 3;
    public int screen { get; private set; } = 0;

    public List<GameObject> prefabs;
    public TextAsset jsonFile;


    [System.Serializable]
    public class World
    {
        public List<Level> levels;
        public List<int> allObjects;
    }
    [System.Serializable]
    public class Level
    {
        public List<Pos> positions;
    }
    
    [System.Serializable]
    public class ObjectInfo
    {
        public int YOffset;
        public int index;
        public int orientation;
    }
    [System.Serializable]
    public class Pos
    {
        public float x;
        public float y;
        public float z;
        public List<ObjectInfo> objects;

        public Vector3 ToVec3()
        {
            return new Vector3(x, y, z);
        }
        public Vector3 ToVec3(float YOffset)
        {
            return new Vector3(x, y + YOffset, z);
        }
    }

    public World world = new World();


    void Start()
    {
        animator = GetComponent<Animator>();
        try
        {
            world = JsonUtility.FromJson<World>(jsonFile.text);

            numberOfScreens = world.levels.Count;

            allObjects = new GameObject[world.allObjects.Count][];
            for (int i = 0; i < allObjects.Length; i++)
            {
                allObjects[i] = new GameObject[world.allObjects[i]];
                for (int j = 0; j < allObjects[i].Length; j++)
                {
                    allObjects[i][j] = Instantiate(prefabs[i], Vector3.zero, Quaternion.identity, this.transform);
                }
            }

            screen -= 1;
            HiddenObjectsChange();
        }
        catch 
        {
            UnityEngine.Debug.Log("Json not attached or doesnt exist");
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            NextScreen();
        }
    }

    public void NextScreen()
    {
        animator.SetTrigger("hide");
    }

    public void HiddenObjectsChange()
    {
        screen++;

        if (screen == numberOfScreens)
        {
            WorldManager.Instance.WorldComplete();
            return;
        }

        

        int [] indexes = new int[world.allObjects.Count];
        for (int i = 0; i < indexes.Length; i++)
        {
            indexes[i] = 0;
        }

        for (int i = 0; i < world.levels[screen].positions.Count; i++)
        {
            for (int j = 0; j < world.levels[screen].positions[i].objects.Count; j++)
            {
                ObjectInfo obj = world.levels[screen].positions[i].objects[j];
                Quaternion quat = Quaternion.Euler(0, obj.orientation, 0);
                allObjects[obj.index][indexes[obj.index]++].transform.SetLocalPositionAndRotation(world.levels[screen].positions[i].ToVec3(obj.YOffset), quat);
            }
        }

        Vector3 resetPos = new Vector3(100, 100, 100);
        for (int i = 0; i < allObjects.Length; i++)
        {
            for (int j = indexes[i]; j < allObjects[i].Length; j++)
            {
                allObjects[i][j].transform.localPosition = resetPos;
            }
        }

        animator.SetTrigger("rise");
    }

    public void DoneRising()
    {
        WorldManager.Instance.DoneRising();
    }
    
    public GameObject[] GetAllBlocks() { return allObjects[0]; }
}

/*
    "block": 0,
    "turnBlock": 1,
    "crossBlock": 2,
    "damagedBlock" 3,
    "turn": 4,
    "groundSpike": 5,
    "halfSpike": 6,
    "fullSpike": 7,
    "hammer": 8,
    "stop": 9
*/