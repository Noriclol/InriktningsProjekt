using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//controlls the wave material and retrieves important variables
public class WaveHandler : MonoBehaviour
{
    public static WaveHandler current;

    public bool isMoving;

    //WaveFunction GENERAL
    public float scale = 0.1f;
    public float speed = 1.0f;


    //waveFunctions GERSTNER
    //public List<Vector4> waves;

    //WaveFunctions SINE
    public float waveDistance = 1f;
    //WaveFunctions PERLIN
    public float noiseStrength = 1f;
    public float noiseWalk = 1f;

    void Start()
    {
        current = this;
    }


   
    void Update()
    {
        //Shader.SetGlobalFloat("_WaterTime", Time.time);
    }




    public float GetWaveYPos(Vector3 pos, float timeSinceStart)
    {
        if (isMoving)
        {
            return WaveTypes.SinXWave(pos, speed, scale, waveDistance, noiseStrength, noiseWalk, timeSinceStart);
        }
        else
        {
            return 0f;
        }
    }

    public float DistanceToSurface(Vector3 tPos, float timeSinceStart)
    {
        float waterHeight = GetWaveYPos(tPos, timeSinceStart);

        float distanceToWater = tPos.y - waterHeight;

        return distanceToWater;
    }
}
