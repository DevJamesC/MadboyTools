using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanPickupAmmo : MonoBehaviour
{
    public LayerMask pickUpLayer;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (pickUpLayer.value == (pickUpLayer | 1 << collision.gameObject.layer))
            {
            
            if (collision.gameObject.tag=="Ammo")
            {
                
                SingleFireGun singleFireGun;
                if(TryGetComponent(out singleFireGun))
                {
                  
                    AmmoCount ammo = singleFireGun.GetAmmo();
                    if (ammo.currentAmmo<ammo.maxAmmo)
                    {
                        singleFireGun.AddAmmo(collision.gameObject.GetComponent<AmmoPickup>().Pickup(singleFireGun.maxAmmo));
                        
                    }
                }
            }

        }
    }
}
