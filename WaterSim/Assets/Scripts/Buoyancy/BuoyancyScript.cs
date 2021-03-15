using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshCollider), typeof(Rigidbody))]
public class BuoyancyScript : MonoBehaviour
{
    /* Interface script to add to GameObjects
     This script has a main purpouse of being an interface to the user.
     Also the "parent" Script that other related scripts run under.
     */



    //refs
    public GameObject underwaterObj;

    //fields
    private Rigidbody rb;
    private Mesh underwaterMesh;
    private ShipAlgoritms algorithmLib;
    private MeshGridGenerator grid;

    private float waterDensity = 1027f;
    

    private void Start()
	{
        algorithmLib = new ShipAlgoritms();
        rb = gameObject.GetComponent<Rigidbody>();
        algorithmLib.InitTriCutAlgoritm(gameObject);
        underwaterMesh = underwaterObj.GetComponent<MeshFilter>().mesh;
	}
    private void Update()
    {
        //generate and display the underwater mesh
        algorithmLib.GenerateUnderwaterMesh();
        algorithmLib.DisplayMesh(underwaterMesh, "Underwater Mesh", algorithmLib.underWaterTriangleData);
    }

    private void FixedUpdate()
    {
        if (algorithmLib.underWaterTriangleData.Count > 0)
        {
            AddUnderWaterForces();
        }
        //Debug.Log("Triangles Under Water: " + algorithmLib.underWaterTriangleData.Count);
    }

    void AddUnderWaterForces()
    {
        List<TriangleData> underWaterTriangleData = algorithmLib.underWaterTriangleData;
        //loop through all triangles and apply forces
        for (int i = 0; i < underWaterTriangleData.Count; i++) {
            TriangleData triangleData = underWaterTriangleData[i];
            Vector3 buoyancyForce = BuoyancyForce(waterDensity, triangleData);

            rb.AddForceAtPosition(buoyancyForce, triangleData.center);

            //Normal
            //Debug.DrawRay(triangleData.center, triangleData.normal * 3f, Color.white);

            //Buoyancy
            //Debug.DrawRay(triangleData.center, buoyancyForce.normalized * -3f, Color.blue);
        }
    }

    private Vector3 BuoyancyForce(float waterDensity, TriangleData triangleData)
    {
        Vector3 buoyancyForce = waterDensity * Physics.gravity.y * triangleData.distanceToSurface * triangleData.area * triangleData.normal;

        //The vertical component of the hydrostatic forces don't cancel out but the horizontal do
        
        buoyancyForce.x = 0f;
        buoyancyForce.z = 0f;

        return buoyancyForce;
    }



}




