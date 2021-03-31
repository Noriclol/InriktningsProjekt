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
        //transform.position = WaveHandler.instance.CustomWaveFunc2(origo.position, GameManager.secondsSinceStart); //aproximated height of xy
        transform.position = new Vector3(0, WaveHandler.instance.CustomWaveFunc3(origo.position, GameManager.secondsSinceStart), 0); // approximated height of xy but with unmodified xy
    }
}
