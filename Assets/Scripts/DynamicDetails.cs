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
    private List<Pos>[] allBlocks;
    private GameObject block;
    private int level;
    private bool is_ok;
    List<int> ocupiedBlocks = new List<int>();

    private GameObject movDecoGO;

    struct NumConstrains { public int max, min; };

    [SerializeField] GameObject birdPrefab;
    public float skyHeight = 10f;
    private NumConstrains birdMaxMin = new NumConstrains { max = 4, min = 1 };
    private int maxBirdCount = 5;

    private float cubeSize = 2f;
    private float posY = 5f;
    struct PosRot
    {
        public Vector3 pos;
        public float ydegrees;
    }

    [SerializeField] GameObject lizardPrefab;
    private NumConstrains lizardMaxMin = new NumConstrains { max = 8, min = 4 };
    private float yLizardStart = -4f;
    private float yLizardEndMargin = -2.2f;

    [SerializeField] GameObject eyePrefab;
    private NumConstrains eyeMaxMin = new NumConstrains { max = 2, min = 6 };
    private float yEyeStart = -8f;
    private float yEyeEndMargin = -2f;

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

        GetTopLevelBaseBlocks();
    }


    private void GetTopLevelBaseBlocks()
    {
        try
        {
            worldData = JsonUtility.FromJson<World>(jsonFile.text);
            allBlocks = new List<Pos>[worldData.levels.Count];

            for( int i = 0; i< worldData.levels.Count; ++i)
            {
                List<Pos> levelTopBlocks = new List<Pos>();

                foreach (Pos pos in worldData.levels[i].positions)
                {
                    // Check if this position has a base block (index 0)
                    bool hasBaseBlock = false;
                    bool hasOtherBlocks = false;

                    foreach (ObjectInfo obj in pos.objects)
                    {
                        if (obj.index == 0)
                        {
                            hasBaseBlock = true;
                        }
                        else
                        {
                            hasOtherBlocks = true;
                            break;
                        }
                    }

                    // If this position has only base blocks (index 0), add it to the result
                    if (hasBaseBlock && !hasOtherBlocks && pos.x< 100)
                    {
                        levelTopBlocks.Add(pos);
                    }
                }

                allBlocks[i]= (levelTopBlocks);
            }
        }
        catch {
            UnityEngine.Debug.Log("Json not attached or doesnt exist");
        }
    }

    // Update is called once per frame
    void Update()
    {  

    }

    private Vector3 CalcTopBlock(int index)
    {
        Pos posBlock = allBlocks[level][index];
        Vector3 top = new Vector3(posBlock.x, posBlock.y, posBlock.z) + new Vector3(10, posY, -10); // Top position of this collider
        return top;
    }

    private int CalcMaxMovDeco(NumConstrains maxmin)
    {
        int MaxNumDeco = allBlocks[level].Count - 2 - (numCoins + numStars);
        int MinNumDeco = maxmin.min;
        if(MaxNumDeco > maxmin.max) {
            MaxNumDeco = maxmin.max;
        }
        else if(MaxNumDeco > maxmin.min) {
            MinNumDeco = 1;
        }
        return UnityEngine.Random.Range(MinNumDeco, MaxNumDeco);
    }

    public void CreateDetails(int level_screen, int movDecoType)
    {        
        if (level_screen < 0) level_screen = 0;
        else if (level_screen > worldData.levels.Count - 1) level_screen = worldData.levels.Count - 1;
        level = level_screen;
        coinStarsGO = new GameObject();
        movDecoGO = new GameObject();

        if (movDecoType == 1 && birdPrefab != null)
        {    
            int groupCount = UnityEngine.Random.Range(1, CalcMaxMovDeco(birdMaxMin)); // Generate 1 to 3 groups
            int birdCount = UnityEngine.Random.Range(1, maxBirdCount);
            CreateCoinStar(coinStarsGO);
            CreateBirdGroups(movDecoGO, groupCount, birdCount);
        }
        else if(movDecoType == 2 && lizardPrefab != null)
        {
            int lizardCount = CalcMaxMovDeco(lizardMaxMin);
            CreateCoinStar(coinStarsGO);
            CreateLizards(movDecoGO, lizardCount);
        }
        else if(movDecoType == 3 && eyePrefab != null)
        {
            int eyeCount = CalcMaxMovDeco(eyeMaxMin);
            CreateCoinStar(coinStarsGO);
            CreateEyes(movDecoGO, eyeCount);
        }   
    }

    public void DestroyDetails(int movDecoType)
    {
        AnimationScript[] starcoins = coinStarsGO.GetComponentsInChildren<AnimationScript>();
        foreach (AnimationScript script in starcoins)
        {
            script.DisappearAnim();
        }
        if (movDecoType == 1)
        {
            BirdController[] birdScripts = movDecoGO.GetComponentsInChildren<BirdController>();
            foreach (BirdController birdscript in birdScripts)
            {
                birdscript.DisappearAnim();
            }
        }
        else if(movDecoType == 2)
        {
            LizardAnimation[] lizardScripts = movDecoGO.GetComponentsInChildren<LizardAnimation>();
            foreach (LizardAnimation lizardScript in lizardScripts)
            {
                lizardScript.DisappearAnim();
            }
        }
        else if (movDecoType == 3)
        {
            EyeController[] eyeScripts = movDecoGO.GetComponentsInChildren<EyeController>();
            foreach (EyeController eyeScript in eyeScripts) {  eyeScript.DisappearAnim(); }
        }
    }

    private int SelectBlock()
    {
        int indexBlock = UnityEngine.Random.Range(1, allBlocks[level].Count-1);
        while (ocupiedBlocks.Contains(indexBlock))
        {
            indexBlock = UnityEngine.Random.Range(1, allBlocks[level].Count-1);

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

        //selectedDegree = (selectedDegree == 0f || selectedDegree == 180f)? selectedDegree+ 180f : selectedDegree;
        //x = (selectedDegree == 90f || selectedDegree == -90f)? -1*x: x;

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
        for(int j = 0; j < eyeCount; j++)
        {
            int indexBlock = SelectBlock();

            PosRot eyeTopBlock = calcStartPos(indexBlock, cubeSize * 3 / 2f);
            Vector3 eyeStartPos = eyeTopBlock.pos + new Vector3(0f, yEyeStart, 0f); 
            GameObject eye = Instantiate(eyePrefab, eyeStartPos, Quaternion.identity, parentGO.transform);
            float yoffset = eyeTopBlock.pos.y + yEyeEndMargin - (j % 4) * 0.5f;
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