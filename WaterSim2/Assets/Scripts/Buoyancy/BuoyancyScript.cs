using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuoyancyScript : MonoBehaviour
{
    public ShipType shipType;
    public GameObject boatMeshObj;
    public GameObject underWaterObj;
    public GameObject aboveWaterObj;
    public Vector3 centerOfMass;

    private Mesh underWaterMesh;
    private Mesh aboveWaterMesh;

    private ShipMath shipMath;
    private Rigidbody shipRB;
    //densities
    private float waterDensity = 1027f;
    private float airDensity = 1.225f;
    //Drag coefficients
    public const float C_d_flat_plate_perpendicular_to_flow = 1.28f;

    void Awake()
    {
        shipRB = this.GetComponent<Rigidbody>();
        shipRB.mass = shipType.mass;
        //shipRB.centerOfMass = centerOfMass;
    }


    void Start()
    {
        shipMath = new ShipMath();
        shipMath.InitTriCutAlgoritm(gameObject, underWaterObj, aboveWaterObj, shipRB);
        underWaterMesh = underWaterObj.GetComponent<MeshFilter>().mesh;
        aboveWaterMesh = aboveWaterObj.GetComponent<MeshFilter>().mesh;
    }

    void Update()
    {
        shipMath.GenerateUnderwaterMesh();

        shipMath.DisplayMesh(underWaterMesh, "Underwater Mesh", shipMath.underWaterTriangleData);
        shipMath.DisplayMesh(aboveWaterMesh, "Abovewater Mesh", shipMath.aboveWaterTriangleData);
    }

    private void FixedUpdate()
    {
        shipRB.centerOfMass = centerOfMass;
        if (shipMath.underWaterTriangleData.Count > 0)
        {
            AddUnderWaterForces();
        }
        if (shipMath.aboveWaterTriangleData.Count > 0)
        {
            AddAboveWaterForces();
        }
        //Debug.Log("Triangles Under Water: " + shipMath.underWaterTriangleData.Count);
    }
    void AddUnderWaterForces()
    {
        float Cf = ShipMath.ResistanceCoefficient(waterDensity, shipRB.velocity.magnitude, shipMath.CalculateUnderWaterLength());

        List<SlammingForceData> slammingForceData = shipMath.slammingForceData;
        CalculateSlammingVelocities(slammingForceData);

        float boatArea = shipMath.boatArea;
        float boatMass = shipType.mass;

        List<int> indexOfOriginalTriagle = shipMath.indexOfOriginalTriangle;

        List<TriangleData> underWaterTriangleData = shipMath.underWaterTriangleData;

        //loop through all triangles and apply forces
        for (int i = 0; i < underWaterTriangleData.Count; i++)
        {
            TriangleData triangleData = underWaterTriangleData[i];

            Vector3 forceToAdd = Vector3.zero;
            forceToAdd += ShipMath.BuoyancyForce(waterDensity, triangleData);
            forceToAdd += ShipMath.ViscousWaterResistanceForce(waterDensity, triangleData, Cf);
            forceToAdd += ShipMath.PressureDragForce(triangleData);
            int originalTriangleIndex = indexOfOriginalTriagle[i];
            SlammingForceData slammingData = slammingForceData[originalTriangleIndex];

            forceToAdd += ShipMath.SlammingForce(slammingData, triangleData, boatArea, boatMass);

            shipRB.AddForceAtPosition(forceToAdd, triangleData.center, ForceMode.Force);

            //Normal
            //Debug.DrawRay(triangleData.center, triangleData.normal * 3f, Color.white);

            //Buoyancy
            //Debug.DrawRay(triangleData.center, buoyancyForce.normalized * -3f, Color.blue);
        }
    }
    void AddAboveWaterForces()
    {
        List<TriangleData> aboveWaterTriangleData = shipMath.aboveWaterTriangleData;

        //Loop through all triangles
        for (int i = 0; i < aboveWaterTriangleData.Count; i++)
        {
            TriangleData triangleData = aboveWaterTriangleData[i];


            //Calculate the forces
            Vector3 forceToAdd = Vector3.zero;

            //Force 1 - Air resistance 
            //Replace VisbyData.C_r with your boat's drag coefficient
            forceToAdd += ShipMath.AirResistanceForce(airDensity, triangleData, 0.8f); // replace hard val with 

            //Add the forces to the boat
            shipRB.AddForceAtPosition(forceToAdd, triangleData.center);


            //Debug

            //The normal
            //Debug.DrawRay(triangleCenter, triangleNormal * 3f, Color.white);

            //The velocity
            //Debug.DrawRay(triangleCenter, triangleVelocityDir * 3f, Color.black);

            if (triangleData.cosTheta > 0f)
            {
                //Debug.DrawRay(triangleCenter, triangleVelocityDir * 3f, Color.black);
            }
        }

    }
    private void CalculateSlammingVelocities(List<SlammingForceData> slammingForceData)
    {
        for (int i = 0; i < slammingForceData.Count; i++)
        {
            //Set the new velocity to the old velocity
            slammingForceData[i].previousVelocity = slammingForceData[i].velocity;

            //Center of the triangle in world space
            Vector3 center = transform.TransformPoint(slammingForceData[i].triangleCenter);

            //Get the current velocity at the center of the triangle
            slammingForceData[i].velocity = ShipMath.GetTriangleVelocity(shipRB, center);
        }
    }
}
