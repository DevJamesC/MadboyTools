using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalisticProjectile : MonoBehaviour
{
    public float speed;
    public LayerMask obsticleMask;
    public LayerMask damageableMask;

    [SerializeField]
    public DamageData damageData;

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        transform.Translate(-Vector3.up*speed*Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(damageableMask.value==(damageableMask|1<<collision.gameObject.layer))
        {
            collision.gameObject.GetComponent<Health>().ApplyDamage(damageData);
            //do endOfLife effects
            gameObject.SetActive(false);
        }
        
        if (obsticleMask.value == (obsticleMask | 1<<collision.gameObject.layer))
        {
            //do endOfLife effects
            gameObject.SetActive(false);
        }
    }



}
