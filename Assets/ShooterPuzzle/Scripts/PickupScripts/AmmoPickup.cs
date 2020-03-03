using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{

    [Range(0,100)]public float percentToFullAmmo;

    public float Pickup(float maxAmmo)
    {
        StartCoroutine(Disable());
        return Mathf.Ceil(maxAmmo * (percentToFullAmmo/100));
    }

    IEnumerator Disable()
    {
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
        StopCoroutine(Disable());
    }
}
