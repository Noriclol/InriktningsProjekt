using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBuoyGerst : MonoBehaviour
{
    //Vector3 startPos;
    Vector3 startPos;
    
    private void Start()
    {
        startPos = transform.position;
        //gameObject.transform.position = startPos + WaveHandler.instance.CustomWaveFunc(startPos, GameManager.secondsSinceStart); //aproximated height of xy
    }
    void Update()
    {

        //startPos = transform.position;
        //startPos.y = 0.0f;
        //Debug.Log("before: " + startPos);
        gameObject.transform.position = new Vector3(transform.position.x, WaveHandler.instance.CustomWaveFunc3(startPos, GameManager.secondsSinceStart), transform.position.z);//WaveHandler.instance.CustomWaveFunc2(startPos, GameManager.secondsSinceStart); //aproximated height of xy
        //Debug.DrawLine(startPos, WaveHandler.instance.CustomWaveFunc(startPos, GameManager.secondsSinceStart), Color.cyan); //debug

        //startPos = new Vector3(transform.position.x, WaveHandler.instance.CustomWaveFunc3(startPos, GameManager.secondsSinceStart), transform.position.z); // approximated height of xy but with unmodified xy
        //transform.position = startPos;
        //Debug.Log("after: " + startPos);
    }
}
