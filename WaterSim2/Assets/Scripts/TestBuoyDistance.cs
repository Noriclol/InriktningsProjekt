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
        gameObject.transform.position = new Vector3(transform.position.x, WaveHandler.instance.DistanceToWater(startPos, GameManager.secondsSinceStart), transform.position.z);
    }
}
