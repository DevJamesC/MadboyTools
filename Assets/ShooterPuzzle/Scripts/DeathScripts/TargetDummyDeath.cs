using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDummyDeath : DeathMain
{


    // Update is called once per frame
    public void LateUpdate()
    {
       

        if(isDead)
        {
            RespawnOnDeath(1);
            
        }
    }

    void RespawnOnDeath(float timeToRespawn)
    {
       Invoke("Respawn",timeToRespawn);
        gameObject.SetActive(false);
        ResetHealth();
    }

    void Respawn()
    {
        gameObject.SetActive(true);
    }
}
