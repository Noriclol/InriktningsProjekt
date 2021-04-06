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
    public Vector4 waveA = new Vector4(0.72f, 0.25f, 0.3f, 100f);
    public Vector4 waveB = new Vector4(0.57f, 0.56f, 0.25f, 200f);
    public Vector4 waveC = new Vector4(0.12f, 0.95f, 0.15f, 100f);

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
        Shader.SetGlobalVector("_WaveA", waveA);
        Shader.SetGlobalVector("_WaveB", waveB);
        Shader.SetGlobalVector("_WaveC", waveC);
    }

    public float GetWaveYPos(Vector3 position, float timeSinceStart)
    {
        return CustomWaveFunc3(position, timeSinceStart);
        //return SinXWave(position, speed, scale, waveDistance, noiseStrength, noiseWalk, timeSinceStart);
    }
    public float DistanceToWater(Vector3 position, float timeSinceStart)
    {
        float waterHeight = GetWaveYPos(position, timeSinceStart);

        float distanceToWater = position.y - waterHeight;
        //distanceToWater = Mathf.Abs(distanceToWater);
        Debug.DrawLine(position, new Vector3(position.x, waterHeight, position.z), Color.red);
        //Debug.DrawLine(position, new Vector3(position.x, distanceToWater, position.z), Color.blue);
        return distanceToWater;
    }

    // Wave Functions

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
        //y += Mathf.PerlinNoise(x + noiseWalk, y + Mathf.Sin(timeSinceStart * 0.1f)) * noiseStrength;

        return y;
    }

    private static Vector3 GerstnerWave(Vector4 waveData, Vector3 pos, float timeSinceStart)
    {
        //steepness&wavelength
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

    //customGerstnerWavemult

    public Vector3 CustomWaveFunc(Vector3 pos, float timeSinceStart) //returns modified xyz for given xyz (original wave modifier)
    {
        //pos.y = 0;
        Vector3 p = pos;
        Vector3 originalPoint = pos;

        p += GerstnerWave(waveA, originalPoint, timeSinceStart);
        p += GerstnerWave(waveB, originalPoint, timeSinceStart);
        p += GerstnerWave(waveC, originalPoint, timeSinceStart);

        Debug.DrawLine(originalPoint, p, Color.red);
        Debug.DrawLine(originalPoint, new Vector3(p.x, originalPoint.y, p.z), Color.green);
        Debug.DrawLine(new Vector3(p.x, originalPoint.y, p.z), p, Color.blue);

        return p;
    }

    public Vector3 CustomWaveFunc2(Vector3 pos, float timeSinceStart) //returns aproximated xyz for xy
    {
        //pos.y = 0;

        //original
        Vector3 p1 = new Vector3();
        Vector3 originalPoint = pos;
        p1 += originalPoint + GerstnerWave(waveA, originalPoint, timeSinceStart);
        p1 += GerstnerWave(waveB, originalPoint, timeSinceStart);
        p1 += GerstnerWave(waveC, originalPoint, timeSinceStart);

        //original triangle
        Debug.DrawLine(originalPoint, p1, Color.red);
        Debug.DrawLine(originalPoint, new Vector3(p1.x, originalPoint.y, p1.z), Color.red);
        Debug.DrawLine(new Vector3(p1.x, originalPoint.y, p1.z), p1, Color.red);

        //calculation
        Vector3 start1 = originalPoint;
        Vector3 end1 = new Vector3(p1.x, originalPoint.y, p1.z);
        Vector3 length1 = start1 - end1;
        //debug
        //Debug.DrawLine(start1, length1, Color.white);
        //Debug.DrawRay(end1, Vector3.Normalize(start1 - end1));


        //third iteration
        Vector3 start2 = start1 - length1;
        Vector3 p2 = start2 + GerstnerWave(waveA, start2, timeSinceStart);
        p2 += GerstnerWave(waveB, start2, timeSinceStart);
        p2 += GerstnerWave(waveC, start2, timeSinceStart);
        Vector3 end2 = new Vector3(p2.x, originalPoint.y, p2.z);
        //debug
        Debug.DrawLine(start2, p2, Color.green);
        Debug.DrawLine(start2, end2, Color.green);
        Debug.DrawLine(end2, p2, Color.green);

        //third iteration
        Vector3 delta2 = end2 - originalPoint;
        Vector3 start3 = start2 - delta2;
        Vector3 p3 = start3 + GerstnerWave(waveA, start3, timeSinceStart);
        p3 += GerstnerWave(waveB, start3, timeSinceStart);
        p3 += GerstnerWave(waveC, start3, timeSinceStart);
        Vector3 end3 = new Vector3(p3.x, originalPoint.y, p3.z);
        //debug
        //Debug.DrawLine(start3, p3, Color.red);
        //Debug.DrawLine(start3, end3, Color.red);
        //Debug.DrawLine(end3, p3, Color.red);

        //fourth iteration
        Vector3 delta3 = end3 - originalPoint;
        Vector3 start4 = start3 - delta3;
        Vector3 p4 = start4 + GerstnerWave(waveA, start4, timeSinceStart);
        p4 += GerstnerWave(waveB, start4, timeSinceStart);
        p4 += GerstnerWave(waveC, start4, timeSinceStart);
        Vector3 end4 =new Vector3(p4.x, originalPoint.y, p4.z);
        //debug
        //Debug.DrawLine(start4, p4, Color.blue);
        //Debug.DrawLine(start4, end4, Color.blue);
        //Debug.DrawLine(end4, p4, Color.blue);

        //fifth iteration
        Vector3 delta4 = end4 - originalPoint;
        Vector3 start5 = start4 - delta4;
        Vector3 p5 = start5 + GerstnerWave(waveA, start5, timeSinceStart);
        p5 += GerstnerWave(waveB, start5, timeSinceStart);
        p5 += GerstnerWave(waveC, start5, timeSinceStart);
        //Vector3 end5 = originalPoint + new Vector3(p5.x, originalPoint.y, p5.z);
        //debug
        //Debug.DrawLine(start5, p5, Color.cyan);
        //Debug.DrawLine(start5, end5, Color.cyan);
        //Debug.DrawLine(end5, p5, Color.cyan);
        //p5 += originalPoint;
        //return 5th iteration
        //p5 += originalPoint;
        return p5;
    }

    public float CustomWaveFunc3(Vector3 pos, float timeSinceStart) //returns only height of given xy
    {
        //original
        Vector3 p1 = new Vector3();
        Vector3 originalPoint = pos;
        p1 += originalPoint + GerstnerWave(waveA, originalPoint, timeSinceStart);
        //p1 += GerstnerWave(waveB, originalPoint, timeSinceStart);
        //p1 += GerstnerWave(waveC, originalPoint, timeSinceStart);

        //original triangle
        //Debug.DrawLine(originalPoint, p1, Color.red);
        //Debug.DrawLine(originalPoint, new Vector3(p1.x, originalPoint.y, p1.z), Color.red);
        //Debug.DrawLine(new Vector3(p1.x, originalPoint.y, p1.z), p1, Color.red);

        //calculation
        Vector3 start1 = originalPoint;
        Vector3 end1 = new Vector3(p1.x, originalPoint.y, p1.z);
        Vector3 length1 = start1 - end1;
        //debug
        //Debug.DrawLine(start1, length1, Color.white);
        //Debug.DrawRay(end1, Vector3.Normalize(start1 - end1));


        //third iteration
        Vector3 start2 = start1 - length1;
        Vector3 p2 = start2 + GerstnerWave(waveA, start2, timeSinceStart);
        //p2 += GerstnerWave(waveB, start2, timeSinceStart);
        //p2 += GerstnerWave(waveC, start2, timeSinceStart);
        Vector3 end2 = new Vector3(p2.x, originalPoint.y, p2.z);
        //debug
        //Debug.DrawLine(start2, p2, Color.green);
        //Debug.DrawLine(start2, end2, Color.green);
        //Debug.DrawLine(end2, p2, Color.green);

        //third iteration
        Vector3 delta2 = end2 - originalPoint;
        Vector3 start3 = start2 - delta2;
        Vector3 p3 = start3 + GerstnerWave(waveA, start3, timeSinceStart);
        //p3 += GerstnerWave(waveB, start3, timeSinceStart);
        //p3 += GerstnerWave(waveC, start3, timeSinceStart);
        Vector3 end3 = new Vector3(p3.x, originalPoint.y, p3.z);
        //debug
        //Debug.DrawLine(start3, p3, Color.red);
        //Debug.DrawLine(start3, end3, Color.red);
        //Debug.DrawLine(end3, p3, Color.red);

        //fourth iteration
        Vector3 delta3 = end3 - originalPoint;
        Vector3 start4 = start3 - delta3;
        Vector3 p4 = start4 + GerstnerWave(waveA, start4, timeSinceStart);
        //p4 += GerstnerWave(waveB, start4, timeSinceStart);
        //p4 += GerstnerWave(waveC, start4, timeSinceStart);
        Vector3 end4 = new Vector3(p4.x, originalPoint.y, p4.z);
        //debug
        //Debug.DrawLine(start4, p4, Color.blue);
        //Debug.DrawLine(start4, end4, Color.blue);
        //Debug.DrawLine(end4, p4, Color.blue);

        //fifth iteration
        Vector3 delta4 = end4 - originalPoint;
        Vector3 start5 = start4 - delta4;
        Vector3 p5 = start5 + GerstnerWave(waveA, start5, timeSinceStart);
        //p5 += GerstnerWave(waveB, start5, timeSinceStart);
        //p5 += GerstnerWave(waveC, start5, timeSinceStart);
        //Vector3 end5 = originalPoint + new Vector3(p5.x, originalPoint.y, p5.z);
        //debug
        //Debug.DrawLine(start5, p5, Color.cyan);
        //Debug.DrawLine(start5, end5, Color.cyan);
        //Debug.DrawLine(end5, p5, Color.cyan);
        //p5 += originalPoint;
        //return 5th iteration
        //p5 += originalPoint;
        return p5.y;
    }
}
