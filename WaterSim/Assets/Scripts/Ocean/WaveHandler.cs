using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//controlls the wave material and retrieves important variables
public class WaveHandler : MonoBehaviour
{
    public static WaveHandler current;

    public GameObject waterObj;
    public bool isMoving;
    public int waveSelection;

    public float scale = 0.1f;
    public float speed = 1.0f;
    public float waveDistance = 1f;
    public float noiseStrength = 1f;
    public float noiseWalk = 1f;

    public Vector4 waveA = new Vector4(1f, 0f, 0.5f, 100f);
    public Vector4 waveB = new Vector4(0f, 1f, 0.25f, 200f);
    public Vector4 waveC = new Vector4(1f, 1f, 0.15f, 100f);

    //WaveFunctions SINE
    


    void Start()
    {
        current = this;
        
    }


   
    void Update()
    {
        //Shader.SetGlobalFloat("_WaveTime", Time.time);
        //Shader.SetGlobalVector("_WaveA", waveA);
        //Shader.SetGlobalVector("_WaveB", waveA);
        //Shader.SetGlobalVector("_WaveC", waveA);
    }


    private Vector3 GerstnerWave(Vector4 wave, Vector3 p, float timeSinceStart)
    {
        float steepness = wave.z;
        float wavelength = wave.w;
        float k = 2 * Mathf.PI / wavelength;
        float c = Mathf.Sqrt((float)9.8 / k);
        Vector2 d = new Vector2(wave.x, wave.y).normalized;
        float f = k * (Vector2.Dot(d, new Vector2(p.x, p.z)) - c * timeSinceStart);
        float a = steepness / k;
        return new Vector3(
                d.x * (a * Mathf.Cos(f)),
                a * Mathf.Sin(f),
                d.y * (a * Mathf.Cos(f))
                );
    }

    public float CustomWaveFunc(Vector3 pos, float timeSinceStart)
    {
        Vector3 p = pos;
        p += GerstnerWave(waveA, pos, timeSinceStart);
        p += GerstnerWave(waveB, pos, timeSinceStart);
        p += GerstnerWave(waveC, pos, timeSinceStart);
        //Vector2 normal = (Vector3.Cross(binormal, tangent).normalized);
        //p = p.normalized;
        return p.y;
    }

    public float GetWaveYPos(Vector3 pos, float timeSinceStart)
    {
        if (isMoving)
        {
            if (waveSelection == 0)
            {
                return WaveTypes.SinXWave(pos, speed, scale, waveDistance, noiseStrength, noiseWalk, timeSinceStart);
            }

            else if (waveSelection == 1)
            {
                return CustomWaveFunc(pos, timeSinceStart);
            }
            else { return 0f; }
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
