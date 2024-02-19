using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Start")] 
    [SerializeField] private Light _light;
    [SerializeField] private float lightIntensityRate;
    [SerializeField] private float lightIntensityMax;
    [SerializeField] private float lightIntensityMin;
    public bool gameStart = false;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //set light intensity
        if (gameStart)
        {
            if (_light.intensity < lightIntensityMax)
            {
                _light.intensity += lightIntensityRate * Time.deltaTime;
            }

            if (_light.intensity >= lightIntensityMax)
            {
                _light.intensity = lightIntensityMax;
            }
        }
        else
        {
            if (_light.intensity > lightIntensityMin)
            {
                _light.intensity -= lightIntensityRate * Time.deltaTime;
            }

            if (_light.intensity <= lightIntensityMin)
            {
                _light.intensity = lightIntensityMin;
            }
        }
        
        //game timer
    }
}
