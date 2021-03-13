using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//controlls the wave material and retrieves important variables
public class WaveHandler : MonoBehaviour
{
    public static WaveHandler current;

    //waveFunctions
    public List<Vector4> waves;

    void Start()
    {
        current = this;
    }
   
    void Update()
    {
        Shader.SetGlobalFloat("_WaterTime", Time.time);
    }
    public float GetWaveYPos(Vector3 pos, float timeSinceStart)
    {
        return 0f;
    }

    public float DistanceToSurface(Vector3 tPos, float timeSinceStart)
    {
        float waterHeight = GetWaveYPos(tPos, timeSinceStart);

        float distanceToWater = tPos.y - waterHeight;

        return distanceToWater;
    }
}
