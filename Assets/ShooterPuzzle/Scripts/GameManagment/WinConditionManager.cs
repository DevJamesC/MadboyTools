using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinConditionManager : MonoBehaviour
{
    public static WinConditionManager instance;

    PuzzleShooterGameManager gameManager;
    public int LEVEL;
    public bool hasTargetScore;
    public int targetScore;
    public bool hasTimeLimit;
    public int timeLimit;

   public float currentScore;
    float currentTime;
    bool scoreConditionMet;
    bool timeConditionMet;
    bool levelCompleted;

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
       
        currentScore = 0;
        gameManager = PuzzleShooterGameManager.instance;
        timeConditionMet = true;
        if(hasTimeLimit)
        {
            currentTime = timeLimit;
        }

        scoreConditionMet = (hasTargetScore) ?false:true;
        levelCompleted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(hasTimeLimit)
        {
            if(currentTime<=0)
            {
                Debug.Log("Time's Up!");
                timeConditionMet = false;
            }
            else
            {
                currentTime -= Time.deltaTime;
            }
        }
    }

    public void AddScore(int amount)
    {
        currentScore += amount;

        if(hasTargetScore)
        {
            if(currentScore>=targetScore)
            {
                scoreConditionMet = true;
                CheckConditions();
            }
        }
    }

    void CheckConditions()
    {
        if(timeConditionMet&&scoreConditionMet&&!levelCompleted)
        {
            gameManager.CompletedLevel(LEVEL);
            gameManager.ReturnToLevelSelect();
            levelCompleted = true;
        }
    }

    public void LoseByNoAmmo()
    {
        
    }

    public void LoseByObjectiveFailed()
    {

    }
}
