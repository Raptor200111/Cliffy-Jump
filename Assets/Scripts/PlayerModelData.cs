using UnityEngine;

[CreateAssetMenu(fileName = "PlayerModelData", menuName = "Player/Model Data")]
public class PlayerModelData : ScriptableObject
{
    public string modelName;
    public GameObject modelPrefab;  // prefab playerVoxel
    public RuntimeAnimatorController animatorController; // playerVoxel animator
}

