using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Triangle{
    Vector3 p0, p1, p2;
}



public class FloatingActor : MonoBehaviour
{
    //refs
    public GameObject waterPatch;
    public GameObject actor;


    //vars
    Mesh mesh; //ship mesh
    Mesh patchMesh;


    Vector3[] vertices;
    bool[] vertexState;

    // Start is called before the first frame update
    void Start()
    {
        mesh = actor.GetComponent<MeshFilter>().mesh;
        patchMesh = waterPatch.GetComponent<MeshFilter>().mesh;
        vertexState = new bool[mesh.vertexCount];
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGridPositionandRotation();
    }
    bool VertexAboveWater(int index, Vector3 p) //index and position
    {
        vertexState[index] = true;
        Vector2 g; //gridcell x,z;
        int i = 0, j = 0, n = 0; // row column amount of rows/columns
        i = [(p.x - g.x) / n]; // return row index of waterpatch squares
        j = [(p.z - g.y) / n]; // return row index of waterpatch squares

        //X and Y in - cell cords
        int Xcp, Zcp;
        //get vertex position x,z to correspond with square on waterpatch.
        //if vertex position.y is greater than waterpatch position.y then return true, else return false
        return false;
    }
    //ActorMesh Functions
    void GetVertices() 
    {
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            Vector3 vertexPos = mesh.vertices[i];
            bool aboveWater = VertexAboveWater(i, vertexPos);
            
            //load in current triangle, and determine if its upside down
            if(aboveWater)
            {
                
                //if height above water set vertex to positive
            }
            else
            {
                //if height below water set vertex to negative
            }

        }
        //return vertex heightdata
    }
    void GetTriangles() {
        Mesh mesh = actor.GetComponent<MeshFilter>().mesh;
        //get triangles
        
        for (int i = 0; i < mesh.triangles.Length; i++)
        {

            //loop through all the triangles in the boat mesh and

            //crossreference and divide up into above, below and border triangles.
        }
        //return triangles into 3 specific groups.
    }
    //waterGrid Functions
    void UpdateGridPositionandRotation() 
    {
        Vector3 newPos = new Vector3(actor.transform.position.x, 0, actor.transform.position.z);
        Quaternion newRot = new Quaternion(0, actor.transform.rotation.y, 0, actor.transform.rotation.w);
        waterPatch.transform.SetPositionAndRotation(newPos, newRot);



    }

    void UpdateGridHeight(Vector3 p, Vector2 g)
    {
        for (int i = 0; i < patchMesh.vertexCount; i++)
        {
            //update the grid height by sampling the water shader
            //behöver hjälp från philip/smörgås med detta, vet inte hur jag samplar
        }
    }

    void IndiceWaterLevel()// unknown function (felt cute, might delete later)
    {

    }

    //Triangle Cutting functions

    void CutTriangle() { //smart triangle cut function that determines if its a 2:1 triangle or a 1:2
        
    }

    void OneAboveTwoBelow(Triangle tri)
    {//if two of the triangle verticies is below water 

    } 
    void TwoAboveOneBelow(Triangle tri)
    {//if one of the triangles verticies is below water

    } 
}
