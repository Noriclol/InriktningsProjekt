using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuoyancyScript : MonoBehaviour
{
    public GameObject underWaterObj;
    private ShipMath shipMath;
    private Mesh underWaterMesh;
    private Rigidbody shipRB;
    private float waterDensity = 1027f;

    void Start()
    {
        shipMath = new ShipMath();
        shipRB = gameObject.GetComponent<Rigidbody>();
        shipMath.InitTriCutAlgoritm(gameObject);
        underWaterMesh = underWaterObj.GetComponent<MeshFilter>().mesh;
    }

    void Update()
    {
        shipMath.GenerateUnderwaterMesh();
        shipMath.DisplayMesh(underWaterMesh, "Underwater Mesh", shipMath.underWaterTriangleData);
    }

    private void FixedUpdate()
    {
        if (shipMath.underWaterTriangleData.Count > 0)
        {
            AddUnderWaterForces();
        }
        //Debug.Log("Triangles Under Water: " + shipMath.underWaterTriangleData.Count);
    }
    void AddUnderWaterForces()
    {
        List<TriangleData> underWaterTriangleData = shipMath.underWaterTriangleData;
        //loop through all triangles and apply forces
        for (int i = 0; i < underWaterTriangleData.Count; i++)
        {
            TriangleData triangleData = underWaterTriangleData[i];
            Vector3 buoyancyForce = BuoyancyForce(waterDensity, triangleData);

            shipRB.AddForceAtPosition(buoyancyForce, triangleData.center, ForceMode.Force);

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

        //buoyancyForce.x = 0f;
        //buoyancyForce.z = 0f;

        return buoyancyForce;
    }
}
