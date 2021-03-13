using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//controlls the wave material and retrieves important variables
public class WaveHandler : MonoBehaviour
{
    public static WaveHandler current;

    //waveFunctions
    public List<Vector4> waves;


    // Start is called before the first frame update
    void Start()
    {
        current = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
