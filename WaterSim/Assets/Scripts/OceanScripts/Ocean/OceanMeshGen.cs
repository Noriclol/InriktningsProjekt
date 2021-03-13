using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class OceanMeshGen : MonoBehaviour
{
    //public fields
    public Transform gridTransform;
    public Vector3 centerPos;

    //modifier fields
    float scale; //size
    float cellSize; // spacing
    int gridSize; //width


    //grid internal fields;
    public int[] triangles;
    public Vector3[] vertices;
    public MeshFilter meshFilter;

    //Constructor
    public OceanMeshGen(GameObject waterGridObj, float scale, float cellSize)
    {
        this.gridTransform = waterGridObj.transform;

        this.scale = scale;
        this.cellSize = cellSize;

        this.meshFilter = gridTransform.GetComponent<MeshFilter>();

        gridSize = (int)(scale / cellSize);
        gridSize += 1;

        //Center the sea
        float offset = -((gridSize - 1) * cellSize) / 2;

        Vector3 newPos = new Vector3(offset, gridTransform.position.y, offset);
        gridTransform.position += newPos;

        this.centerPos = waterGridObj.transform.localPosition;

        float startTime = System.Environment.TickCount;
        GenerateMesh();
        float timeToGenerateSea = (System.Environment.TickCount - startTime) / 1000f;
        Debug.Log("Sea was generated in " + timeToGenerateSea.ToString() + " seconds");

    }

    public void ShiftMesh(Vector3 oceanPos, float timeSinceStart) //shifts the oceangrid to new position
    {
        Vector3[] vertices = meshFilter.mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];
            Vector3 vertexGlobal = vertex + centerPos + oceanPos;

            vertex.y = WaveHandler.current.GetWaveYPos(vertexGlobal, timeSinceStart);
            vertices[i] = vertex;
        }
    }

    public void GenerateMesh() 
    {

        //Data allocation
        List<Vector3[]> verts = new List<Vector3[]>(); ;
        List<int> tris = new List<int>(); ;
        List<Vector2> uvs = new List<Vector2>();
        List<Vector4> tangents = new List<Vector4>(); //tangents might be in wrong direction. disable if it stops working i guess.

        for (int z = 0; z < gridSize; z++)
        {
            verts.Add(new Vector3[gridSize]);
            Debug.Log("row: " + verts.Count);
            for (int x = 0; x < gridSize; x++)
            {
                Vector3 current_point = new Vector3();
                current_point.x = x * cellSize;
                current_point.z = z * cellSize;
                current_point.y = gridTransform.position.y;
                //return vertex to vert 2d array
                verts[z][x] = current_point;
                uvs.Add(new Vector2(x, z));
                tangents.Add(new Vector4(1f, 0f, 0f, -1f)); // might be 0, 1, 0, -1 instead;

                //if on first corner cancel gen triangle
                if (x <= 0 || z <= 0) { continue; }

                //The triangle south-west of the vertice
                tris.Add(x + z * gridSize);
                tris.Add(x + (z - 1) * gridSize);
                tris.Add((x - 1) + (z - 1) * gridSize);

                //The triangle west-south of the vertice
                tris.Add(x + z * gridSize);
                tris.Add((x - 1) + (z - 1) * gridSize);
                tris.Add((x - 1) + z * gridSize);

            }
        }


        //unfold the 2d Array so it can be saved in Vertices array.
        Vector3[] unfolded_verts = new Vector3[gridSize * gridSize];
        int i = 0;
        foreach (Vector3[] v in verts)
        {
            //Copies all the elements of the current 2D-array to the specified 1D-array
            v.CopyTo(unfolded_verts, i * gridSize);
            i++;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = unfolded_verts;
        mesh.uv = uvs.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.tangents = tangents.ToArray();

        //Debug.Log(mesh.vertices.Length);
        //Debug.Log(mesh.triangles.Length);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        meshFilter.mesh.Clear();
        meshFilter.mesh = mesh;
        meshFilter.mesh.name = "Water Grid";

        //Debug.Log("Generated Ocean Grid");
        //Debug.Log(meshFilter.mesh.vertices.Length);
        //Debug.Log(meshFilter.mesh.triangles.Length);
    }


   
}
