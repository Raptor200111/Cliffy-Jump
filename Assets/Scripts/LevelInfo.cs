using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DynamicDetails;

[CreateAssetMenu(fileName = "LevelInfo", menuName = "LevelInfo")]
public class WorldInfo : ScriptableObject
{
    public TextAsset jsonFile;
    public List<GameObject> prefabs;
    public DetailType[] detailTypes;
    public int[] indexObstacles;
    public GameObject Player; 

}
