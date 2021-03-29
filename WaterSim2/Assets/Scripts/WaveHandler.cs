using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveHandler : MonoBehaviour
{
    //Singleton
    /*
     Handles the wave options / parameters
     
     */
    //WATER FIELDS
    public bool isMoving = true;
    public int waveType = 0;
    //Wave height and speed
    public float scale = 0.4f;
    public float speed = 1.0f;
    //The width between the waves
    public float waveDistance = 1f;
    //Noise parameters
    public float noiseStrength = 1f;
    public float noiseWalk = 1f;

    //GerstnerWaves
    public Vector4 waveA = new Vector4(1f, 0f, 0.5f, 100f);
    public Vector4 waveB = new Vector4(0f, 1f, 0.25f, 200f);
    public Vector4 waveC = new Vector4(1f, 1f, 0.15f, 100f);

    public static WaveHandler instance;
    void Start()
    {
        instance = this;
    }

    void Update()
    {
        //wave fields
        Shader.SetGlobalInt("_WaveType", waveType);
        Shader.SetGlobalFloat("_WaterTime", Time.time);

        //general fields
        Shader.SetGlobalFloat("_WaterScale", scale);
        Shader.SetGlobalFloat("_WaterSpeed", speed);
        Shader.SetGlobalFloat("_WaterDistance", waveDistance);
        
        //noise Fields
        Shader.SetGlobalFloat("_WaterNoiseStrength", noiseStrength);
        Shader.SetGlobalFloat("_WaterNoiseWalk", noiseWalk);
        //Wave Modifiers

    }

    public float GetWaveYPos(Vector3 position, float timeSinceStart)
    {
        if (isMoving && waveType == 0)
        {
            return SinXWave(position, speed, scale, waveDistance, noiseStrength, noiseWalk, timeSinceStart);
        }
        else if (isMoving && waveType == 1)
        {
            return CustomWaveFunc(position, timeSinceStart);
        }
        else
        {
            return 0f;
        }
    }
    public float DistanceToWater(Vector3 position, float timeSinceStart)
    {
        float waterHeight = GetWaveYPos(position, timeSinceStart);

        float distanceToWater = position.y - waterHeight;

        return distanceToWater;
    }

    // Wave Functions

    private static Vector3 GerstnerWave(Vector4 waveData, Vector3 pos, float timeSinceStart)
    {
        float steepness = waveData.z;
        float wavelength = waveData.w;
        float k = 2 * Mathf.PI / wavelength;
        float c = Mathf.Sqrt((float)9.8 / k);
        Vector2 d = new Vector2(waveData.x, waveData.y).normalized;
        float f = k * (Vector2.Dot(d, new Vector2(pos.x, pos.z)) - c * timeSinceStart);
        float a = steepness / k;
        return new Vector3(
                d.x * (a * Mathf.Cos(f)),
                a * Mathf.Sin(f),
                d.y * (a * Mathf.Cos(f))
                );
    }

    //Sinus waves
    public static float SinXWave(Vector3 position, float speed, float scale, float waveDistance, float noiseStrength, float noiseWalk, float timeSinceStart)
    {
        float x = position.x;
        float y = 0f;
        float z = position.z;

        //Using only x or z will produce straight waves
        //Using only y will produce an up/down movement
        //x + y + z rolling waves
        //x * z produces a moving sea without rolling waves

        float waveType = z;

        y += Mathf.Sin((timeSinceStart * speed + waveType) / waveDistance) * scale;

        //Add noise to make it more realistic
        y += Mathf.PerlinNoise(x + noiseWalk, y + Mathf.Sin(timeSinceStart * 0.1f)) * noiseStrength;

        return y;
    }

    //customGerstnerWavemult

    public float CustomWaveFunc(Vector3 pos, float timeSinceStart)
    {
        Vector3 p = pos;
        pos.y = 0;
        p += GerstnerWave(waveA, pos, timeSinceStart);
        p += GerstnerWave(waveB, pos, timeSinceStart);
        p += GerstnerWave(waveC, pos, timeSinceStart);
        //Vector2 normal = (Vector3.Cross(binormal, tangent).normalized);
        //p = p.normalized;
        return p.y;
    }

}
