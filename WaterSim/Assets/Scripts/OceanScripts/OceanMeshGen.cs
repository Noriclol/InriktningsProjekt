using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class OceanMeshGen : MonoBehaviour
{
    //public fields
    public Transform gridTransform;
    public Vector3 centerPos;

    //modifier fields
    float scale; //size
    int gridSize; //width
    float cellSize; // spacing


    //grid internal fields;
    private int[] triangles;
    private Vector3[] vertices;
    private MeshFilter meshFilter;
    //private Mesh mesh;

    //Constructor
    OceanMeshGen(GameObject waterGridObj, float scale, int gridSize)
    {
        this.gridTransform = waterGridObj.transform;

        this.scale = scale;
        this.gridSize = gridSize;

        gridSize = (int)(scale / cellSize);
        gridSize += 1;

        //Center the sea
        float offset = -((gridSize - 1) * cellSize) / 2;

        Vector3 newPos = new Vector3(offset, gridTransform.position.y, offset);
        gridTransform.position = newPos;

        this.centerPos = waterGridObj.transform.localPosition;

        float startTime = System.Environment.TickCount;
        GenerateMesh();
        float timeToGenerateSea = (System.Environment.TickCount - startTime) / 1000f;
        Debug.Log("Sea was generated in " + timeToGenerateSea.ToString() + " seconds");

    }

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ShiftMesh(Vector3 oceanPos, float timeSinceStart)
    {
        Vector3[] vertices = meshFilter.mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];
            Vector3 vertexGlobal = vertex + centerPos + oceanPos;

            //vertex.y; //set the 
            vertices[i] = vertex;
        }
    }

    void GenerateMesh() 
    {
        //Data allocation
        List<Vector3[]> verts = new List<Vector3[]>(); ;
        List<int> tris = new List<int>(); ;
        List<Vector2> uvs = new List<Vector2>();
        List<Vector4> tangents = new List<Vector4>(); //tangents might be in wrong direction. disable if it stops working i guess.

        for (int z = 0; z < gridSize; z++)
        {
            verts.Add(new Vector3[gridSize]);
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
                if (x <= 0 || z <= 0)
                    continue;

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
            //Copies all the elements of the current 1D-array to the specified 1D-array
            v.CopyTo(unfolded_verts, i * gridSize);

            i++;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = unfolded_verts;
        mesh.uv = uvs.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.tangents = tangents.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        meshFilter.mesh.Clear();
        meshFilter.mesh = mesh;
        meshFilter.mesh.name = "Water Grid";

        Debug.Log("Generated Ocean Grid");
        Debug.Log(meshFilter.mesh.vertices.Length);
        
    }


   
}
