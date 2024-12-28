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
    GameObject[] allBlocks;
    GameObject[] allTurns;
    GameObject[] allObstacles;
    //GameObject[] allCollectebles;
    GameObject[] allDecoration;

    int numberOfScreens = 3;
    int screen = 0;

    public List<GameObject> prefabs;
    public TextAsset jsonFile;

    [System.Serializable]
    public class World
    {
        public List<Level> levels;
        public List<int> blockType;
        public List<int> turningType;
        public List<int> obsType;
        public List<int> decType;
    }
    [System.Serializable]
    public class Level
    {
        public List<Pos> blocks;
        public List<RotObject> turnings;
        public List<RotObject> obs;
        public List<RotObject> dec;
    }
    
    [System.Serializable]
    public class RotObject
    {
        public Pos pos;
        public int Yrotation;
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
            
            allBlocks = new GameObject[world.blockType.Count];
            Vector3 pos000 = new Vector3(0, 0, 0);
            for (int i = 0; i < allBlocks.Length; i++)
            {
                allBlocks[i] = Instantiate(prefabs[world.blockType[i]], pos000, Quaternion.identity, this.transform);
            }

            allTurns = new GameObject[world.turningType.Count];
            for (int i = 0; i < allTurns.Length; i++)
            {
                allTurns[i] = Instantiate(prefabs[world.turningType[i]], pos000, Quaternion.identity, this.transform);
            }

            allObstacles = new GameObject[world.obsType.Count];
            for (int i = 0; i < allObstacles.Length; i++)
            {
                allObstacles[i] = Instantiate(prefabs[world.obsType[i]], pos000, Quaternion.identity, this.transform);
            }

            allDecoration = new GameObject[world.decType.Count];
            for (int i = 0; i < allDecoration.Length; i++)
            {
                allDecoration[i] = Instantiate(prefabs[world.decType[i]], pos000, Quaternion.identity, this.transform);
            }

            screen = -1;
            HiddenObjectsChange();
        }
        catch 
        {
            UnityEngine.Debug.Log("Json not attached or doesnt exist");
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.S)) 
        {
            animator.SetTrigger("hide");
        }
    }

    public void HiddenObjectsChange()
    {
        screen = (screen + 1) % numberOfScreens;

        for (int i = 0; i < allBlocks.Length; i++)
        {
            //allBlocks[i].transform.localPosition = new Vector3(world.levels[screen].blocks[i].x, world.levels[screen].blocks[i].y, world.levels[screen].blocks[i].z);
            allBlocks[i].transform.localPosition = PosToVec3(world.levels[screen].blocks[i]);
        }
        
        for (int i = 0; i < allTurns.Length; i++)
        {
            allTurns[i].transform.localPosition = PosToVec3(world.levels[screen].turnings[i].pos);
            allTurns[i].GetComponent<TurnScript>().rotation = world.levels[screen].turnings[i].Yrotation;
        }

        for (int i = 0; i < allObstacles.Length; i++)
        {

            Quaternion quat = Quaternion.Euler(0, world.levels[screen].obs[i].Yrotation, 0);
            allObstacles[i].transform.SetPositionAndRotation(PosToVec3(world.levels[screen].obs[i].pos), quat);

            //allObstacles[i].transform.localPosition = PosToVec3(world.levels[screen].obs[i].pos);
            //allObstacles[i].GetComponent<TurnScript>().rotation = world.levels[screen].obs[i].Yrotation;
        }

        for (int i = 0; i < allDecoration.Length; i++)
        {
            Quaternion quat = Quaternion.Euler(0, world.levels[screen].dec[i].Yrotation, 0);
            allObstacles[i].transform.SetPositionAndRotation(PosToVec3(world.levels[screen].dec[i].pos), quat);

            //allObstacles[i].transform.localPosition = PosToVec3(world.levels[screen].dec[i].pos);
            //allObstacles[i].GetComponent<TurnScript>().rotation = world.levels[screen].dec[i].Yrotation;
        }

        animator.SetTrigger("rise");
    }

    Vector3 PosToVec3(Pos pos)
    {
        return new Vector3(pos.x, pos.y, pos.z);
    }
}
