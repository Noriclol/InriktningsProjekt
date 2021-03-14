using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingActor : MonoBehaviour
{
    //comment: you might need a rigidbody for hitdetection!!!!
    //comment: Try to enable the waterpatch once youve got the program working reasonably
    //public GameObject waterPatch; //deactivating waterpatch until further notice
    //Mesh patchMesh;

    //refs
    public GameObject underwaterObj;

    //vars
    private MeshSegmentation segmenter;
    private Mesh underwaterMesh;
    private Rigidbody shipRB;
    private float waterDensity = 1027f;

    void Start()
    { 
        shipRB = gameObject.GetComponent<Rigidbody>();
        segmenter = new MeshSegmentation(gameObject);
        underwaterMesh = underwaterObj.GetComponent<MeshFilter>().mesh;
    }

    void Update()
    {
        //UpdateGridPositionandRotation();
        //generate the underwater mesh
        //display the underwater mesh

        segmenter.GenerateUnderwaterMesh();
        segmenter.DisplayMesh(underwaterMesh, "Underwater Mesh", segmenter.underWaterTriangleData);
    }
    private void FixedUpdate()
    {
        //Debug.Log(segmenter.underWaterTriangleData.Count);
        //Add forces to the part of the boat that's below the water
        if (segmenter.underWaterTriangleData.Count > 0)
        {
            AddUnderWaterForces();
        }
    }


    void AddUnderWaterForces() {
        List<TriangleData> underwaterTriangleData = segmenter.underWaterTriangleData;

        for (int i = 0; i < underwaterTriangleData.Count; i++)
        {
            TriangleData triangleData = underwaterTriangleData[i];

            Vector3 buoyancyForce = BuoyancyForce(waterDensity, triangleData);

            shipRB.AddForceAtPosition(buoyancyForce, triangleData.center);

            //Normal
            Debug.DrawRay(triangleData.center, triangleData.normal * 3f, Color.white);

            //Buoyancy
            Debug.DrawRay(triangleData.center, buoyancyForce.normalized * -3f, Color.blue);
        }
    }
    private Vector3 BuoyancyForce(float waterDensity, TriangleData triangleData)
    {

        Vector3 buoyancyForce = waterDensity * Physics.gravity.y * triangleData.distanceToSurface * triangleData.area * triangleData.normal;

        buoyancyForce.x = 0f;
        buoyancyForce.z = 0f;

        return buoyancyForce;
    }
}

//function for aligning the ship waterpatch grid
//void updategridpositionandrotation() 
//{
//    vector3 newpos = new vector3(ship.transform.position.x, 0, ship.transform.position.z);
//    quaternion newrot = new quaternion(0, ship.transform.rotation.y, 0, ship.transform.rotation.w);
//    waterpatch.transform.setpositionandrotation(newpos, newrot);



//}

// function for going through the watermesh grid to apply height on the waterpatch
//void updategridheight(vector3 p, vector2 g)
//{
//    for (int i = 0; i < patchmesh.vertexcount; i++)
//    {
//        //update the grid height by sampling the water shader
//        //behöver hjälp från philip/smörgås med detta, vet inte hur jag samplar
//    }
//}