using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static DynamicStructures;


/*
 bird:
public float skyHeight = 10f;
private int maxGroupCount = 4;
private int maxBirdCount = 5;
CalcBirdOffset
//apears on top

lizard:
public flost maxy = 5f - 2.5f;
appear 1
//appears on side

eyes:
maxy 

 */


public class DynamicBirds : MonoBehaviour
{
    [SerializeField] GameObject birdPrefab;
    List<int> ocupiedBlocks = new List<int>();

    [SerializeField] private int numStars = 3; 
    [SerializeField] private int numCoins = 3;
    private GameManager gameManager;
    private List<Vector3> circuitCubes = new List<Vector3>();
    public float skyHeight = 10f;
    private int maxGroupCount = 4;
    private int maxBirdCount = 5;


    private bool is_ok;
    private GameObject coinStarsGO;
    private GameObject movDecoGO;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        if (birdPrefab == null || gameManager == null)
        {
            Debug.LogError("Bird prefab or circuit not assigned!");
            return;
        }
        numCoins = 3;
        numStars = 3;
    }
    // Update is called once per frame
    void Update()
    {  

    }

    public void CreateDetails(GameObject[] blocks, StageName lvl)
    {
        for (int i = 0; i < blocks.Count(); i++)
        {
            if (blocks[i].transform.position.x < 90)
            {
                Collider col = blocks[i].GetComponent<Collider>();
                Bounds bounds = col.bounds; // Get the bounds in world space
                Vector3 top = bounds.center + new Vector3(0, bounds.extents.y, 0); // Top position of this collider
                circuitCubes.Add(top);
            }
        }

        if (circuitCubes.Count != 0)
        {
            if (birdPrefab != null)// && gameManager.stageName == StageName.LVL_1)
            {
                if(lvl == StageName.LVL_1)
                {
                    int groupCount = UnityEngine.Random.Range(1, maxGroupCount); // Generate 1 to 3 groups
                    int birdCount = UnityEngine.Random.Range(1, maxBirdCount);
                    is_ok = circuitCubes.Count > (numCoins + numStars + groupCount);
                    coinStarsGO = new GameObject();
                    CreateCoinStar(coinStarsGO);
                    movDecoGO = new GameObject();
                    CreateBirdGroups(movDecoGO, groupCount, birdCount);
                }
            }
        }
    }

    public void DestroyDetails()
    {
        AnimationScript[] starcoins = coinStarsGO.GetComponentsInChildren<AnimationScript>();
        foreach (AnimationScript script in starcoins)
        {
            script.DestroyAnimation();
        }
        Bird[] birdScripts = movDecoGO.GetComponentsInChildren<Bird>();
        foreach (Bird birdscript in birdScripts)
        {
            birdscript.FlyAway();
        }
    }

    private int SelectBlock()
    {
        int indexBlock = UnityEngine.Random.Range(0, circuitCubes.Count);
        if(is_ok)
        {
            while (ocupiedBlocks.Contains(indexBlock))
            {
                indexBlock = UnityEngine.Random.Range(0, circuitCubes.Count);

            }
        }

        ocupiedBlocks.Add(indexBlock);
        return indexBlock;
    }

    private void CreateCoinStar(GameObject paretnGO)
    {
        for (int i = 0; i < numStars; i++)
        {
            int indexBlock = SelectBlock();
            Vector3 starPos = circuitCubes[indexBlock] + new Vector3(0f, 1, 0f);
            GameObject star = Instantiate(gameManager.GetStarPrefab(), starPos, Quaternion.identity, paretnGO.transform);
        }
        for (int j = 0; j < numCoins; j++)
        {
            int indexBlock = SelectBlock();
            GameObject coin = Instantiate(gameManager.GetCoinPrefab(), circuitCubes[indexBlock], Quaternion.identity, paretnGO.transform);
        }

    }

    private void CreateBirdGroups(GameObject paretnGO, int groupCount, int birdCount)
    {

        for (int i = 0; i < groupCount; i++)
        {
            int indexCube = SelectBlock();
            Vector3 cubePos = circuitCubes[indexCube];

            for (int j = 0; j < birdCount; j++)
            {
                // Spawn the bird at a random sky position
                Vector3 skyPosition = new Vector3(
                    cubePos.x + UnityEngine.Random.Range(-2f, 2f),
                    skyHeight,
                    cubePos.z + UnityEngine.Random.Range(-2f, 2f)
                );
                Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0f, 355f), 0);
                GameObject bird = Instantiate(birdPrefab, skyPosition, rotation, paretnGO.transform);
                Bird birdScript = bird.GetComponent<Bird>();

                if (birdScript != null)
                {
                    Vector3 sincosb = CalcBirdOffset(bird, birdCount, j);
                    Vector3 targetPosition = cubePos + sincosb;

                    birdScript.SetLandingPosition(targetPosition);

                    Debug.Log($"cubePos: {cubePos}");
                    Debug.Log($"sincosb: {sincosb}, birdCount: {birdCount}, index: {j}");
                    Debug.Log($"targetPosition: {targetPosition}, id: {i}{j}");
                }
                else
                {
                    Debug.LogError("Bird prefab does not have a BirdScript component!");
                }
            }
        }
    }

    private Vector3 CalcBirdOffset(GameObject bird, int birdCount, int index)
    {
        if(birdCount == 1) { return Vector3.zero; }
        float angle = 2f * Mathf.PI / (float)birdCount * index;
        float radius = 1f;
        return new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
    }
}