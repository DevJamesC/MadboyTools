using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMain : MonoBehaviour
{
    // Start is called before the first frame update
    Health health;
    public bool isDead;
    public virtual void Start()
    {
        health = GetComponent<Health>();
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if(health.healthData.isDead)
        {
            isDead = true;
        }
    }

    public void ResetHealth()
    {
        isDead = false;
        health.healthData.Reset();
    }
}
