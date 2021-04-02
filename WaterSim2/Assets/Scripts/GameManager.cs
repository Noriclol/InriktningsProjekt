using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameManager instance;
    public static float secondsSinceStart;

    public GameObject oceanHandlerObj;
    public GameObject player;

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
        GameObject oHandlerObj = Instantiate(oceanHandlerObj, oceanHandlerObj.transform.position, oceanHandlerObj.transform.rotation);
        oHandlerObj.SetActive(true);
        oHandlerObj.transform.parent = transform;
        oHandlerObj.GetComponent<OceanHandler>().playerObj = player;


    }
    private void Update()
    {
        secondsSinceStart = Time.time;
    }
    //private void SpawnOcean();
}

