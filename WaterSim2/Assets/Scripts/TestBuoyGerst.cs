using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBuoyGerst : MonoBehaviour
{
    Transform origo;

    private void Start()
    {
        //
        origo = transform;
    }
    void Update()
    {
        origo.position = new Vector3(0,0,0);
        //var newWaveHeight = WaveHandler.instance.CustomWaveFunc2Reverse(transform.position, GameManager.secondsSinceStart);
        //var buoyCurrentPosition = transform.position;
        //Vector3 nextBuoyPosition = new Vector3(buoyCurrentPosition.x,buoyCurrentPosition.y + (newWaveHeight.y-buoyCurrentPosition.y),buoyCurrentPosition.z);
        //Debug.Log("X: " + origo.position.x + "Z: " + origo.position.z);
        //transform.position = nextBuoyPosition;
        //transform.position = new Vector3(transform.position.x, WaveHandler.instance.CustomWaveFunc(transform.position, GameManager.secondsSinceStart), transform.position.z);
        transform.position = WaveHandler.instance.CustomWaveFunc2(origo.position, GameManager.secondsSinceStart);
        //transform.position = WaveHandler.instance.CustomWaveFunc2Reverse(transform.position, GameManager.secondsSinceStart);

    }
}
