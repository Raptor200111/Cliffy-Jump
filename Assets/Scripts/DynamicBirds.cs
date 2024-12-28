using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;


public class DynamicBirds : MonoBehaviour
{
    [SerializeField] GameObject birdPrefab;
    [SerializeField] GameObject circuit;
    private GameManager gameManager;
    private List<Vector3> circuitCubes = new List<Vector3>();
    private float startGen;
    public float skyHeight = 10f;
    private int maxGroupCount = 4;
    private int maxBirdCount = 5;
    private float birdScale = 0.1f;
    private float timeAppearCube = 1.8f;
    private bool created = false;
 
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        startGen = 0f;
        if (birdPrefab == null || circuit == null || gameManager == null)
        {
            Debug.LogError("Bird prefab or circuit not assigned!");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (startGen < timeAppearCube) {
            startGen += Time.deltaTime;
        }
        else if (startGen <= 10) 
        {

            Collider[] colliders = circuit.GetComponentsInChildren<Collider>();

            
            foreach (Collider col in colliders)
            {
                Bounds bounds = col.bounds; // Get the bounds in world space
                Vector3 top = bounds.center + new Vector3(0, bounds.extents.y, 0); // Top position of this collider
                circuitCubes.Add(top);
            }

            if (circuitCubes.Count == 0)
            {
                Debug.LogError("No cubes found in the circuit!");
                return;
            }
            startGen = 100;
        }
        if (circuitCubes.Count != 0 && !created)
        {
            CreateBirdGroups();
            created = true;
        }
    }

    private void CreateBirdGroups()
    {
        int groupCount = UnityEngine.Random.Range(1, maxGroupCount); // Generate 1 to 3 groups

        List<int> groups = new List<int>(groupCount);
        for (int i = 0; i < groupCount; i++)
        {
            //TO DO: BETTER (no todos seguidos)
            int indexCube = UnityEngine.Random.Range(0, circuitCubes.Count);
            while (groups.Contains(indexCube))
            {
                indexCube = UnityEngine.Random.Range(0, circuitCubes.Count);

            }
            groups.Add(indexCube);
            Vector3 cubePos = circuitCubes[indexCube];
            // Generate a random number of birds for this group
            int birdCount = UnityEngine.Random.Range(1, maxBirdCount); // 1 to 4 birds per group

            for (int j = 0; j < birdCount; j++)
            {
                // Spawn the bird at a random sky position
                Vector3 skyPosition = new Vector3(
                    cubePos.x + UnityEngine.Random.Range(-2f, 2f), // Random offset around the target cube
                    skyHeight,
                    cubePos.z + UnityEngine.Random.Range(-2f, 2f)
                );
                Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0f, 355f), 0);
                GameObject bird = Instantiate(birdPrefab, skyPosition, rotation, this.transform);
                bird.transform.localScale = new Vector3(birdScale, birdScale, birdScale);
                Bird birdScript = bird.GetComponent<Bird>();

                if (birdScript != null)
                {
                    Vector3 sincosb = CalcBirdOffset(bird.GetComponent<BoxCollider>(), birdCount, j);
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

    private Vector3 CalcBirdOffset(BoxCollider birdCollider, int birdCount, int index)
    {
        float playerBottomOffset = birdCollider.center.y / 2f * birdScale; //birdCollider.bounds.extents.y
        if(birdCount == 1) { return new Vector3(0f, playerBottomOffset, 0f); }
        float angle = 2f * Mathf.PI / (float)birdCount * index;
        float radius = 1f;
        return new Vector3(Mathf.Cos(angle) * radius, playerBottomOffset, Mathf.Sin(angle) * radius);
    }
}