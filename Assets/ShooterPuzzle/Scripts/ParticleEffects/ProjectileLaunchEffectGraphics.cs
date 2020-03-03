using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLaunchEffectGraphics : MonoBehaviour
{
    public ParticleSystem muzzleFlashFX;
    public ParticleSystem bulletTrailFX;
    
    
    public void DisplayShootFX(Vector3 position, Quaternion rotation)
    {
        muzzleFlashFX.transform.position = bulletTrailFX.transform.position = position;
        muzzleFlashFX.transform.rotation = bulletTrailFX.transform.rotation = rotation;

        muzzleFlashFX.Emit(1);
        bulletTrailFX.Emit(1);
    }
}
