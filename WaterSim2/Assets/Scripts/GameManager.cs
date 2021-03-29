using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameManager instance;
    public static float secondsSinceStart;

    public GameObject oceanHandlerObj;

    WaveHandler waveHandler;
    OceanHandler oceanHandler;
    private void Start()
    {
        if(instance)
        {
            Destroy(instance.gameObject);
            Debug.Log("GameManager Instance found and destroyed");
        }
        instance = this;
        secondsSinceStart = Time.time;
        oceanHandler = oceanHandlerObj.GetComponent<OceanHandler>();
        waveHandler = gameObject.GetComponent<WaveHandler>();


        //Instantiate oceanhandler
        GameObject waterHandlerObj = Instantiate(oceanHandlerObj, oceanHandlerObj.transform.position, oceanHandlerObj.transform.rotation);
        waterHandlerObj.SetActive(true);
        waterHandlerObj.transform.parent = transform;


    }
    private void Update()
    {
        secondsSinceStart = Time.time;
    }
    //private void SpawnOcean();
}

