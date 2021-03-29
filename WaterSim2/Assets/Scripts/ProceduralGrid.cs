using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGrid : MonoBehaviour
{
    public Transform gridTransform;
    public MeshFilter meshFilter;

    //total grid size
    private float size;
    //size of each square
    private float spacing;
    //width in gridsquares
    private int width;

    public Vector3 centerPos;
    public Vector3[] vertices;

    public void  SetProceduralGrid(GameObject gridObj, float size, float spacing)
    {
        this.gridTransform = gridObj.transform;
        this.size = size;
        this.spacing = spacing;
        this.meshFilter = gridObj.GetComponent<MeshFilter>();

        width = (int)(size / spacing);
        width += 1;

        //centering
        float offset = -(width - 1) * spacing / 2;
        Vector3 newPos = new Vector3(offset, gridTransform.position.y, offset);
        gridTransform.position += newPos;

        GenerateMesh();

    }


    //moves the tile through cpu
    public void MoveSea(Vector3 oceanPos, float timeSinceStart)
    {
        Vector3[] vertices = meshFilter.mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];

            Vector3 vertexGlobal = vertex + centerPos + oceanPos;


            //Get the water height at this coordinate
            vertex.y = WaveHandler.instance.GetWaveYPos(vertexGlobal, timeSinceStart);

            //From global to local - not needed if we use the saved local x,z position
            //vertices[i] = transform.InverseTransformPoint(vertex);

            //Don't need to go from global to local because the y pos is always at 0
            vertices[i] = vertex;
        }

        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateNormals();



    }

    public void GenerateMesh()
    {
        //Data
        List<Vector3[]> verts = new List<Vector3[]>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int z = 0; z < width; z++)
        {
            verts.Add(new Vector3[width]);
            //Debug.Log("row: " + verts.Count);
            for (int x = 0; x < width; x++)
            {
                Vector3 current_point = new Vector3();
                current_point.x = x * spacing;
                current_point.z = z * spacing;
                current_point.y = gridTransform.position.y;

                //add vertex to vert 2D array
                verts[z][x] = current_point;
                uvs.Add(new Vector2(x, z));

                //dont generate on the first column and row
                if (x <= 0 || z <= 0) { continue; }

                //The triangle south-west of the vertice
                tris.Add(x + z * width);
                tris.Add(x + (z - 1) * width);
                tris.Add((x - 1) + (z - 1) * width);

                //The triangle west-south of the vertice
                tris.Add(x + z * width);
                tris.Add((x - 1) + (z - 1) * width);
                tris.Add((x - 1) + z * width);

            }
        }


        //unfold 2D array
        Vector3[] unfolded_verts = new Vector3[width * width];
        int i = 0;
        foreach (Vector3[] v in verts)
        {
            v.CopyTo(unfolded_verts, i * width);
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
