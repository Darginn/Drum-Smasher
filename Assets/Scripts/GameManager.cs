using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Placeholder boolean
    public bool startPlaying;

    public NoteScroll theBS;

    public static GameManager instance;

    public int currentScore;
    public const int SCORE_VALUE = 300;

    public int multiplierValue;
    public int multiplierTracker;
    public int[] multiplierThresholds;
    public int combo;

    public Text scoreText;
    public Text multiText;
    public Text comboText;

    public float totalNotes;
    //public float goodHits;
    public float missHits;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        scoreText.text = "0";
        combo = 0;
        comboText.text = "";
        multiplierValue = 1;

        totalNotes = FindObjectsOfType<NoteObject>().Length;
    }

    // Update is called once per frame
    void Update()
    {
        if(!startPlaying)
        {
            startPlaying = true;
            theBS.gameStart = true;
        }
    }

    public void NoteHit()
    {
        //Debug.Log("Hit");

        combo++;
        totalNotes++;
        comboText.text = "" + combo;

        if (multiplierValue - 1 < multiplierThresholds.Length)
        {
            multiplierTracker++;

            if (multiplierThresholds[multiplierValue - 1] <= multiplierTracker)
            {
                multiplierTracker = 0;
                multiplierValue++;
            }
        }

        currentScore += (SCORE_VALUE + (96 * (multiplierValue - 1)));
        scoreText.text = "" + currentScore;
    }

    /*public void NormalHit()
    {
        currentScore += (SCORE_VALUE + (96 * (multiplierValue - 1)));
        NoteHit();
        goodHits++;
    }*/


    public void NoteMissed()
    {
        Debug.Log("Miss");

        combo = 0;
        multiplierValue = 1;
        comboText.text = "";

        missHits++;

    }
}