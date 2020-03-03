using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{
    public LayerMask collisionLayers;
    public LayerMask damageableLayers;
    List<GameObject> projectilePool;
    List<GameObject> projectileActive;

    public virtual void Start()
    {
        projectilePool = new List<GameObject>();
        projectileActive = new List<GameObject>();

    }

    public void Launch(GameObject projectile,Vector3 position, Quaternion rotation)
    {
        if(projectilePool.Count>0)
        {
            projectilePool[0].transform.position = position;
            projectilePool[0].transform.rotation = rotation;
            projectilePool[0].SetActive(true);
            projectileActive.Add(projectilePool[0]);
            projectilePool.Remove(projectilePool[0]);
        }
        else
        {
            projectileActive.Add(Instantiate(projectile, position,rotation));
        }
    }
    public void Launch(Vector3 position,Vector2 forward, DamageData power,float range)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, forward,range, collisionLayers);

        if(hit)
        {
            if(damageableLayers.value==(damageableLayers| 1<<hit.collider.gameObject.layer))
            {
                hit.collider.gameObject.GetComponent<Health>().ApplyDamage(power);
            }
        }
    }

    

    public void CheckIfProjectilesExpired()
    {
        int length = projectileActive.Count;
        
            List<GameObject> disabledProjectiles = new List<GameObject>();
            for (int i = 0; i < length; ++i)
            {
                if (!projectileActive[i].activeSelf)
                {
                    disabledProjectiles.Add(projectileActive[i]);
                    projectilePool.Add(projectileActive[i]);

                }
            }
            length = disabledProjectiles.Count;
            for (int i = 0; i < length; i++)
            {
                projectileActive.Remove(disabledProjectiles[i]);
            }
            }
}

public struct AmmoCount
{
    public float currentAmmo;
    public float maxAmmo;

    public AmmoCount(float currentAmmo_,float maxAmmo_)
    {
        currentAmmo = currentAmmo_;
        maxAmmo = maxAmmo_;
    }
    public float PercentFull()
    {
        return currentAmmo/maxAmmo;
    }
}
