using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGridGenerator : MonoBehaviour
{
    //Fields BEGIN

    //public fields
    
    public Transform gridTransform;
    public float cellSize; // size of cell
    public int gridSize; // cell amount
    public Vector3 centerPos;
    public bool CPUWaves;

    //grid internal fields;
    public int[] triangles;
    public Vector3[] vertices;
    public MeshFilter meshFilter;


    //when calling constructor Start will overwrite set values // not working. as is the the mesh generates twice when instantiated from custom constructor
    //will have to have parent variable handling bool
    //bool constructorCalled = false;
    //Fields END

    private void Start()
	{
        //Debug.Log("Start"); //debug

  //      if (!constructorCalled)
		//{
            //Debug.Log("constructor not called"); //debug
            gridTransform = gameObject.transform;
            meshFilter = gameObject.GetComponent<MeshFilter>();
            centerPos = gameObject.transform.localPosition;

            //center Grid
            gridSize += 1;
            float offset = -((gridSize - 1) * cellSize) / 2;
            Vector3 newPos = new Vector3(offset, gridTransform.position.y, offset);
            gridTransform.position = newPos;

            //Generate
            float startTime = System.Environment.TickCount; //debug
            GenerateMesh();
            float timeToGenerateSea = (System.Environment.TickCount - startTime) / 1000f; //debug
            //Debug.Log("Sea was generated in " + timeToGenerateSea.ToString() + " seconds"); //debug
        //}
        //return;
    }


    public MeshGridGenerator(GameObject parentObj, float cellSize, int gridSize)
    {
        //Debug.Log("Constructor called");
        //constructorCalled = true;
        //initialize fields;
        this.cellSize = cellSize;
        this.gridSize = gridSize;
        this.gridTransform = parentObj.transform;
        this.meshFilter = gridTransform.GetComponent<MeshFilter>();
        this.centerPos = parentObj.transform.localPosition;

        //Center the sea
        gridSize += 1;
        float offset = -((gridSize - 1) * cellSize) / 2;
        Vector3 newPos = new Vector3(offset, gridTransform.position.y, offset);
        gridTransform.position = newPos;


        float startTime = System.Environment.TickCount; //debug
        GenerateMesh();
        float timeToGenerateSea = (System.Environment.TickCount - startTime) / 1000f; //debug
        Debug.Log("Sea was generated in " + timeToGenerateSea.ToString() + " seconds"); //debug
    }
    public void AnimateMesh(Vector3 oceanPos, float timeSinceStart)
    {

        Vector3[] vertices = meshFilter.mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];
            Vector3 vertexGlobal = vertex + centerPos + oceanPos;


            //Get the water height at this coordinate

            vertex.y = WaveHandler.current.GetWaveYPos(vertexGlobal, timeSinceStart);
            
            //From global to local - not needed if we use the saved local x,z position
            vertices[i] = transform.InverseTransformPoint(vertex);

            //Don't need to go from global to local because the y pos is always at 0
            vertices[i] = vertex;
        }
        Debug.Log("Wave Sampler: " + WaveHandler.current.GetWaveYPos(new Vector3(0, 0, 0), timeSinceStart));
        meshFilter.mesh.vertices = vertices;

        meshFilter.mesh.RecalculateNormals();
    }

        //Public Functions
        public void GenerateMesh()
    {
        //Data
        List<Vector3[]> verts = new List<Vector3[]>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int z = 0; z < gridSize; z++)
        {
            verts.Add(new Vector3[gridSize]);
            //Debug.Log("row: " + verts.Count);
            for (int x = 0; x < gridSize; x++)
            {
                Vector3 current_point = new Vector3();
                current_point.x = x * cellSize;
                current_point.z = z * cellSize;
                current_point.y = gridTransform.position.y;

                //add vertex to vert 2D array
                verts[z][x] = current_point;
                uvs.Add(new Vector2(x, z));

                //dont generate on the first column and row
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


        //unfold 2D array
        Vector3[] unfolded_verts = new Vector3[gridSize * gridSize];
        int i = 0;
        foreach (Vector3[] v in verts)
        {
            v.CopyTo(unfolded_verts, i * gridSize);
            i++;
        }

        //apply meshData
        Mesh mesh = new Mesh();
        mesh.vertices = unfolded_verts;
        mesh.uv = uvs.ToArray();
        mesh.triangles = tris.ToArray();

        //recalc
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        //clear old mesh and reapply
        meshFilter.mesh.Clear();
        meshFilter.mesh = mesh;
        meshFilter.mesh.name = "Water Grid";
    }
}
