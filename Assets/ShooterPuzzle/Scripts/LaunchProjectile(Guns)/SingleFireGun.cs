using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ProjectileLaunchEffectGraphics))]
public class SingleFireGun : LaunchProjectile
{

    public GameObject projectilePrefab;
    public bool hitScan;
    public DamageData power;
    public Transform launchPoint;
    public Vector3 rotationOffset;
    public Vector3 maxAccuracySpread;
    public float rateOfFire;
    public bool holdToFire;
    public float reloadTime;
    public float clipSize;
    public float startingClipSize;
    public float maxAmmo;
    public float startingAmmo;//this will be set with a player data ammoCount eventually

    ProjectileLaunchEffectGraphics shootFX;

    float currentAmmoInClip;
    float currentAmmo;
    float chamberingProgress;
    float reloadProgress;
    bool reloadingFlag;
    bool chamberingFlag;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        currentAmmoInClip = startingClipSize;
        currentAmmo = startingAmmo;
        reloadProgress = chamberingProgress = 1;
        projectilePrefab.GetComponent<BalisticProjectile>().damageData=power;
        shootFX = GetComponent<ProjectileLaunchEffectGraphics>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfProjectilesExpired();
        Reload(false);
        ChamberShot(false);
        if (holdToFire)
        {
            if (Input.GetButton("Fire1"))
            {
                Fire();
            }
        }
        else
        {
            if(Input.GetButtonDown("Fire1"))
            {
                Fire();
            }
        }
        if (Input.GetButtonDown("Reload"))
        {
            Reload(true);
        }
    }

    public void Fire()
    {

        if (chamberingProgress >= 1 && currentAmmoInClip > 0)
        {
            
            Vector3 accuracyMod = (Vector3.zero.Equals(maxAccuracySpread))?Vector3.zero:new Vector3(Random.Range(-maxAccuracySpread.x, maxAccuracySpread.x),
              Random.Range(-maxAccuracySpread.y, maxAccuracySpread.y), Random.Range(-maxAccuracySpread.z, maxAccuracySpread.z));

            Quaternion launchRotation = launchPoint.localToWorldMatrix.rotation;
            launchRotation = Quaternion.Euler(launchRotation.eulerAngles+rotationOffset+accuracyMod);

            if (!hitScan)
            {
                Launch(projectilePrefab, launchPoint.position, launchRotation);
            }
            else
            {
                Launch(launchPoint.position, transform.right, power, 100);
            }
            shootFX.DisplayShootFX(launchPoint.position,launchRotation);
            --currentAmmoInClip;

            if (currentAmmoInClip > 0)
            {
                ChamberShot(true);
            }
            else
            {
                currentAmmoInClip = 0;
            }
            
        }
        else if (currentAmmoInClip <= 0)
        {
            Reload(true);
        }
        
    }

    public void Reload(bool startReload)
    {
        if (!reloadingFlag && startReload)
        {
            
            if (reloadProgress >= 1 && clipSize > currentAmmoInClip && currentAmmo > 0)
            {
                reloadProgress = 0;
                reloadingFlag = true;
            }
        }

        if (reloadingFlag)
        {
            reloadProgress += (reloadTime*Time.deltaTime)/reloadTime;

            if(reloadProgress>=1)
            {
                reloadProgress = 1;
                reloadingFlag = false;

                if(currentAmmo>clipSize-currentAmmoInClip)
                {
                    currentAmmo -= clipSize - currentAmmoInClip;
                    currentAmmoInClip = clipSize;
                }
                else
                {
                    currentAmmoInClip += currentAmmo;
                    currentAmmo = 0;
                }
            }
        }
    }

    public void ChamberShot(bool startChamberShot)
    {
        if (!chamberingFlag && startChamberShot)
        {
            if (chamberingProgress >= 1)
            {

                chamberingProgress = 0;
                chamberingFlag = true;
            }
        }

        if(chamberingFlag)
        {
            chamberingProgress += rateOfFire * Time.deltaTime;
            if(chamberingProgress>=1)
            {
                chamberingProgress = 1;
                chamberingFlag = false;
            }
        }

        
    }

    public void AddAmmo(float ammo)
    {
        currentAmmo += ammo;
    }

    public float GetReloadProgress()
    {
        return reloadProgress;
    }

    public AmmoCount GetClip()
    {
        return (new AmmoCount(currentAmmoInClip,clipSize));
    }

    public AmmoCount GetAmmo()
    {
        return (new AmmoCount(currentAmmo, maxAmmo));
    }


}
