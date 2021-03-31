using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class OceanHandler : MonoBehaviour
{
    public OceanHandler instance;
    //public GameManager instanceRef;
    public GameObject playerObj;
    public GameObject gridRef;
    

    //SquareParameters
    private float squareWidth = 1000f;
    private float innerGridRes = 5f;
    private float outerGridRes = 20f;

    List<ProceduralGrid> oceanGrids = new List<ProceduralGrid>();
    Vector3 playerPos, oceanPos;
    bool hasThreadUpdatedWater;
    void Start()
    {
        //instanceRef = new GameManager().instance;
        instance = this;
        CreateEndlessSea();

        ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateWaterWithThreadPooling));
        StartCoroutine(UpdateWater());

    }

    void Update()
    {
        UpdateWaterNoThread();
        playerPos = playerObj.transform.position;

    } 

    //Update the water with no thread to compare 
    void UpdateWaterNoThread()
    {
        playerPos = playerObj.transform.position;
        MoveWaterToBoat();
        transform.position = oceanPos;

        //for (int i = 0; i < oceanGrids.Count; i++)
        //{
        //    oceanGrids[i].MoveSea(oceanPos, Time.time);
        //}
    }


    //The loop that gives the updated vertices from the thread to the meshes
    //which we can't do in its own thread
    IEnumerator UpdateWater()
    {
        while (true)
        {
            //Has the thread finished updating the water?
            if (hasThreadUpdatedWater) {
                transform.position = oceanPos;

                //Add the updated vertices to the water meshes
                for (int i = 0; i < oceanGrids.Count; i++) {
                    oceanGrids[i].meshFilter.mesh.vertices = oceanGrids[i].vertices;

                    oceanGrids[i].meshFilter.mesh.RecalculateNormals();
                }
                hasThreadUpdatedWater = false;
                ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateWaterWithThreadPooling));
            }

            //Don't need to update the water every frame
            yield return new WaitForSeconds(Time.deltaTime * 3f);
        }
    }

    //The thread that updates the water vertices
    void UpdateWaterWithThreadPooling(object state)
    {
        //Move the water to the boat
        MoveWaterToBoat();

        //Loop through all water squares
        for (int i = 0; i < oceanGrids.Count; i++)
        {
            //The local center pos of this square
            Vector3 centerPos = oceanGrids[i].centerPos;
            //All the vertices this square consists of
            Vector3[] vertices = oceanGrids[i].vertices;

            //Update the vertices in this square
            for (int j = 0; j < vertices.Length; j++)
            {
                //The local position of the vertex
                Vector3 vertexPos = vertices[j];

                //Can't use transformpoint in a thread, so to find the global position of the vertex
                //we just add the position of the ocean and the square because rotation and scale is always 0 and 1
                Vector3 vertexPosGlobal = vertexPos + centerPos + oceanPos;

                //Get the water height
                //vertexPos.y = WaveHandler.instance.GetWaveYPos(vertexPosGlobal, GameManager.secondsSinceStart);

                //Save the new y coordinate, but x and z are still in local position
                vertices[j] = vertexPos;
            }
        }

        hasThreadUpdatedWater = true;

        //Debug.Log("Thread finished");
    }

    //Move the endless water to the boat's position in steps that's the same as the water's resolution
    void MoveWaterToBoat()
    {
        //Round to nearest resolution
        float x = innerGridRes * (int)Mathf.Round(playerPos.x / innerGridRes);
        float z = innerGridRes * (int)Mathf.Round(playerPos.z / innerGridRes);

        //Should we move the water?
        if (oceanPos.x != x || oceanPos.z != z)
        {
            //Debug.Log("Moved sea");

            oceanPos = new Vector3(x, oceanPos.y, z);
        }
    }

    //Init the endless sea by creating all squares
    void CreateEndlessSea()
    {
        //The center piece
        AddGrid(0f, 0f, 0f, squareWidth, innerGridRes);

        //The 8 squares around the center square
        for (int x = -1; x <= 1; x += 1)
        {
            for (int z = -1; z <= 1; z += 1)
            {
                //Ignore the center pos
                if (x == 0 && z == 0)
                {
                    continue;
                }

                //The y-Pos should be lower than the square with high resolution to avoid an ugly seam
                float yPos = -0.5f;
                AddGrid(x * squareWidth, z * squareWidth, yPos, squareWidth, outerGridRes);
            }
        }
    }

    //Add one water plane
    void AddGrid(float xCoord, float zCoord, float yPos, float squareWidth, float spacing)
    {
        GameObject waterPlane = Instantiate(gridRef, transform.position, transform.rotation) as GameObject;

        waterPlane.SetActive(true);

        //Change its position
        Vector3 centerPos = transform.position;

        centerPos.x += xCoord;
        centerPos.y = yPos;
        centerPos.z += zCoord;

        waterPlane.transform.position = centerPos;

        //Parent it
        waterPlane.transform.parent = transform;

        //Give it moving water properties and set its width and resolution to generate the water mesh
        ProceduralGrid newWaterSquare = waterPlane.AddComponent<ProceduralGrid>();
        newWaterSquare.SetProceduralGrid(waterPlane, squareWidth, spacing);

        oceanGrids.Add(newWaterSquare);
    }
}
