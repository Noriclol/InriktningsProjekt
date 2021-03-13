using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class EndlessOcean : MonoBehaviour
{
    public static EndlessOcean current;
    public GameObject shipObj; //for grabbing the position of the boat/ship in the ocean
    public GameObject oceanObj; //the ocean grid object as reference

    //oceanvariables
    private float squareWidth = 800f;
    private float innerSquareResolution = 5f;
    private float outerSquareResolution = 25f;
    //gridvariables

    List<OceanMeshGen> oceanMeshes = new List<OceanMeshGen>();

    float secondsSinceStart;
    Vector3 shipPos;
    Vector3 oceanPos;
    bool hasThreadUpdatedWater;

    //Start is called before the first frame update
    void Start()
    {
        current = this;
        CreateEndlessOcean();
        //Time Init
        secondsSinceStart = Time.time;
        ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateWaterWithThreadPooling));
        StartCoroutine(UpdateWater());
    }

    //Update is called once per frame
    void Update()
    {
        secondsSinceStart = Time.time;
        shipPos = shipObj.transform.position;
    }

    void UpdateWaterNoThread()
    {
        shipPos = shipObj.transform.position;
        MoveWaterToShip();
        transform.position = oceanPos;
        for (int i = 0; i < oceanMeshes.Count; i++) {
            oceanMeshes[i].ShiftMesh(oceanPos, Time.time);
        }

    }

    IEnumerator UpdateWater() {
        while (true) {
            if (hasThreadUpdatedWater) { //is water updated??

                transform.position = oceanPos;
                for (int i = 0; i < oceanMeshes.Count; i++) {
                    oceanMeshes[i].meshFilter.mesh.vertices = oceanMeshes[i].vertices;
                    oceanMeshes[i].meshFilter.mesh.RecalculateNormals();
                }

                hasThreadUpdatedWater = false;
                ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateWaterWithThreadPooling));

            }
            yield return new WaitForSeconds(Time.deltaTime * 3f);
        }
    }

    void UpdateWaterWithThreadPooling(object state) {
        MoveWaterToShip();
        for (int i = 0; i < oceanMeshes.Count; i++) {

            Vector3 centerPos = oceanMeshes[i].centerPos;
            Vector3[] verts = oceanMeshes[i].vertices;
            for (int j = 0; j < verts.Length; j++) {
                Vector3 vertexPos = verts[i];
                Vector3 vertexPosGlobal = vertexPos + centerPos + oceanPos;
                vertexPos.y = WaveHandler.current.GetWaveYPos(vertexPosGlobal, secondsSinceStart);
            }
        }
    }
    void MoveWaterToShip()
    {
        float x = innerSquareResolution * (int)Mathf.Round(shipPos.x / innerSquareResolution);
        float z = innerSquareResolution * (int)Mathf.Round(shipPos.z / innerSquareResolution);
        if (oceanPos.x != x || oceanPos.z != z)
        {
            Debug.Log("Moved Sea");
            oceanPos = new Vector3(x, oceanPos.y, z); 
        }
    }

    void CreateEndlessOcean() {
        //centerpiece
        AddGrid(0f, 0f, 0f, squareWidth, innerSquareResolution);

        //cornerpieces x8
        for (int x = -1; x <=  1; x =+ 1) {
            for (int z = -1; z <= 1; z+= 1) {
                if (x == 0 && z == 0)
                {
                    continue;
                }
                float yPos = -0.5f;
                AddGrid(x * squareWidth, z * squareWidth, yPos, squareWidth, outerSquareResolution);
            }
        }
    }

    void AddGrid(float x, float z, float y, float squareWidth, float cellSize)
    {
        GameObject grid = Instantiate(oceanObj, transform.position, transform.rotation) as GameObject;
        grid.SetActive(true);
        Vector3 centerPos = transform.position;
        centerPos.x += x;
        centerPos.y = y;
        centerPos.z += z;

        grid.transform.position = centerPos;
        grid.transform.parent = transform;

        OceanMeshGen newOceanMesh = new OceanMeshGen(grid, squareWidth, cellSize);
        oceanMeshes.Add(newOceanMesh);
    }
}
