using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GerstLine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, WaveHandler.instance.CustomWaveFunc(transform.position, GameManager.secondsSinceStart), Color.red); //debug
    }
}
