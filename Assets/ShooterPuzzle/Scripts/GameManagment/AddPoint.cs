using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class AddPoint : MonoBehaviour
{
    public bool addPointOnDeath;
    public int pointsForDeath;
    public bool addPointOnDamage;
    public int pointsForDamage;

    Health health;
    WinConditionManager gameScoreManager;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
        gameScoreManager = WinConditionManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(addPointOnDamage)
        {
            if(health.tookDamage)
            {
                gameScoreManager.AddScore(pointsForDamage);
            }
        }
        if(addPointOnDeath)
        {
            if(health.healthData.isDead)
            {
                gameScoreManager.AddScore(pointsForDeath);
            }
        }
    }
}
