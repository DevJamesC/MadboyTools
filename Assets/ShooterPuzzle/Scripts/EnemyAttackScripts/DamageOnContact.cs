using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnContact : MonoBehaviour
{
    public LayerMask collisionLayers;
    public float damageTickRateOnContactStay;
    public DamageData damageData;

    
    float timeTillNextTick;

    Dictionary<GameObject,Health> healthDict;
    private void Start()
    {
        healthDict = new Dictionary<GameObject, Health>();
    }

    private void Update()
    {

        CountNextTick();

    }

    void CountNextTick()
    {
        if (timeTillNextTick <= 0)
        {
            if (healthDict.Count > 0)
            {
                int length = healthDict.Count;
                foreach (KeyValuePair<GameObject, Health> healthKeyValue in healthDict)
                {
                    healthKeyValue.Value.ApplyDamage(damageData);
                }
                timeTillNextTick = damageTickRateOnContactStay;
            }
            else
            {
                return;
            }
        }

        if (timeTillNextTick > 0)
        {
            timeTillNextTick -= Time.deltaTime;

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

       
        if (collisionLayers.value == (collisionLayers | 1 << collision.gameObject.layer))
        {
            Health health;
            collision.gameObject.TryGetComponent(out health);

            if (health)
            {
                healthDict.Add(collision.gameObject,health);
                health.ApplyDamage(damageData);
                timeTillNextTick = damageTickRateOnContactStay;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(healthDict.ContainsKey(collision.gameObject))
        {
            
            healthDict.Remove(collision.gameObject);
        }
    }
}
