using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class EndlessOcean : MonoBehaviour
{
    public static EndlessOcean current;
    public GameObject gameObj;
    public GameObject oceanObj;

    //oceanvariables
    private float squareWidth = 800f;
    private float innerSquareResolution = 5f;
    private float outerSquareResolution = 25f;
    //gridvariables
    public int gridSize = 10;
    public int cellSize = 10;

    List<OceanMeshGen> oceanMeshes = new List<OceanMeshGen>();

    float secondsSinceStart;
    Vector3 boatPos;
    Vector3 oceanPos;
    bool hasThreadUpdatedWater;

    // Start is called before the first frame update
    void Start()
    {
        current = this;
        CreateEndlessSea();
        //Using 
        secondsSinceStart = Time.time;
        ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateWaterWithThreadPooling()));
        StartCoroutine(UpdateWater());
    }

    // Update is called once per frame
    void Update()
    {
        secondsSinceStart = Time.time;
        boatPos = gameObj.transform.position;
    }

    void UpdateWaterNoThread() 
    { 
    
    }
    IEnumerator UpdateWater()
    {

    }
    void UpdateWaterWithThreadPooling()
    {

    }

    void CreateEndlessSea()
    {

    }

    void AddGrid()
    {
        GameObject grid = Instantiate(oceanObject, transform.position, transform.rotation) as GameObject;
    }
}
