using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TriangleData
{
    /*
     TriangleData Struct for ease of use.
     */
    //vertex, centeroftri, normaloftri
    public Vector3 p0, p1, p2, center, normal;
    //center to surface, area
    public float distanceToSurface, area;

    public TriangleData(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        this.p0 = p0;
        this.p1 = p1;
        this.p2 = p2;

        this.center = (p0 + p1 + p2) / 3f;

        this.distanceToSurface = Mathf.Abs(1); //plug distance to water here!
        this.normal = Vector3.Cross(p1 - p0, p2 - p0).normalized;

        //area

        float a = Vector3.Distance(p0, p1), c = Vector3.Distance(p2, p0);
        this.area = (a * c * Mathf.Sin(Vector3.Angle(p1 - p0, p2 - p0) * Mathf.Deg2Rad)) / 2f;

    }
    public class VertexData
    {
        //The distance to water from this vertex
        public float distance;
        //An index so we can form clockwise triangles
        public int index;
        //The global Vector3 position of the vertex
        public Vector3 globalVertexPos;
    }
}
