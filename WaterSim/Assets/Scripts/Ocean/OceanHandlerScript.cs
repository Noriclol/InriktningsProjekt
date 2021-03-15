using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanHandlerScript : MonoBehaviour
{
    public OceanHandlerScript current;
    public GameObject Ocean;
    public float timeSinceStart;
    Vector3 boatPos;
    Vector3 oceanPos;
    MeshGridGenerator mesh;

    void Start()
    {
        current = this;
        timeSinceStart = Time.time;
        mesh = Ocean.GetComponent<MeshGridGenerator>();
        oceanPos = mesh.centerPos;
    }

    void Update()
    {
        if (WaveHandler.current.isMoving)
        {

            mesh.AnimateMesh(oceanPos, timeSinceStart);
        }
        timeSinceStart = Time.time;
    }
}
