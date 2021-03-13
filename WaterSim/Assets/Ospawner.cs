using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ospawner : MonoBehaviour
{

    public GameObject OceanObj;


    public float squareWidth = 800f;
    public float innerSquareResolution = 5f;
    public float outerSquareResolution = 25f;
    
    //rudimentary OceanSpawner

    void Start()
    {
        GameObject grid = Instantiate(OceanObj, transform.position, transform.rotation) as GameObject;
        grid.SetActive(true);
        Vector3 centerPos = transform.position;

        grid.transform.position = centerPos;
        grid.transform.parent = transform;

        OceanMeshGen newOceanMesh = new OceanMeshGen(grid, squareWidth, squareWidth);
    }
}
