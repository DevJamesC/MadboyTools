using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPersistentEffectController : MonoBehaviour
{
    public LayerMask effectableLayers;//this class manages the "effects" of persistant effects

    Dictionary<string, ElementalDamageEffect> lastActiveEffects;
    Dictionary<string, ActiveEffect> stringToEffect;
    Dictionary<GameObject, Health> collidedObjects;
    List<ApplyEffectsTo> collidedWith;
    Health playerHealth;
    PlayerTopDown playerMove;
    StatusEffectManager graphics;
    DamageEffectData originalEffects;

    bool onFire;
    bool inAcid;
    float fireAcidSpreadTickRate;
    float fireAcidSpreadTimeToNextTick;
    bool ticked;
    bool frozenSolid;
    float progressTillFrozenSolid;
   const float frozenSolidDuration=3;
    float frozenSolidCurrentDuration;
    //bool shocked;

    float shockedTickRate;
    float shockedTimeTillNextTick;
    float shockSlowDuration;
    float shockSlowCurrentDuration;

    //bool fireTerminated;
    bool freezeTerminated;
    bool shockTerminated;
    bool acidTerminated;

    float originalSpeed;
    float freezeSlow;
    float acidSlow;
    float shockedSlow;
    bool updateNetSlow;



    // Start is called before the first frame update
    void Start()
    {

        lastActiveEffects = new Dictionary<string, ElementalDamageEffect>();
        stringToEffect = new Dictionary<string, ActiveEffect>();
        collidedObjects = new Dictionary<GameObject, Health>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        playerMove = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerTopDown>();
        graphics= StatusEffectManager.instance;
        collidedWith = new List<ApplyEffectsTo>();
        originalSpeed = playerMove.speed;
        fireAcidSpreadTickRate = Health.tickRate;
        shockSlowDuration = .1f;
        frozenSolid = false;
        progressTillFrozenSolid = 0;

    }

    // Update is called once per frame
    void Update()
    {
       

        ApplyActiveEffects();

        if(onFire||inAcid)
        {
            if(fireAcidSpreadTimeToNextTick>0)
            {
                fireAcidSpreadTimeToNextTick -= Time.deltaTime;
            }
            else if(ticked)
            {
                fireAcidSpreadTimeToNextTick = fireAcidSpreadTickRate;
                ticked = false;
            }
        }
        else
        {
            fireAcidSpreadTimeToNextTick = fireAcidSpreadTickRate;
        }
        if(progressTillFrozenSolid>0)
        {
            progressTillFrozenSolid -= Time.deltaTime/3;

        }
        if(frozenSolid)
        {
            frozenSolidCurrentDuration -= Time.deltaTime;

            if(frozenSolidCurrentDuration<=0)
            {
                frozenSolid = false;
            }
        }
    }

    private void LateUpdate()
    {
        if (collidedWith.Count > 0)
        {
            int length = collidedWith.Count;
            ApplyEffectsTo data;
            for (int i = 0; i < length; i++)
            {
                data = collidedWith[i];
                data.healthdata.ApplyEffectDirectly(data.newEffects, data.obj);
            }
            collidedWith.Clear();
            ticked = true;
        }
    }
    void ApplyActiveEffects()
    {
        DamageEffectData data = playerHealth.GetPersistantEffects();
        originalEffects = playerHealth.GetOriginalPersistantEffects();

        //fireTerminated =
        ManageEffect(data.fire, "fire");
        freezeTerminated = ManageEffect(data.freeze, "freeze");
        shockTerminated = ManageEffect(data.shock, "shock");
        acidTerminated = ManageEffect(data.acid, "acid");

        if (stringToEffect.ContainsKey("fire"))
        {
            //if something else hits while on fire, chance to spread the fire based on intensity
            onFire = true;
            graphics.AddEffectAtLocation(transform.position,"fire");
        }
        else
        {
            onFire = false;
        }
        if (stringToEffect.ContainsKey("freeze"))
        {
            //slow player based on Intensity
    
            if(playerHealth.freezeChanged)
            {
                AddFreeze(playerHealth.freezeAddIntensity);
                playerHealth.freezeChanged = false;
              //  Debug.Log(progressTillFrozenSolid);
            }
            if (progressTillFrozenSolid < data.freeze.intensity)
                progressTillFrozenSolid = data.freeze.intensity;
            
            freezeSlow = (progressTillFrozenSolid>data.freeze.intensity)?progressTillFrozenSolid:data.freeze.intensity;
            
            if(frozenSolid)
            {
                freezeSlow = 1;
            }
            
            updateNetSlow = true;
            graphics.AddEffectAtLocation(transform.position, "freeze");
        }
        else
        {
            // frozen = false;
            if (freezeTerminated)
            {
                freezeSlow = 0;
                updateNetSlow = true;
                progressTillFrozenSolid = 0;
            }
        }
        if (stringToEffect.ContainsKey("shock"))
        {
            //slow player periodically based on intensity (period is effected by intensity too)
            // shocked = true;
            shockedTickRate = .5f * (1 - (data.shock.intensity * .9f));

            if (shockSlowCurrentDuration <= 0 && shockedTimeTillNextTick <= 0)
            {
                shockSlowCurrentDuration = shockSlowDuration;
            }

            if (shockedTimeTillNextTick <= 0 && shockSlowCurrentDuration > 0)
            {

                shockSlowCurrentDuration -= Time.deltaTime;

                if (shockSlowCurrentDuration <= 0)
                {
                    shockedTimeTillNextTick = shockedTickRate;
                    shockedSlow = 0;
                    updateNetSlow = true;
                    graphics.AddEffectAtLocation(transform.position, "shock");
                }
            }
            else if (shockedTimeTillNextTick > 0)
            {
                shockedTimeTillNextTick -= Time.deltaTime;
                if (shockedTimeTillNextTick <= 0)
                {
                    shockSlowCurrentDuration = shockSlowDuration;
                    shockedSlow = data.shock.intensity * .9f;
                    updateNetSlow = true;
                }
            }
            // Debug.Log(shockedTimeTillNextTick);

        }
        else
        {
            if (shockTerminated)
            {
                // shocked = false;
                shockedSlow = 0;
                shockedTickRate = 0;
                updateNetSlow = true;
            }
        }
        if (stringToEffect.ContainsKey("acid"))
        {
            //slow player and chance to spread based on intensity
            inAcid = true;
            acidSlow = data.acid.intensity / 2;
            updateNetSlow = true;
            graphics.AddEffectAtLocation(transform.position, "acid");
        }
        else
        {
            inAcid = false;

            if (acidTerminated)
            {
                acidSlow = 0;
                updateNetSlow = true;
            }
        }

        if (updateNetSlow)
        {
            float slow = (acidSlow > freezeSlow) ? acidSlow : freezeSlow;
            playerMove.speed = originalSpeed * Mathf.Clamp01(1 - (slow + shockedSlow));
            updateNetSlow = false;
        }

        bool ManageEffect(ElementalDamageEffect effect, string effectName)
        {
            bool effectTerminated = false;
            if (lastActiveEffects.ContainsKey(effectName))
            {

                if (!effect.Compare(lastActiveEffects[effectName]))
                {

                    if (effect.GetScore() > 0)
                    {
                        ActiveEffect newEffect = new ActiveEffect(effectName, effect.intensity);
                        if (!stringToEffect.ContainsKey(effectName))
                        {
                            stringToEffect.Add(effectName, newEffect);
                        }
                    }
                    else
                    {

                        lastActiveEffects.Remove(effectName);
                        stringToEffect.Remove(effectName);
                        effectTerminated = true;
                    }
                }
            }
            else
            {
                if (effect.GetScore() > 0)
                {
                    ActiveEffect newEffect = new ActiveEffect(effectName, effect.intensity);
                    lastActiveEffects.Add(effectName, effect);
                    stringToEffect.Add(effectName, newEffect);

                }
            }
            return effectTerminated;
        }
    }

    public void AddFreeze(float intensity)
    {
        progressTillFrozenSolid += intensity;
        
        if (progressTillFrozenSolid >= 1)
        {
            if (!frozenSolid)//if frozen solid and not already frozen solid, emit particles
            { graphics.AddEffectAtLocation(transform.position, "frozenSolid"); }

            frozenSolid = true;
            frozenSolidCurrentDuration = frozenSolidDuration;
            progressTillFrozenSolid = 0;
        }

        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
   
        if (effectableLayers.value == (effectableLayers | 1 << collision.gameObject.layer))
        {
            Debug.Log(collision.gameObject.name);
                Health health = collision.gameObject.GetComponent<Health>();
                collidedObjects.Add(collision.gameObject, health);

                if (fireAcidSpreadTimeToNextTick <= 0)
                {
                    DamageEffectData newDamage = new DamageEffectData();
                    if (onFire || inAcid)
                    {

                        newDamage.Clear();
                        if (onFire)
                        {
                            newDamage.fire = originalEffects.fire;


                        }
                        if (inAcid)
                        {
                            newDamage.acid = originalEffects.acid;
                        }
                        collidedWith.Add(new ApplyEffectsTo(health, newDamage, gameObject));

                    }



                }
            
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
  

        if (effectableLayers.value == (effectableLayers | 1 << collision.gameObject.layer))
        {
            if (onFire || inAcid)
            {
                if(fireAcidSpreadTimeToNextTick<=0)
                {
                    if (collidedObjects.ContainsKey(collision.gameObject))
                    {
                        DamageEffectData newDamage = new DamageEffectData();
                        newDamage.Clear();
                        if (onFire)
                        {
                            newDamage.fire = originalEffects.fire;


                        }
                        if (inAcid)
                        {
                            newDamage.acid = originalEffects.acid;
                        }
                        collidedWith.Add(new ApplyEffectsTo(collidedObjects[collision.gameObject], newDamage, gameObject));

                    }
                    else
                    {
                        Health health = collision.gameObject.GetComponent<Health>();
                        collidedObjects.Add(collision.gameObject, health);

                        if (fireAcidSpreadTimeToNextTick <= 0)
                        {
                            DamageEffectData newDamage = new DamageEffectData();
                            if (onFire || inAcid)
                            {

                                newDamage.Clear();
                                if (onFire)
                                {
                                    newDamage.fire = originalEffects.fire;


                                }
                                if (inAcid)
                                {
                                    newDamage.acid = originalEffects.acid;
                                }
                                collidedWith.Add(new ApplyEffectsTo(health, newDamage, gameObject));

                            }



                        }
                    
                }

                   
                }
            }
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
  

        if (effectableLayers.value == (effectableLayers | 1 << collision.gameObject.layer))
        {


            if (collidedObjects.ContainsKey(collision.gameObject))
            {
                collidedObjects.Remove(collision.gameObject);
            }

        }
    }


    public struct ActiveEffect
    {
        public string name;
        public float intensity;

        public ActiveEffect(string name_, float intensity_)
        {
            name = name_;
            intensity = intensity_;
        }
    }

    public struct ApplyEffectsTo
    {
        public Health healthdata;
        public DamageEffectData newEffects;
        public GameObject obj;

        public ApplyEffectsTo(Health healthdata_, DamageEffectData newEffects_, GameObject obj_)
        {
            healthdata = healthdata_;
            newEffects = newEffects_;
            obj = obj_;
        }
    }
}
