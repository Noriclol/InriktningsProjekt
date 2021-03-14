using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//script that splits up the boatmesh into groups of triangles depending on their relation to the surface and generate a hull mesh of the part of the hull thats underwater
public class MeshSegmentation
{
    //DEBUG FIELDS

    int[] debugArray = new int[4];



    //transform verts and tris for boat
    private Transform shipTrans;
    Vector3[] shipVerts;
    int[] shipTris;

    //So we only need to make the transformation from local to global once
    public Vector3[] shipVertsGlobal;
    //Find all the distances to water once because some triangles share vertices, so reuse
    float[] allDistancesToWater;

    //The triangles belonging to the part of the boat that's under water
    public List<TriangleData> underWaterTriangleData = new List<TriangleData>();

    public MeshSegmentation(GameObject underwaterMesh)
    {
        //gets refs
        shipTrans = underwaterMesh.transform;
        shipVerts = underwaterMesh.GetComponent<MeshFilter>().mesh.vertices;
        shipTris = underwaterMesh.GetComponent<MeshFilter>().mesh.triangles;

        //get verts in global pos
        shipVertsGlobal = new Vector3[shipVerts.Length];
        //set distance for all triangles underwater
        allDistancesToWater = new float[shipVerts.Length];

    }

    public void GenerateUnderwaterMesh()
    {
        //Debug.Log("attempting to generate mesh");
        underWaterTriangleData.Clear();
        for (int j = 0; j < shipVerts.Length; j++)
        {
            Vector3 gPos = shipTrans.TransformPoint(shipVerts[j]);
            shipVertsGlobal[j] = gPos;
            allDistancesToWater[j] = WaveHandler.current.DistanceToSurface(gPos, Time.time); //returns 0 atm. need to fix wavehandler
        }
        AddTriangles();
        //Debug.Log("triangles found = " + underWaterTriangleData.Count);
    }

    private void AddTriangles() 
    {

        List<VertexData> vertexData = new List<VertexData>();

        vertexData.Add(new VertexData());
        vertexData.Add(new VertexData());
        vertexData.Add(new VertexData());


        int i = 0;
        while (i < shipTris.Length) //for each triangle
        {
            for (int x = 0; x < 3; x++) //loop through the verts
            { //gather data
                vertexData[x].distance = allDistancesToWater[shipTris[i]];
                vertexData[x].index = x;
                vertexData[x].globalVertexPos = shipVertsGlobal[shipTris[i]];
                i++;
            }
            if (vertexData[0].distance > 0f && vertexData[1].distance > 0f && vertexData[2].distance > 0f) // if all verts above sea
            {

                continue;
            }
            if (vertexData[0].distance < 0f && vertexData[1].distance < 0f && vertexData[2].distance < 0f) // if all verts below sea
            {
                Vector3 p0 = vertexData[0].globalVertexPos;
                Vector3 p1 = vertexData[1].globalVertexPos;
                Vector3 p2 = vertexData[2].globalVertexPos;


                underWaterTriangleData.Add(new TriangleData(p0, p1, p2));
                
            }
            //1 or 2 vertices are below the water
            else
            {
                //sort the verts after height from surface
                vertexData.Sort((x, y) => x.distance.CompareTo(y.distance));

                vertexData.Reverse();
                if (vertexData[0].distance > 0f && vertexData[1].distance < 0f && vertexData[2].distance < 0f) // if one point is above rest below
                {
                    OneAboveTwoBelow(vertexData);
                }
                else if (vertexData[0].distance > 0f && vertexData[1].distance > 0f && vertexData[2].distance < 0f) // if one point is above rest below
                {
                    TwoAboveOneBelow(vertexData);
                }
            }
        }
    }

    private void OneAboveTwoBelow(List<VertexData> vertexData)
    {
        Vector3 H = vertexData[0].globalVertexPos;
        Vector3 M = Vector3.zero;
        Vector3 L = Vector3.zero;

        int M_index = vertexData[0].index - 1; //find index of M
        if (M_index < 0) { M_index = 2; }

        float h_H = vertexData[0].distance;
        float h_M = 0f, h_L = 0f;

        if (vertexData[1].index == M_index) //if M is in pos 1
        {
            M = vertexData[1].globalVertexPos;
            L = vertexData[2].globalVertexPos;

            h_M = vertexData[1].distance;
            h_L = vertexData[2].distance;
        }
        else
        {
            M = vertexData[2].globalVertexPos;
            L = vertexData[1].globalVertexPos;

            h_M = vertexData[2].distance;
            h_L = vertexData[1].distance;
        }

        //calculate cutting points to separate triangles

        //Point I_M
        Vector3 MH = H - M;

        float t_M = -h_M / (h_H - h_M);

        Vector3 MI_M = t_M * MH;

        Vector3 I_M = MI_M + M; //cut 1


        //Point I_L
        Vector3 LH = H - L;

        float t_L = -h_L / (h_H - h_L);

        Vector3 LI_L = t_L * LH;

        Vector3 I_L = LI_L + L; //cut 2

        //Save the data, such as normal, area, etc      
        //2 triangles below the water  
        underWaterTriangleData.Add(new TriangleData(M, I_M, I_L));
        underWaterTriangleData.Add(new TriangleData(M, I_L, L));
    }
    private void TwoAboveOneBelow(List<VertexData> vertexData)
    {
        Vector3 L = vertexData[2].globalVertexPos;
        Vector3 H = Vector3.zero;
        Vector3 M = Vector3.zero;

        //index of H
        int H_index = vertexData[2].index + 1;
        if (H_index > 2) { H_index = 0; }

        float h_L = vertexData[2].distance;
        float h_M = 0f, h_H = 0f;

        if (vertexData[1].index == H_index) //H is in pos 1
        {
            H = vertexData[1].globalVertexPos;
            M = vertexData[0].globalVertexPos;

            h_H = vertexData[1].distance;
            h_M = vertexData[0].distance;
        }
        else //H is in pos 0
        {
            H = vertexData[0].globalVertexPos;
            M = vertexData[1].globalVertexPos;

            h_H = vertexData[0].distance;
            h_M = vertexData[1].distance;
        }




        //find cutting points
        Vector3 LM = M - L;

        float t_M = -h_L / (h_M - h_L);

        Vector3 LJ_M = t_M * LM;

        Vector3 J_M = LJ_M + L; //Point 1


        
        Vector3 LH = H - L;

        float t_H = -h_L / (h_H - h_L);

        Vector3 LJ_H = t_H * LH;

        Vector3 J_H = LJ_H + L; //Point 2


        //Save the data, such as normal, area, etc
        //1 triangle below the water
        underWaterTriangleData.Add(new TriangleData(L, J_H, J_M));
    }

    private class VertexData //helper function for passing vertex data
    {
        //The distance to water from this vertex
        public float distance;
        //An index so we can form clockwise triangles
        public int index;
        //The global Vector3 position of the vertex
        public Vector3 globalVertexPos;
    }

    public void DisplayMesh(Mesh mesh, string name, List<TriangleData> triangleData) {
        List<Vector3> verticies = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i < triangleData.Count; i++) {
            //reverse vertices from global to local and save as 3 temp vectors
            Vector3 p0, p1, p2;
            p0 = shipTrans.InverseTransformPoint(triangleData[i].p0);
            p1 = shipTrans.InverseTransformPoint(triangleData[i].p1);
            p2 = shipTrans.InverseTransformPoint(triangleData[i].p2);

            //add verticies and indices to meshfields.
            verticies.Add(p0);
            triangles.Add(verticies.Count - 1);
            verticies.Add(p1);
            triangles.Add(verticies.Count - 1);
            verticies.Add(p2);
            triangles.Add(verticies.Count - 1);
        }
        mesh.Clear();

        mesh.name = name;
        mesh.vertices = verticies.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}