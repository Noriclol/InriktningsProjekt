using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBuoyDistance : MonoBehaviour
{

    Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 distance = new Vector3(transform.position.x, WaveHandler.instance.DistanceToWater(startPos, GameManager.secondsSinceStart), transform.position.z);
        //gameObject.transform.position = new Vector3(transform.position.x, WaveHandler.instance.DistanceToWater(startPos, GameManager.secondsSinceStart), transform.position.z);
        //Debug.DrawLine(startPos, new Vector3(position.x, waterHeight  , position.z), Color.red);
        //Debug.DrawLine(startPos, transform.position, Color.yellow);
    }
}

