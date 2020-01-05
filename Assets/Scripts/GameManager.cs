using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public bool startPlaying;

    public NoteScroll theBS;

    public static GameManager instance;

    public int currentScore;
    public int SCORE_VALUE = 300;

    public int currentMultiplier;
    public int multiplierTracker;
    public int[] multiplierThresholds;
    public int combo;

    public Text scoreText;
    public Text multiText;
    public Text comboText;

    public float totalNotes;
    public float goodHits;
    public float missHits;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        scoreText.text = "0";
        combo = 0;
        comboText.text = "";
        currentMultiplier = 1;

        totalNotes = FindObjectsOfType<NoteObject>().Length;
        totalNotes += FindObjectsOfType<BigNoteObject>().Length;
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

        if (currentMultiplier - 1 < multiplierThresholds.Length)
        {
            multiplierTracker++;

            if (multiplierThresholds[currentMultiplier - 1] <= multiplierTracker)
            {
                multiplierTracker = 0;
                currentMultiplier++;
            }
        }

        currentScore += (SCORE_VALUE + (96 * (currentMultiplier - 1)));
        scoreText.text = "" + currentScore;
    }

    /*public void NormalHit()
    {
        currentScore += (SCORE_VALUE + (96 * (currentMultiplier - 1)));
        NoteHit();
        goodHits++;
    }*/


    public void NoteMissed()
    {
        Debug.Log("Miss");

        combo = 0;
        currentMultiplier = 1;
        comboText.text = "";

        missHits++;

    }
}