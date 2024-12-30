using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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


public class DynamicDetails : MonoBehaviour
{
    private List<Vector3> circuitCubes = new List<Vector3>();
    private bool is_ok;
    List<int> ocupiedBlocks = new List<int>();

    private GameObject movDecoGO;

    [SerializeField] GameObject birdPrefab;
    public float skyHeight = 10f;
    private int maxGroupCount = 4;
    private int maxBirdCount = 5;

    private float cubeSize = 2f;
    struct PosRot
    {
        public Vector3 pos;
        public float ydegrees;
    }

    [SerializeField] GameObject lizardPrefab;
    private int maxLizardCount = 9;
    private int minLizardCount = 4;
    private float yLizardStart = -4f;
    private float yLizardEndMargin = -2.2f;

    [SerializeField] GameObject eyePrefab;
    private float yEyeStart = -4f;
    private int maxEyeCount = 8;
    private float yEyeEndMargin = 3f;

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
            else if(lvl == 3 && eyePrefab != null)
            {
                int eyeCount = UnityEngine.Random.Range(minLizardCount, maxEyeCount);
                is_ok = circuitCubes.Count > (numCoins + numStars + eyeCount);
                CreateCoinStar(coinStarsGO);
                CreateEyes(movDecoGO, eyeCount);
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
            BirdController[] birdScripts = movDecoGO.GetComponentsInChildren<BirdController>();
            foreach (BirdController birdscript in birdScripts)
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
        else if (lvl == 3)
        {
            EyeController[] eyeScripts = movDecoGO.GetComponentsInChildren<EyeController>();
            foreach (EyeController eyeScript in eyeScripts) {  eyeScript.DisappearAnim(); }
        }
    }

    private int SelectBlock()
    {
        int indexBlock = UnityEngine.Random.Range(1, circuitCubes.Count-1);
        if(is_ok)
        {
            while (ocupiedBlocks.Contains(indexBlock))
            {
                indexBlock = UnityEngine.Random.Range(1, circuitCubes.Count-1);

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

    private float calcDegrees(Vector3 block, Vector3 blockSide)
    {
        Vector3 direction =  block - blockSide; //returns what i cannot        
        return Mathf.Atan2(direction.x, -direction.z) * Mathf.Rad2Deg;
    }

    private PosRot calcStartPos(int indexBlock, float y, float scale)
    {
        List<float> possibleDegrees = new List<float>() { 0f, 90f , 180f, -90f };
        possibleDegrees.Remove(calcDegrees(circuitCubes[indexBlock], circuitCubes[indexBlock - 1]));
        possibleDegrees.Remove(calcDegrees(circuitCubes[indexBlock], circuitCubes[indexBlock + 1]));

        //if lizard prefer 0 and -90;
        int selected = UnityEngine.Random.Range(0, possibleDegrees.Count);
        float selectedDegree = possibleDegrees[selected];

        // Convert degrees to position
        float x = Mathf.Sin(selectedDegree * Mathf.Deg2Rad);
        float z = Mathf.Cos(selectedDegree * Mathf.Deg2Rad);

        selectedDegree = (selectedDegree == 0f || selectedDegree == 180f)? selectedDegree+ 180f : selectedDegree;
        x = (selectedDegree == 90f || selectedDegree == -90f)? -1*x: x;

        return new PosRot
        {
            pos = circuitCubes[indexBlock] +  new Vector3(x * scale, y, z * scale),
            ydegrees = selectedDegree
        };
    }

    private void CreateLizards(GameObject parentGO, int lizardCount)
    {
        for(int i = 0;i < lizardCount;i++)
        {
            int indexBlock = SelectBlock();            
            PosRot lizardStart = calcStartPos(indexBlock, yLizardStart, cubeSize / 2f);
            Quaternion rotation = Quaternion.Euler(0, lizardStart.ydegrees, 0);
            Debug.Log("pos: " + lizardStart.pos + ";  degrees: " + lizardStart.ydegrees);
            GameObject lizard = Instantiate(lizardPrefab, lizardStart.pos, rotation, parentGO.transform);
            float yoffset = circuitCubes[indexBlock].y + yLizardEndMargin - (i % 4) * 0.5f;
            Vector3 targetPos = new Vector3(lizardStart.pos.x, yoffset, lizardStart.pos.z);
            lizard.GetComponent<LizardAnimation>().AppearAnim(targetPos);
        }
    }

    private void CreateEyes(GameObject parentGO, int eyeCount)
    {
        for(int i = 0; i < eyeCount; i++)
        {
            int indexBlock = SelectBlock();

            PosRot eyeStart = calcStartPos(indexBlock, yEyeStart, cubeSize * 3 / 2f);
            GameObject eye = Instantiate(eyePrefab, eyeStart.pos, Quaternion.identity, parentGO.transform);
            float yoffset = circuitCubes[indexBlock].y + yEyeStart - (i % 4) * 0.5f;
            Vector3 targetPos = new Vector3(eyeStart.pos.x, yoffset, eyeStart.pos.z);
            eye.GetComponent<EyeController>().AppearAnim(targetPos);
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
                BirdController birdScript = bird.GetComponent<BirdController>();

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