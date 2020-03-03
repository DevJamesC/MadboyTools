using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour//health- need it to live, shield- protects your health, armour-damage reduction percent
{
    public HealthData healthData;
    List<GameObject> blackListObjects;//used to prevent certain object from appling thier effect too many times
    float blackListReset;
    float timeTillReset;
    

    [HideInInspector]
    public const float tickRate = .2f;//times per second
    [HideInInspector]
    public float nextTick;
    [HideInInspector]
    public bool freezeChanged;
    [HideInInspector]
    public float freezeAddIntensity;

    DamageEffectData persistantDamages;
    DamageEffectData originalPersistantDamages;

    public bool tookDamage { get; protected set; }

    float timeTillTempResistExpire;

    // Start is called before the first frame update
    void Start()
    {
       // healthData = new HealthData();
        healthData.isDamageable = true;
        healthData.isDead = false;
        tookDamage = false;
        persistantDamages = new DamageEffectData();
        blackListObjects = new List<GameObject>();
        blackListReset = 2;
        persistantDamages.Clear();
        nextTick = tickRate;
    
    }

    // Update is called once per frame
    void Update()
    {
       
        ApplyPersistentDamages();
        ApplyRegenHeal();

        if (blackListObjects.Count>0)
        {
            if(timeTillReset>0)
            {
                timeTillReset -= Time.deltaTime;
            }
            else
            {
                timeTillReset = blackListReset;
                blackListObjects.Clear();
            }
        }

        if(timeTillTempResistExpire>0)
        {
            timeTillTempResistExpire -= Time.deltaTime;

            if(timeTillTempResistExpire<=0)
            {
                healthData.resistancesData.ResetTempResistances();
            }
        }
    }
    private void LateUpdate()
    {
        if(tookDamage)
        {
            tookDamage = false;
        }
    }

    void ApplyPersistentDamages()
    {
        if (!healthData.isDamageable || healthData.isDead)
        {
            persistantDamages.Clear();
            return;
        }

        if (persistantDamages.isEmpty())
        {
            return;
        }

        if (nextTick > 0)
        {
            nextTick -= Time.deltaTime;
            return;
        }
        else
        {
            nextTick = tickRate;
        }


        float[] damageArray = persistantDamages.DamagePerSecondAsArray(tickRate);

        float[] damageModArray = persistantDamages.ShieldModAsArray();
        float[] damageTaken = new float[4] { 0, 0, 0, 0 };//order is fire, freeze, shock, acid. Prepend another float for regular damage if applicible

        int length = damageArray.Length;
        for (int i = 0; i < length; ++i)//appy damage to shield
        {
            if (healthData.currentShield > 0)
            {
                

                healthData.currentShield -= damageArray[i] * damageModArray[i];
                
                damageTaken[i] += damageArray[i] * damageModArray[i];
                if(damageArray[i]*damageArray[i]>0)
                {
                    healthData.regenData.timeTillShieldRegenStart = healthData.regenData.shieldRegenDelay;
                }
                if (!tookDamage)
                {
                    tookDamage = (damageTaken[i] > 0) ? true : false;

                }
                if (healthData.currentShield <= 0)
                {
                    damageArray[i] = healthData.currentShield / damageModArray[i];
                    damageTaken[i] -= healthData.currentShield / damageModArray[i];
                    healthData.currentShield = 0;
                    break;
                }
                damageArray[i] = 0;
            }
        }
        healthData.currentShield = Mathf.Round(healthData.currentShield * 100) / 100;

        damageModArray = persistantDamages.HealthModAsArray();

        length = damageModArray.Length;
        for (int i = 0; i < length; ++i)//apply damage to health
        {
            if (healthData.currentHealth > 0)
            {
                healthData.currentHealth -= damageArray[i] * damageModArray[i];
                damageTaken[i] += damageArray[i] * damageModArray[i];
                if (damageArray[i] * damageArray[i] > 0)
                {
                    healthData.regenData.timeTillShieldRegenStart = healthData.regenData.shieldRegenDelay;
                    healthData.regenData.timeTillHealthRegenStart = healthData.regenData.healthRegenDelay;

                }
                if (!tookDamage)
                {
                    tookDamage = (damageTaken[i] > 0) ? true : false;

                }
                if (healthData.currentHealth <= 0)
                {
                    healthData.currentHealth = 0;
                    healthData.isDead = true;
                    break;
                }
            }
        }
        healthData.currentHealth = Mathf.Round(healthData.currentHealth * 100) / 100;
        healthData.recentDamage.SetElementDamage(damageTaken);
        persistantDamages.ApplyTickRateCountDown(tickRate);

       

        //Debug.Log("Took " + damageTaken[0] + " Fire," + damageTaken[1] + " Freeze, " + damageTaken[2] + " Shock, " + damageTaken[3] + " Acid Damage");
        //Debug.Log("Health: " + healthData.currentHealth + ". Shield: " + healthData.currentShield);

    }

    public void ApplyDamage(DamageData damageData)// need to have a apply peristant effect damage
    {
        if (!healthData.isDamageable || healthData.isDead)
            return;

        float[] damageTaken = new float[5] {0, 0, 0, 0, 0 };
        DamageEffectData originalPersistantDamagesCopy = damageData.damageEffectData;

        float armourValue = (healthData.currentArmour - damageData.armourPierce > 0) ? healthData.currentArmour - damageData.armourPierce : 0;
        float ignoreShield = damageData.damage * damageData.shieldPierce;
        damageData.damage -= ignoreShield;
        damageData.damageEffectData.ApplyResistances(healthData.resistancesData.TotalResistance());

        float[] damageArray = damageData.damageEffectData.ImpactDamageAsArray() ;
        damageArray = new float[5] { damageData.damage, damageArray[0], damageArray[1], damageArray[2], damageArray[3], };
        float[] damageModArray = damageData.damageEffectData.ShieldModAsArray();
        damageModArray = new float[] { damageData.shieldDamageMod, damageModArray[0], damageModArray[1], damageModArray[2], damageModArray[3] };
        
        
        int length = damageArray.Length;
        for (int i = 0; i < length; ++i)
        {
            if (healthData.currentShield > 0)//apply shield Damage
            {
                healthData.currentShield -= damageArray[i] * damageModArray[i];

                damageTaken[i] += damageArray[i] * damageModArray[i];
                if (damageArray[i] * damageArray[i] > 0)
                {
                    healthData.regenData.timeTillShieldRegenStart = healthData.regenData.shieldRegenDelay;
                }
                if (!tookDamage)
                {
                    tookDamage = (damageTaken[i] > 0) ? true : false;

                }
                if (healthData.currentShield <= 0)
                {
                    damageArray[i] = healthData.currentShield / damageModArray[i];
                    damageTaken[i] -= healthData.currentShield / damageModArray[i];
                    healthData.currentShield = 0;
                    break;
                }
                damageArray[i] = 0;
            }

        }
        damageModArray = damageData.damageEffectData.HealthModAsArray();
        damageModArray = new float[] { damageData.healthDamageMod, damageModArray[0], damageModArray[1], damageModArray[2], damageModArray[3] };

        damageArray[0] *= (1 - armourValue);
        for (int i = 0; i < length; ++i)
        {
            if (healthData.currentHealth > 0)// apply health Damage
            {
                healthData.currentHealth -= damageArray[i] * damageModArray[i];
                damageTaken[i] += damageArray[i] * damageModArray[i];
                if (damageArray[i] * damageArray[i] > 0)
                {
                    healthData.regenData.timeTillShieldRegenStart = healthData.regenData.shieldRegenDelay;
                    healthData.regenData.timeTillHealthRegenStart = healthData.regenData.healthRegenDelay;

                }
                if (!tookDamage)
                {
                    tookDamage = (damageTaken[i] > 0) ? true : false;

                }
                if (healthData.currentHealth <= 0)
                {
                    damageArray[i] = healthData.currentHealth / damageModArray[i];
                    damageTaken[i] -= healthData.currentHealth / damageModArray[i];
                    healthData.currentHealth = 0;
                    healthData.isDead = true;
                    break;
                }
                damageArray[i] = 0;
            }
        }

        if (!damageData.damageEffectData.isEmpty())//apply new persistent effects, if there are any to apply
        {
            damageData.damageEffectData.RollForEffectChance();
            if (damageData.damageEffectData.fire.GetScore()==0)
            {
                originalPersistantDamagesCopy.fire.Clear();
            }
            if (damageData.damageEffectData.freeze.GetScore() == 0)
            {
                originalPersistantDamagesCopy.freeze.Clear();
            }
            if (damageData.damageEffectData.shock.GetScore() == 0)
            {
                originalPersistantDamagesCopy.shock.Clear();
            }
            if (damageData.damageEffectData.acid.GetScore() == 0)
            {
                originalPersistantDamagesCopy.acid.Clear();
            }

            damageData.damageEffectData.SetInitalScores();
            originalPersistantDamagesCopy.SetInitalScores();
            freezeChanged = false;
            freezeAddIntensity = 0;
            freezeChanged = damageData.damageEffectData.freeze.GetScore()>0;
            freezeAddIntensity = damageData.damageEffectData.freeze.intensity;
            persistantDamages.CheckEffectScores(damageData.damageEffectData);
            originalPersistantDamages.CheckEffectScores(originalPersistantDamagesCopy);
        }

        healthData.recentDamage.SetAllDamage(damageTaken);

       // Debug.Log("Health: " + healthData.currentHealth + ". Shield: " + healthData.currentShield);
    }

    public void ApplyEffectDirectly(DamageEffectData damage,GameObject obj)
    {
        if (!blackListObjects.Contains(obj))
        {
            damage.ApplyResistances(healthData.resistancesData.TotalResistance());
            damage.RollForEffectChance();
            damage.SetInitalScores();
            
            freezeChanged = persistantDamages.freeze.GetScore()>0;
            persistantDamages.CheckEffectScores(damage);
            originalPersistantDamages.CheckEffectScores(damage);
            if (damage.fire.GetScore() > 0 || damage.acid.GetScore() > 0)
            {
                blackListObjects.Add(obj);
            }
        }
       
    }

    public void ApplyHeal(HealData healing, bool applyResistanceData=false, float resistanceDuration=0)
    {
        healthData.currentHealth =  Mathf.Min(healthData.maxHealth, healthData.currentHealth+ healing.heath);
        healthData.currentShield = Mathf.Min(healthData.maxShield, healthData.currentShield + healing.shield);
        healthData.currentArmour = Mathf.Min(healthData.maxArmour, healthData.currentArmour + healing.armour);

        if (applyResistanceData)
        {
           
                timeTillTempResistExpire = resistanceDuration;
                healthData.resistancesData.SetTempResist(healing.resistance);
            //this replaces old buffs with new ones. If we want to keep multiple resistance buffs, then 
            //the temp resistances need to be in a list, each with thier own remaining duration timer. this means that the tempRes will need to be in a struct.
            

        }
    }

    void ApplyRegenHeal()
    {

        if (healthData.regenData.timeTillHealthRegenStart >0)
        {
            healthData.regenData.timeTillHealthRegenStart -= Time.deltaTime;
        }
        else
        {
            if(healthData.currentHealth<healthData.maxHealth)
            {
                healthData.currentHealth += healthData.regenData.healthRegenRate * Time.deltaTime;
            }
        }

        if (healthData.regenData.timeTillShieldRegenStart >0)
        {
            healthData.regenData.timeTillShieldRegenStart -= Time.deltaTime;
        }
        else
        {
            if (healthData.currentShield < healthData.maxShield)
            {
                healthData.currentShield += healthData.regenData.shieldRegenRate * Time.deltaTime;
            }
        }
    }

    

    public HealthData GetVitals()
    {
        return healthData;
    }

    public DamageEffectData GetPersistantEffects()
    {
        return persistantDamages;
    }

    public DamageEffectData GetOriginalPersistantEffects()
    {
        return originalPersistantDamages;
    }
   
}

[Serializable]
public struct HealthData
{
    public float maxHealth;
    public float maxArmour;
    public float maxShield;

    public float currentHealth;
    public float currentArmour;
    public float currentShield;

    public bool isDamageable;
    public bool isDead;

    public RegenData regenData;
    public ResistancesData resistancesData;
    public LastDamageTaken recentDamage;

    public void Reset()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;
        currentArmour = maxArmour;
        isDamageable = true;
        isDead = false;
    }

}

public struct LastDamageTaken
{
    float normal;
    float fire;
    float freeze;
    float shock;
    float acid;

    public void ClearElements()
    {
        fire = freeze = shock = acid = 0;
    }

    public void SetElementDamage(float[] elements)
    {
        if(elements.Length==4)
        {
            fire = elements[0];
            freeze = elements[1];
            shock = elements[2];
            acid = elements[3];
        }
    }

    public void SetAllDamage(float[] damages)
    {
        if (damages.Length == 5)
        {
            normal = damages[0];
            fire = damages[1];
            freeze = damages[2];
            shock = damages[3];
            acid = damages[4];
        }
    }
    public float[] ElementalDamageAsArray()
    {
        return new float[4] {fire, freeze,shock,acid };
    }
}

[Serializable]
public struct RegenData
{
    public float healthRegenRate;
    public float shieldRegenRate;
    public float healthRegenDelay;
    public float shieldRegenDelay;
    public float timeTillHealthRegenStart;
    public float timeTillShieldRegenStart;
}

[Serializable]
public struct ResistancesData
{
    public float fireResist;
    public float freezeResist;
    public float shockResist;
    public float acidResist;

    public float tempFireResist;
    public float tempFreezeResist;
    public float tempShockResist;
    public float tempAcidResist;

    public ResistancesData TotalResistance()
    {
        ResistancesData data= new ResistancesData();
        data.fireResist = fireResist + tempFireResist;
        data.freezeResist = freezeResist + tempFreezeResist;
        data.shockResist = shockResist + tempShockResist;
        data.acidResist = acidResist + tempAcidResist;

        return data;
    }

    public void SetTempResist(ResistancesData data)
    {
        tempFireResist = data.tempFireResist;
        tempFreezeResist = data.tempFreezeResist;
        tempShockResist = data.tempShockResist;
        tempAcidResist = data.tempAcidResist;
    }

    public void ResetTempResistances()
    {
        tempFireResist = tempFreezeResist = tempShockResist = tempAcidResist = 0;
    }

}

[Serializable]
public struct DamageData
{
    public float damage;
    public float armourPierce;
    public float shieldPierce;
    public float healthDamageMod;
    public float shieldDamageMod;

    public DamageEffectData damageEffectData;
}

[Serializable]
public struct DamageEffectData
{
    public ElementalDamageEffect fire;
    public ElementalDamageEffect freeze;
    public ElementalDamageEffect shock;
    public ElementalDamageEffect acid;

    public bool isEmpty()
    {
        bool isEmpty = true;
        if (fire.damageDuration > 0 || freeze.damageDuration > 0 || shock.damageDuration > 0 || acid.damageDuration > 0)
        { isEmpty = false; }
        return isEmpty;

    }

    public void ApplyResistances(ResistancesData resistanceData)
    {
       fire.ApplyResistances(resistanceData.fireResist);
       freeze.ApplyResistances(resistanceData.freezeResist);
       shock.ApplyResistances(resistanceData.shockResist);
       acid.ApplyResistances(resistanceData.acidResist);
    }

    public float[] ImpactDamageAsArray()
    {
        return new float[4] {fire.impactDamage,freeze.impactDamage,shock.impactDamage,acid.impactDamage };
    }

    public float[] ShieldModAsArray()
    {
        return new float[4] { fire.shieldMod, freeze.shieldMod, shock.shieldMod, acid.shieldMod };
    }

    public float[] HealthModAsArray()
    {
        return new float[4] { fire.healthMod, freeze.healthMod, shock.healthMod, acid.healthMod };
    }

    public float[] DamagePerSecondAsArray(float tickRate)
    {
       
        return new float[4] { fire.damagePerSecond*tickRate, freeze.damagePerSecond*tickRate, shock.damagePerSecond*tickRate, acid.damagePerSecond*tickRate };
    }

    public void RollForEffectChance()
    {
        fire.RollForEffectChance();
        freeze.RollForEffectChance();
        shock.RollForEffectChance();
        acid.RollForEffectChance();
    }

    public void Clear()
    {
        fire.Clear();
        freeze.Clear();
        shock.Clear();
        acid.Clear();
    }

    

    public void CheckEffectScores(DamageEffectData current)
    {
        fire = (fire.GetScore() > current.fire.GetScore()) ? fire : current.fire;
        freeze = (freeze.GetScore() > current.freeze.GetScore()) ? freeze : current.freeze;
        shock = (shock.GetScore() > current.shock.GetScore()) ? shock : current.shock;
        acid = (acid.GetScore() > current.acid.GetScore()) ? acid : current.acid;
    }
    

    public void ApplyTickRateCountDown(float time)
    {
        fire.damageDuration -= time;
        if (fire.damageDuration <= 0) { fire.Clear(); }
        freeze.damageDuration -= time;
        if (freeze.damageDuration <= 0) { freeze.Clear(); }
        shock.damageDuration -= time;
        if (shock.damageDuration <= 0) { shock.Clear(); }
        acid.damageDuration -= time;
        if (acid.damageDuration <= 0) { acid.Clear(); }
    }

    public void SetInitalScores()
    {
        fire.SetInital();
        freeze.SetInital();
        shock.SetInital();
        acid.SetInital();
    }
}

[Serializable]
public struct ElementalDamageEffect
{
    public float impactDamage;
    public float persistentEffectChance;
    public float intensity;
    public float damagePerSecond;
    public float damageDuration;
    public float initalDuration;
    public float shieldMod;
    public float healthMod;
    public float initalScore;
    public void ApplyResistances(float resistance)
    {
        impactDamage *= (1 - resistance);
        persistentEffectChance *= (1 - resistance);
        intensity *= (1 - resistance);
        damagePerSecond *= (1 - resistance);
        damageDuration *= (1 - resistance);
    }

    public void RollForEffectChance()
    {
        if(UnityEngine.Random.Range(1,100)>persistentEffectChance)
        {
            Clear();
        }
    }

    public float GetScore()
    {
        return damageDuration * damagePerSecond * intensity;
    }

    public void SetInital()
    {
        initalScore = GetScore();
        initalDuration = damageDuration;
    }

    public void Clear()
    {
        impactDamage = persistentEffectChance = intensity=initalDuration = damagePerSecond = damageDuration = shieldMod = healthMod = initalScore= 0;
    }

    public bool Compare(ElementalDamageEffect other)
    {
        bool same = true;
        if(other.impactDamage!=impactDamage||other.damagePerSecond!=damagePerSecond||other.persistentEffectChance!=persistentEffectChance
            ||other.intensity!= intensity||other.initalDuration!=initalDuration||other.initalScore!=initalScore)
            {
            same = false;
        }

        return same;

    }

}

public struct HealData
{
    public float heath;
    public float shield;
    public float armour;
    public ResistancesData resistance;
}




