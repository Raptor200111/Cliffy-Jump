using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static DynamicStructures;
using static UnityEngine.Rendering.HighDefinition.ScalableSettingLevelParameter;


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
    private World worldData = new World();
    [SerializeField] private GameObject blockPrefab;
    public TextAsset jsonFile;
    private List<Vector3>[] allBlocks;
    private GameObject block;
    private int level;
    private bool is_ok;
    List<int> ocupiedBlocks = new List<int>();

    private GameObject movDecoGO;

    [SerializeField] GameObject birdPrefab;
    public float skyHeight = 10f;
    private int maxGroupCount = 4;
    private int maxBirdCount = 5;

    private float cubeSize = 2f;
    private float posY = 5f;
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
        Debug.Log("DynamicDetails Start executed");
        gameManager = GameManager.Instance;

        if (birdPrefab == null || gameManager == null)
        {
            Debug.LogError("Bird prefab or circuit not assigned!");
            return;
        }

        if(blockPrefab == null)
        {
            return;
        }        

        if (jsonFile == null || string.IsNullOrEmpty(jsonFile.text))
        {
            Debug.LogError("JSON file is not assigned or empty.");
            return;
        }

        numCoins = 3;
        numStars = 3;
        level = 0;

        GameObject block = Instantiate(blockPrefab, Vector3.zero, Quaternion.identity, this.transform);
        block.SetActive(false);
        cubeSize = block.transform.localScale.x;
        posY = block.transform.localScale.y / 2f;

        try
        {
            Debug.Log($"JSON Content: {jsonFile.text}");
            worldData = JsonUtility.FromJson<World>(jsonFile.text);

            if (worldData == null || worldData.levels == null)
            {
                Debug.LogError("Deserialization failed or levels are null.");
                return;
            }

            Debug.Log($"Processing world with {worldData.levels.Count} levels.");
            allBlocks = new List<Vector3>[worldData.levels.Count];

            for (int i = 0; i < worldData.levels.Count; i++)
            {
                allBlocks[i] = new List<Vector3>(); // Initialize the list

                var usefulBlocks = worldData.levels[i].positions;

                for (int j = 0; j < usefulBlocks.Count; j++)
                {
                    Vector3 position = usefulBlocks[i].ToVec3();
                    allBlocks[i].Add(position);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Dynamic Details: JSON ERROR - {ex.Message}\n{ex.StackTrace}");
        }
    }


    // Update is called once per frame
    void Update()
    {  

    }

    private Vector3 CalcTopBlock(int index)
    {                
        Vector3 top = allBlocks[level][index] + new Vector3(10, posY, -10); // Top position of this collider
        return top;
    }

    public void CreateDetails(int level_screen, int lvl)
    {        
        if (level_screen < 0) level_screen = 0;
        else if (level_screen > worldData.levels.Count - 1) level_screen = worldData.levels.Count - 1;
        level = level_screen;
        coinStarsGO = new GameObject();
        movDecoGO = new GameObject();
        
        if (lvl == 1 && birdPrefab != null)
        {
            int groupCount = UnityEngine.Random.Range(1, maxGroupCount); // Generate 1 to 3 groups
            int birdCount = UnityEngine.Random.Range(1, maxBirdCount);
            is_ok = (allBlocks[level].Count -2) > (numCoins + numStars + groupCount);
            CreateCoinStar(coinStarsGO);
            CreateBirdGroups(movDecoGO, groupCount, birdCount);
        }
        else if(lvl == 2 && lizardPrefab != null)
        {
            int lizardCount = UnityEngine.Random.Range(minLizardCount, maxLizardCount);
            is_ok = (allBlocks[level].Count - 2) > (numCoins + numStars + lizardCount);
            CreateCoinStar(coinStarsGO);
            CreateLizards(movDecoGO, lizardCount);
        }
        else if(lvl == 3 && eyePrefab != null)
        {
            int eyeCount = UnityEngine.Random.Range(minLizardCount, maxEyeCount);
            is_ok = (allBlocks[level].Count - 2) > (numCoins + numStars + eyeCount);
            CreateCoinStar(coinStarsGO);
            CreateEyes(movDecoGO, eyeCount);
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
        int indexBlock = UnityEngine.Random.Range(1, allBlocks[level].Count-1);
        if(is_ok)
        {
            while (ocupiedBlocks.Contains(indexBlock))
            {
                indexBlock = UnityEngine.Random.Range(1, allBlocks[level].Count-1);

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
            Vector3 starPos = CalcTopBlock(indexBlock) + new Vector3(0f, 1, 0f);
            GameObject star = Instantiate(gameManager.GetStarPrefab(), starPos, Quaternion.identity, paretnGO.transform);
        }
        for (int j = 0; j < numCoins; j++)
        {
            int indexBlock = SelectBlock();
            GameObject coin = Instantiate(gameManager.GetCoinPrefab(), CalcTopBlock(indexBlock), Quaternion.identity, paretnGO.transform);
        }

    }

    private float calcDegrees(Vector3 block, Vector3 blockSide)
    {
        Vector3 direction =  block - blockSide; //returns what i cannot        
        return Mathf.Atan2(direction.x, -direction.z) * Mathf.Rad2Deg;
    }

    private PosRot calcStartPos(int indexBlock, float scale)
    {
        List<float> possibleDegrees = new List<float>() { 0f, 90f , 180f, -90f };
        Vector3 blockTop = CalcTopBlock(indexBlock);
        possibleDegrees.Remove(calcDegrees(blockTop, CalcTopBlock(indexBlock-1)));
        possibleDegrees.Remove(calcDegrees(blockTop, CalcTopBlock(indexBlock+1)));

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
            pos = blockTop +  new Vector3(x * scale, 0f, z * scale),
            ydegrees = selectedDegree
        };
    }

    private void CreateLizards(GameObject parentGO, int lizardCount)
    {
        for(int i = 0;i < lizardCount;i++)
        {
            int indexBlock = SelectBlock();            
            PosRot lizardTopBlock = calcStartPos(indexBlock, cubeSize / 2f);
            Vector3 posStart = lizardTopBlock.pos + new Vector3(0f, yLizardStart, 0f);
            Quaternion rotation = Quaternion.Euler(0, lizardTopBlock.ydegrees, 0);
            Debug.Log("pos: " + lizardTopBlock.pos + ";  degrees: " + lizardTopBlock.ydegrees);
            GameObject lizard = Instantiate(lizardPrefab, posStart, rotation, parentGO.transform);
            float yoffset = lizardTopBlock.pos.y + yLizardEndMargin - (i % 4) * 0.5f;
            Vector3 targetPos = new Vector3(lizardTopBlock.pos.x, yoffset, lizardTopBlock.pos.z);
            lizard.GetComponent<LizardAnimation>().AppearAnim(targetPos);
        }
    }

    private void CreateEyes(GameObject parentGO, int eyeCount)
    {
        for(int i = 0; i < eyeCount; i++)
        {
            int indexBlock = SelectBlock();

            PosRot eyeTopBlock = calcStartPos(indexBlock, cubeSize * 3 / 2f);
            Vector3 eyeStartPos = eyeTopBlock.pos + new Vector3(0f, yEyeStart, 0f); 
            GameObject eye = Instantiate(eyePrefab, eyeStartPos, Quaternion.identity, parentGO.transform);
            float yoffset = eyeTopBlock.pos.y + yEyeStart - (i % 4) * 0.5f;
            Vector3 targetPos = new Vector3(eyeTopBlock.pos.x, yoffset, eyeTopBlock.pos.z);
            eye.GetComponent<EyeController>().AppearAnim(targetPos);
        }
    }


    private void CreateBirdGroups(GameObject parentGO, int groupCount, int birdCount)
    {

        for (int i = 0; i < groupCount; i++)
        {
            int indexCube = SelectBlock();
            Vector3 cubePos = CalcTopBlock(indexCube);

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
                    Vector3 sincosb = CalcBirdOffset(birdCount, j);
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

    private Vector3 CalcBirdOffset(int birdCount, int index)
    {
        if(birdCount == 1) { return Vector3.zero; }
        float angle = 2f * Mathf.PI / (float)birdCount * index;
        float radius = 1f;
        return new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
    }
}