using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TriangleData {

    public Vector3 p0;
    public Vector3 p1;
    public Vector3 p2;

    //The center of the triangle
    public Vector3 center;

    //The distance to the surface from the center of the triangle
    public float distanceToSurface;

    //The normal to the triangle
    public Vector3 normal;

    //The area of the triangle
    public float area;

    //The velocity of the triangle at the center
    public Vector3 velocity;

    //The velocity normalized
    public Vector3 velocityDir;

    //The angle between the normal and the velocity
    //Negative if pointing in the opposite direction
    //Positive if pointing in the same direction
    public float cosTheta;

    public TriangleData(Vector3 p0, Vector3 p1, Vector3 p2, Rigidbody boatRB, float timeSinceStart)
    {
        this.p0 = p0;
        this.p1 = p1;
        this.p2 = p2;

        //Center of the triangle
        this.center = (p0 + p1 + p2) / 3f;

        //Distance to the surface from the center of the triangle
        this.distanceToSurface = Mathf.Abs(WaveHandler.instance.DistanceToWater(this.center, timeSinceStart));

        //Normal to the triangle
        this.normal = Vector3.Cross(p1 - p0, p2 - p0).normalized;

        //Area of the triangle
        this.area = ShipMath.GetTriangleArea(p0, p1, p2);

        //Velocity vector of the triangle at the center
        this.velocity = ShipMath.GetTriangleVelocity(boatRB, this.center);

        //Velocity direction
        this.velocityDir = this.velocity.normalized;

        //Angle between the normal and the velocity
        //Negative if pointing in the opposite direction
        //Positive if pointing in the same direction
        this.cosTheta = Vector3.Dot(this.velocityDir, this.normal);

    }
}

