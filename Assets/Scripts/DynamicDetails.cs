using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.XR;
using static DynamicDetails;
using static DynamicStructures;
using static UnityEngine.Rendering.HighDefinition.ScalableSettingLevelParameter;
using static WorldManager;


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
    public enum MovDecoType
    {
        Birds = 0,
        Lizard = 1,
        Eye = 2
    }

    [System.Serializable]
    public class DetailType
    {
        public MovDecoType movDecoType;
        public GameObject movDecoPrefab;
    }

    public DetailType[] detailTypes;
    public int[] indexObstacles;
    private GameObject birdPrefab;
    private GameObject lizardPrefab;
    private GameObject eyePrefab;

    struct PosInfo
    {
        public Pos pos;
        public bool isValid;
        public List<Pos> neighbours;
    }
    private List<PosInfo>[] allBlocks;
    private int numValidBlocks;
    private GameObject block;
    private int level;
    List<int> ocupiedBlocks = new List<int>();

    private GameObject movDecoGO;

    struct NumConstrains { public int max, min; };

    public float skyHeight = 10f;
    private NumConstrains birdMaxMin = new NumConstrains { max = 5, min = 2 };
    private int maxBirdCount = 5;

    private float cubeSize = 2f;
    private float posY = 5.5f;
    struct PosRot
    {
        public Vector3 pos;
        public float ydegrees;
    }

    private NumConstrains lizardMaxMin = new NumConstrains { max = 6, min = 4 };
    private float yLizardStart = -5f;
    private float yLizardEndMargin = -2.2f;

    private NumConstrains eyeMaxMin = new NumConstrains { max = 4, min = 2 };
    private float yEyeStart = -7f;
    private float yEyeEndMargin = 2f;

    private GameManager gameManager;
    private GameObject coinStarsGO;
    [SerializeField] private int numStars = 3; 
    [SerializeField] private int numCoins = 3;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;

        if (gameManager == null)
        {
            Debug.LogError("gameManager not assigned!");
            return;
        }

        if( detailTypes[0].movDecoPrefab == null || detailTypes[1].movDecoPrefab == null)
        {
            Debug.LogError("prefab not assigned!");
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

        GetTopLevelBaseBlocks();
    }


    private void GetTopLevelBaseBlocks()
    {
        try
        {
            worldData = JsonUtility.FromJson<World>(jsonFile.text);
            allBlocks = new List<PosInfo>[worldData.levels.Count];
            numValidBlocks = 0;

            for (int i = 0; i < worldData.levels.Count; ++i)
            {
                List<PosInfo> levelTopBlocks = new List<PosInfo>();
                List<Pos> positions = worldData.levels[i].positions;

                // Create a dictionary for quick neighbor lookup
                Dictionary<Vector3, PosInfo> positionMap = new Dictionary<Vector3, PosInfo>();

                foreach (Pos position in positions)
                {
                    PosInfo posInfo = new PosInfo
                    {
                        pos = position,
                        isValid = true,
                        neighbours = new List<Pos>()
                    };

                    // Check if this position has a base block (index 0)
                    bool hasBaseBlock = false;
                    bool hasOtherBlocks = false;

                    foreach (ObjectInfo obj in position.objects)
                    {
                        if (!indexObstacles.Contains(obj.index))
                        {
                            hasBaseBlock = true;
                        }
                        else
                        {
                            hasOtherBlocks = true;
                        }
                    }

                    if (hasBaseBlock)
                    {
                        if (hasOtherBlocks || position.y != 0)
                        {
                            posInfo.isValid = false;
                        }
                        else
                        {
                            numValidBlocks += 1;
                        }

                        levelTopBlocks.Add(posInfo);
                        positionMap.Add(position.ToVec3(), posInfo);
                    }
                }

                // Compute neighbors
                foreach (var posInfo in levelTopBlocks)
                {
                    Vector3 posVec = posInfo.pos.ToVec3();

                    foreach (Vector3 offset in new Vector3[]
                    {
                    new Vector3(-2, 0, 0), new Vector3(2, 0, 0),
                    new Vector3(0, 0, -2), new Vector3(0, 0, 2),
                    new Vector3(-1, 0, -1), new Vector3(1, 0, 1),
                    new Vector3(-1, 0, 1), new Vector3(1, 0, -1)
                    })
                    {
                        Vector3 neighborVec = posVec + offset;

                        if (positionMap.TryGetValue(neighborVec, out PosInfo neighborPosInfo))
                        {
                            posInfo.neighbours.Add(neighborPosInfo.pos);
                        }
                    }
                    
                }

                allBlocks[i] = levelTopBlocks;
            }
        }
        catch
        {
            UnityEngine.Debug.Log("Json not attached or doesn't exist");
        }
    }


    // Update is called once per frame
    void Update()
    {
    }

    private Vector3 CalcTopBlock(int index)
    {
        Pos posBlock = allBlocks[level][index].pos;
        Vector3 top = new Vector3(posBlock.x, posBlock.y, posBlock.z) + new Vector3(10, posY, -10); // Top position of this collider
        return top;
    }

    private int CalcMaxMovDeco(NumConstrains maxmin)
    {
        int MaxNumDeco = (numValidBlocks - 2) - (numCoins + numStars);
        int MinNumDeco = maxmin.min;
        if(MaxNumDeco > maxmin.max) {
            MaxNumDeco = maxmin.max;
        }
        else if(MaxNumDeco > maxmin.min) {
            MinNumDeco = 1;
        }
        return UnityEngine.Random.Range(MinNumDeco, MaxNumDeco);
    }

    private int SelectBlock()
    {

        int indexBlock = UnityEngine.Random.Range(1, allBlocks[level].Count - 1);
        bool isValidBlock = allBlocks[level][indexBlock].isValid && allBlocks[level][indexBlock].neighbours.Count < 4;
        while (!isValidBlock || ocupiedBlocks.Contains(indexBlock))
        {
            indexBlock = UnityEngine.Random.Range(1, allBlocks[level].Count - 1);
            isValidBlock = allBlocks[level][indexBlock].isValid && allBlocks[level][indexBlock].neighbours.Count < 4;
        }
        //int indexBlock = UnityEngine.Random.Range(8, 15);
        ocupiedBlocks.Add(indexBlock);
        return indexBlock;
    }

    public void CreateDetails(int level_screen)
    {
        if (detailTypes == null || worldData == null || allBlocks == null)
        {
            Start();
        }
        
        if (level_screen < 0) level_screen = 0;
        else if (level_screen > worldData.levels.Count - 1) level_screen = worldData.levels.Count - 1;
        level = level_screen;
        ocupiedBlocks = new List<int>();
        coinStarsGO = new GameObject();
        movDecoGO = new GameObject();
        coinStarsGO.name = "coinStars";
        movDecoGO.name = "movDeco";


        CreateMovDeco(detailTypes[0]);

        if (level_screen> 7)
        {
            CreateMovDeco(detailTypes[1]);
        }
        
        CreateCoinStar(coinStarsGO);
    }

    public void CreateMovDeco(DetailType detailType)
    {
        if (detailType.movDecoType == MovDecoType.Birds && detailType.movDecoPrefab != null)
        {
            
            int birdCount = UnityEngine.Random.Range(1, maxBirdCount);
            birdPrefab = detailType.movDecoPrefab;
            int groupCount = CalcMaxMovDeco(birdMaxMin);
            CreateBirdGroups(movDecoGO, groupCount, birdCount);
        }
        else if (detailType.movDecoType == MovDecoType.Lizard && detailType.movDecoPrefab != null)

        {
            lizardPrefab = detailType.movDecoPrefab;
            int lizardCount = CalcMaxMovDeco(lizardMaxMin);
            CreateLizards(movDecoGO, lizardCount);
        }
        else if (detailType.movDecoType == MovDecoType.Eye && detailType.movDecoPrefab != null)
        {
            eyePrefab = detailType.movDecoPrefab;
            int eyeCount = CalcMaxMovDeco(eyeMaxMin);
            CreateEyes(movDecoGO, eyeCount);
        }
    }

    public void DestroyDetails()
    {
        if (coinStarsGO != null && coinStarsGO.GetComponentsInChildren<Collectible>() != null)
        {
            Collectible[] starcoins = coinStarsGO.GetComponentsInChildren<Collectible>();

            foreach (Collectible script in starcoins)
            {
                script.Disappear();
            }
        }
        if (movDecoGO != null && movDecoGO.GetComponentsInChildren<MovDeco>() != null)
        {
            MovDeco[] movDecoScripts = movDecoGO.GetComponentsInChildren<MovDeco>();
            foreach (MovDeco movDeco in movDecoScripts)
            {
                movDeco.Disappear();
            }
        }

    }

    private void CreateCoinStar(GameObject paretnGO)
    {
        for (int i = 0; i < numStars; i++)
        {
            int indexBlock = SelectBlock();
            Vector3 starPos = CalcTopBlock(indexBlock) + new Vector3(0f, 1, 0f);
            GameObject star = Instantiate(WorldManager.Instance.StarPrefab, starPos, Quaternion.identity, paretnGO.transform);
        }
        for (int j = 0; j < numCoins; j++)
        {
            int indexBlock = SelectBlock();
            GameObject coin = Instantiate(WorldManager.Instance.CoinPrefab, CalcTopBlock(indexBlock), Quaternion.identity, paretnGO.transform);
        }

    }

    private float calcDegrees(Vector3 block, Vector3 blockSide)
    {
        Vector3 direction =  block - blockSide; //returns what i cannot        
        return Mathf.Atan2(direction.x, -direction.z) * Mathf.Rad2Deg;
    }

    private float calcDegrees2(Vector3 block, Vector3 blockSide)
    {
        Vector3 direction = block - blockSide; //returns what i cannot        
        return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    }

    private PosRot calcStartPos(int indexBlock, float scale)
    {
        List<float> possibleDegrees = new List<float>() { 0f, 90f , 180f, -90f };
        Vector3 blockTop = CalcTopBlock(indexBlock);
        foreach(Pos neighbour in allBlocks[level][indexBlock].neighbours)
        {
            Vector3 topNeighbour = neighbour.ToVec3() + new Vector3(10, posY, -10);
            possibleDegrees.Remove(calcDegrees(blockTop, topNeighbour));
        }



        //if lizard prefer 0 and -90;
        int selected = UnityEngine.Random.Range(0, possibleDegrees.Count);
        float selectedDegree = possibleDegrees[selected];
        if(selectedDegree == 90 || selectedDegree == -90) { selectedDegree *= -1f; }

        float x = Mathf.Sin(selectedDegree * Mathf.Deg2Rad);
        float z = Mathf.Cos(selectedDegree * Mathf.Deg2Rad);       

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
            GameObject lizard = Instantiate(lizardPrefab, posStart, rotation, parentGO.transform);
            float yoffset = lizardTopBlock.pos.y + yLizardEndMargin - (i % 4) * 0.5f;
            Vector3 targetPos = new Vector3(lizardTopBlock.pos.x, yoffset, lizardTopBlock.pos.z);
            CallMovDecoAppear(lizard, targetPos);

        }
    }

    private void CreateEyes(GameObject parentGO, int eyeCount)
    {
        for(int j = 0; j < eyeCount; j++)
        {
            int indexBlock = SelectBlock();

            PosRot eyeTopBlock = calcStartPos(indexBlock, cubeSize * (2f));
            Vector3 eyeStartPos = eyeTopBlock.pos + new Vector3(0, yEyeStart, 0f); 
            GameObject eye = Instantiate(eyePrefab, eyeStartPos, Quaternion.identity, parentGO.transform);
            float yoffset = eyeTopBlock.pos.y + yEyeEndMargin - (j % 4) * 0.5f;
            Vector3 targetPos = new Vector3(eyeTopBlock.pos.x, yoffset, eyeTopBlock.pos.z);
            CallMovDecoAppear(eye, targetPos);
        }
    }


    private void CreateBirdGroups(GameObject parentGO, int groupCount, int birdCount)
    {
        
        for (int i = 0; i < groupCount; i++)
        {
            int indexCube = SelectBlock();
            Vector3 cubePos = CalcTopBlock(indexCube);
            birdCount = 1;
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
                Vector3 sincosb = CalcBirdOffset(birdCount, j);
                Vector3 targetPosition = cubePos + sincosb;
                CallMovDecoAppear(bird, targetPosition);                
            }
        }
    }
    private void CallMovDecoAppear(GameObject gameObjectInstance, Vector3 targetPosition)
    {
        MovDeco movDecoScript = gameObjectInstance.GetComponent<MovDeco>();
        if(movDecoScript == null)
        {
            Debug.LogError("MovDeco missing script");
            return;
        }
        movDecoScript.Appear(targetPosition);

    }

    private Vector3 CalcBirdOffset(int birdCount, int index)
    {
        if(birdCount == 1) { return Vector3.zero; }
        float angle = 2f * Mathf.PI / (float)birdCount * index;
        float radius = 1f;
        return new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
    }
}