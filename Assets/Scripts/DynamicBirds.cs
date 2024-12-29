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
animations, fix offset

lizard:
//appear inside??

eyes:
maxy 

 */


public class DynamicBirds : MonoBehaviour
{
    private List<Vector3> circuitCubes = new List<Vector3>();
    private bool is_ok;
    List<int> ocupiedBlocks = new List<int>();

    private GameObject movDecoGO;

    [SerializeField] GameObject birdPrefab;
    public float skyHeight = 10f;
    private int maxGroupCount = 4;
    private int maxBirdCount = 5;


    [SerializeField] GameObject lizardPrefab;
    private int maxLizardCount = 9;
    private int minLizardCount = 4;
    private float yLizardStart = -4f;
    private float yLizardMargin = -2.2f;

    private GameManager gameManager;
    private GameObject coinStarsGO;
    [SerializeField] private int numStars = 3; 
    [SerializeField] private int numCoins = 3;

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

    public void CreateDetails(GameObject[] blocks, int lvl)
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
            
            coinStarsGO = new GameObject();
            movDecoGO = new GameObject();

            if (lvl == 1 && birdPrefab != null)
            {
                int groupCount = UnityEngine.Random.Range(1, maxGroupCount); // Generate 1 to 3 groups
                int birdCount = UnityEngine.Random.Range(1, maxBirdCount);
                is_ok = circuitCubes.Count > (numCoins + numStars + groupCount);
                CreateCoinStar(coinStarsGO);
                CreateBirdGroups(movDecoGO, groupCount, birdCount);
            }
            else if(lvl == 2 && lizardPrefab != null)
            {
                int lizardCount = UnityEngine.Random.Range(minLizardCount, maxLizardCount);
                is_ok = circuitCubes.Count > (numCoins + numStars + lizardCount);
                CreateCoinStar(coinStarsGO);
                CreateLizards(movDecoGO, lizardCount);
            }
            
        }
    }

    public void DestroyDetails(int lvl)
    {
        AnimationScript[] starcoins = coinStarsGO.GetComponentsInChildren<AnimationScript>();
        foreach (AnimationScript script in starcoins)
        {
            script.DisappearAnim();
        }
        if (lvl == 1)
        {
            Bird[] birdScripts = movDecoGO.GetComponentsInChildren<Bird>();
            foreach (Bird birdscript in birdScripts)
            {
                birdscript.DisappearAnim();
            }
        }
        else if(lvl == 2)
        {
            LizardAnimation[] lizardScripts = movDecoGO.GetComponentsInChildren<LizardAnimation>();
            foreach (LizardAnimation lizardScript in lizardScripts)
            {
                lizardScript.DisappearAnim();
            }
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

    private void CreateLizards(GameObject parentGO, int lizardCount)
    {
        for(int i = 0;i < lizardCount;i++)
        {
            int indexBlock = SelectBlock();
            int option = i % 4;//UnityEngine.Random.Range(0, 4);
            Vector3 offset;
            float yRotation;
            float halfCube = 1f;
            switch (option)
            {
                case 0:
                    offset = new Vector3(0f, yLizardStart, -halfCube);
                    yRotation = 0f;
                    break;
                case 1:
                    offset = new Vector3(0f, yLizardStart, halfCube);
                    yRotation = 180f;
                    break;
                case 2:
                    offset = new Vector3(-halfCube, yLizardStart, -0f);
                    yRotation = 90f;
                    break;  
                default:
                    offset = new Vector3(halfCube, yLizardStart, 0f);
                    yRotation = -90f;
                    break;
            }
            Quaternion rotation = Quaternion.Euler(new Vector3(0f, yRotation, 0f));
            Vector3 lizardPos = circuitCubes[indexBlock] + offset;
            GameObject lizard = Instantiate(lizardPrefab, lizardPos, rotation, parentGO.transform);
            float yoffset = circuitCubes[indexBlock].y + yLizardMargin - (i % 4) * 0.5f;
            Vector3 targetPos = new Vector3(lizardPos.x, yoffset, lizardPos.z);
            lizard.GetComponent<LizardAnimation>().AppearAnim(targetPos);
        }
    }


    private void CreateBirdGroups(GameObject parentGO, int groupCount, int birdCount)
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
                GameObject bird = Instantiate(birdPrefab, skyPosition, rotation, parentGO.transform);
                Bird birdScript = bird.GetComponent<Bird>();

                if (birdScript != null)
                {
                    Vector3 sincosb = CalcBirdOffset(bird, birdCount, j);
                    Vector3 targetPosition = cubePos + sincosb;

                    birdScript.AppearAnim(targetPosition);

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