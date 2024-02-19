using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Windows;
using Random = System.Random;
using System.IO;
using Directory = System.IO.Directory;
using File = System.IO.File;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Start")] 
    [SerializeField] private Light _light;
    [SerializeField] private float lightIntensityRate;
    [SerializeField] private float lightIntensityMax;
    [SerializeField] private float lightIntensityMin;
    public bool gameStart = false;
    [SerializeField] private AudioClip _clip;
    private AudioSource _audioSource;
    [SerializeField] private float twoSecCountDown = 3;
    public float gameCountDown = 10;

    [Header("Targets")] 
    [SerializeField] private GameObject targets;
    [SerializeField] private LayerMask targetsMask;
    [SerializeField] bool hasSpawned = false;

    [Header("Score")] 
    public TextMeshProUGUI display;     //get ui text
    public int score;
    public int finalScore;
    private const string FILE_DIR = "/DATA/";       //set path
    private const string DATA_FILE = "highScores.txt";
    private string FILE_FULL_PATH;
    public bool startedOnce = false;
    public bool storeHighScore = false;
    private bool restartScore = false;

    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;

            // if (isHighScore(score))     //only proceed if score is high score
            // {
            //     int highScoreSlot = -1;     //set the slot that is going to be replaced
            //
            //     for (int i = 0; i < HighScores.Count; i++)      //for each high score in the list
            //     {
            //         if (score > highScores[i])
            //         {
            //             highScoreSlot = i;      //get the slot that is going to be replaced
            //             break;
            //         }
            //     }
            //
            //     highScores.Insert(highScoreSlot, score);    //replace the score
            //
            //     highScores = highScores.GetRange(0, 5);     //returns the new list
            //
            //     string scoreBoardText = "";
            //
            //     foreach (var highScore in highScores)
            //     {
            //         scoreBoardText += highScore + "\n";
            //     }
            //
            //     highScoresString = scoreBoardText;
            //     
            //     File.WriteAllText(FILE_FULL_PATH, highScoresString);
            // }
        }
    }

    public int FinalScore
    {
        get
        {
            return finalScore;
        }
        set
        {
            finalScore = value;

            if (isHighScore(finalScore)) //only proceed if score is high score
            {
                int highScoreSlot = -1; //set the slot that is going to be replaced

                for (int i = 0; i < HighScores.Count; i++) //for each high score in the list
                {
                    if (finalScore > highScores[i])
                    {
                        highScoreSlot = i; //get the slot that is going to be replaced
                        break;
                    }
                }

                highScores.Insert(highScoreSlot, finalScore); //replace the score

                highScores = highScores.GetRange(0, 3); //returns the new list

                string scoreBoardText = "";

                foreach (var highScore in highScores)
                {
                    scoreBoardText += highScore + "\n";
                }

                highScoresString = scoreBoardText;

                File.WriteAllText(FILE_FULL_PATH, highScoresString);
            }
        }
    }
    
    private string highScoresString = "";

    private List<int> highScores;

    public List<int> HighScores
    {
        get
        {
            if (highScores == null)
            {
                highScores = new List<int>();       //create new list

                highScoresString = File.ReadAllText(FILE_FULL_PATH);

                highScoresString = highScoresString.Trim();

                string[] highScoreArray = highScoresString.Split("\n");

                for (int i = 0; i < highScoreArray.Length; i++)
                {
                    int currentScore = Int32.Parse(highScoreArray[i]);
                    highScores.Add(currentScore);
                }
            }
            return highScores;
        } 
    }
    
    
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
        
        startedOnce = false;
        _audioSource = GetComponent<AudioSource>();
        FILE_FULL_PATH = Application.dataPath + FILE_DIR + DATA_FILE;
    }

    // Update is called once per frame
    void Update()
    {
        //start 3 sec count down
        if (gameStart)
        {
            storeHighScore = false;
            twoSecCountDown -= Time.deltaTime;
            if (twoSecCountDown <= 0)
            {
                twoSecCountDown = 0;
            }

            if (!restartScore)
            {
                Score = 0;
                restartScore = true;
            }
        }
        else
        {
            if (!storeHighScore)
            {
                FinalScore = Score;
                storeHighScore = true;
            }
            twoSecCountDown = 3;
            gameCountDown = 10;
            hasSpawned = false;
            //discard all targets
            DeleteTargets();
            restartScore = false;
        }
        
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
        
        //game timer && startup sfx
        if (gameStart && !_audioSource.isPlaying)
        {
            _audioSource.PlayOneShot(_clip);
        }
        //game start after 3 sec && game over after 10 sec
        if (gameStart && twoSecCountDown == 0)
        {
            //game start count down for 10 sec
            gameCountDown -= Time.deltaTime;
            if (gameCountDown <= 0)
            {
                gameCountDown = 0;
            }
            //spawn targets
            if (!hasSpawned)
            {
                Instantiate(targets, new Vector3(UnityEngine.Random.Range(-14.5f, 14.5f), UnityEngine.Random.Range(.5f, 5.5f), UnityEngine.Random.Range(-14.5f, 14.5f)), quaternion.identity);
                Instantiate(targets, new Vector3(UnityEngine.Random.Range(-14.5f, 14.5f), UnityEngine.Random.Range(.5f, 5.5f), UnityEngine.Random.Range(-14.5f, 14.5f)), quaternion.identity);
                Instantiate(targets, new Vector3(UnityEngine.Random.Range(-14.5f, 14.5f), UnityEngine.Random.Range(.5f, 5.5f), UnityEngine.Random.Range(-14.5f, 14.5f)), quaternion.identity);
                hasSpawned = true;
            }
        }
        if (gameCountDown == 0)
        {
            gameStart = false;
        }
        
        //score
        if (gameStart)
        {
            display.text = "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + ""+ score + "";
        }
        else
        {
            if (startedOnce)
            {
                display.text = "FINAL SCORE: "+ "\n" + score + "\n" + "\nHIGH SCORES: \n" + highScoresString;
            }
        }
    }

    bool isHighScore(int score) //check if current socre is highscore
    {
        for (int i = 0; i < HighScores.Count; i++)
        {
            if (highScores[i] < score)  //any of the high score is lower than current score
            {
                return true;
            }
        }

        return false;
    }

    void DeleteTargets()
    {
        GameObject[] targetsToDelete = GameObject.FindGameObjectsWithTag("targets");

        foreach (GameObject obj in targetsToDelete)
        {
            Destroy(obj);
        }
    }
}
