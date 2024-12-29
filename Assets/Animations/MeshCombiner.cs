using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    void Start()
    {
        // Get all mesh filters in children
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        // Combine all meshes
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        // Create a new mesh on the parent object
        var meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.CombineMeshes(combine);

        // Add MeshRenderer if it doesn't exist
        if (!gameObject.GetComponent<MeshRenderer>())
        {
            gameObject.AddComponent<MeshRenderer>();
        }

        // Set the material from the first child
        gameObject.GetComponent<MeshRenderer>().material = meshFilters[1].GetComponent<MeshRenderer>().material;

        // Optionally hide the children
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}