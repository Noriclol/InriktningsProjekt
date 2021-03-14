using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ospawner : MonoBehaviour
{

    public GameObject OceanObj;
    public float cellSize;
    public int gridSize;
    
    //rudimentary OceanSpawner

    void Start()
    {
        GameObject grid = Instantiate(OceanObj, transform.position, transform.rotation) as GameObject;
        grid.SetActive(true);
        Vector3 centerPos = transform.position;

        grid.transform.position = centerPos;
        grid.transform.parent = transform;

        MeshGridGenerator newMesh = new MeshGridGenerator(grid, cellSize, gridSize);
    }
}
