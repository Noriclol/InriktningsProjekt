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

    private void Start()
	{
        rb = gameObject.GetComponent<Rigidbody>();
	}





}




