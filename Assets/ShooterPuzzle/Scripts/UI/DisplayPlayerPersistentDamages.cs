using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPlayerPersistentDamages : MonoBehaviour
{
    public GameObject effectBasePrefab;

    // public Material fireMat; //set image instead?
    // public Material freezeMat;
    // public Material shockMat;
    //public Material acidMat;

    public float xSpacing;

    //value will be gotten from Lobby
    Health playerHealth;

    Dictionary<string, StoredComponents> effectsFromString;
    Dictionary<GameObject, StoredComponents> effectsFromObj;
    //Dictionary<GameObject, RectTransform> objToRect;
    //Dictionary<GameObject, Image> objToImage;
    List<StoredComponents> effectList;
    List<StoredComponents> deadIcons;

    bool needToUpdate;
    EffectSortData[] lastOrder;

    void Start()
    {
        effectsFromString = new Dictionary<string, StoredComponents>();
        // objToRect = new Dictionary<GameObject, RectTransform>();
        //objToImage = new Dictionary<GameObject, Image>();
        lastOrder = new EffectSortData[4] {new EffectSortData().ConstructEmpty(),new EffectSortData().ConstructEmpty(), new EffectSortData().ConstructEmpty(), new EffectSortData().ConstructEmpty() };
        effectList = new List<StoredComponents>();
        deadIcons = new List<StoredComponents>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        DisplayPersistentDamages();
    }

    void DisplayPersistentDamages()
    {
        needToUpdate = false;
        DamageEffectData data = playerHealth.GetPersistantEffects();

        EffectSortData[] ordered = new EffectSortData[4]
        {
            new EffectSortData("fire",data.fire.initalScore,Color.red,data.fire.damagePerSecond,data.fire.damageDuration,data.fire.initalDuration),
            new EffectSortData("freeze",data.freeze.initalScore,Color.cyan,data.freeze.damagePerSecond,data.freeze.damageDuration,data.freeze.initalDuration),
            new EffectSortData("shock",data.shock.initalScore,Color.yellow,data.shock.damagePerSecond,data.shock.damageDuration,data.shock.initalDuration),
            new EffectSortData("acid",data.acid.initalScore,Color.green,data.acid.damagePerSecond,data.acid.damageDuration,data.acid.initalDuration)
        };

        ordered = ArrageByHighestScore(ordered);
        int length = ordered.Length;
        for (int i = 0; i < length; i++)
        {
            if(!lastOrder[i].Comparison(ordered[i]))
            {
                lastOrder[i] = ordered[i];
                needToUpdate = true;
            }
        }

        
        StoredComponents effect;
       
            for (int i = 0; i < length; i++)
            {
                if (ordered[i].score > 0)//needs to be added or checked that there is no change
                {
                if (effectsFromString.ContainsKey(ordered[i].name))
                {
                    if (needToUpdate)
                    {
                        UpdateGraphicData(effectsFromString[ordered[i].name], ordered[i], i);
                    }
                    UpdateTimer(effectsFromString[ordered[i].name].effectRadialSlider, ordered[i], needToUpdate);
                    

                    //look to see if there is an inactive one. if no, then create a new one

                }
                else
                {
                    if (needToUpdate)
                    {
                        if (deadIcons.Count > 0)
                        {
                            effect = deadIcons[0];
                            effectList.Add(effect);
                            effectsFromString.Add(ordered[i].name, effect);
                            effect.effect.SetActive(true);
                            deadIcons.Remove(effect);

                            UpdateGraphicData(effectsFromString[ordered[i].name], ordered[i], i);
                        }
                        else
                        {
                            effect = new StoredComponents();
                            effect.effect = Instantiate(effectBasePrefab, gameObject.transform, false);
                            effect.effectImage = effect.effect.GetComponent<Image>();
                            effect.effectRect = effect.effect.GetComponent<RectTransform>();
                            effect.effectText = effect.effect.GetComponentInChildren<Text>();
                            effect.effectRadialSlider = effect.effect.GetComponentInChildren<Slider>();
                            effectsFromString.Add(ordered[i].name, effect);
                            effectList.Add(effect);
                            UpdateGraphicData(effectsFromString[ordered[i].name], ordered[i], i);
                        }

                        Vector2 pos = effect.effectRect.rect.size / 2;

                        pos.x += (xSpacing * i);
                        effect.effectRect.localPosition = pos;
                    }
                }
                }
                else //needs to be removed or checked that there is no change
                {
                if (needToUpdate)
                {
                    if (effectsFromString.ContainsKey(ordered[i].name))
                    {
                        effect = effectsFromString[ordered[i].name];
                        effectsFromString.Remove(ordered[i].name);
                        effect.effectImage.color = Color.clear;
                        effect.effect.SetActive(false);
                        effectList.Remove(effect);
                        deadIcons.Add(effect);
                    }
                }
                }
            }

        void UpdateGraphicData(StoredComponents effectData, EffectSortData orderedData, int i)
        {


            effectData.effectImage.color = orderedData.color;
            
            effectData.effectText.text = (orderedData.damagePerSecond < 10) ? 
                (Mathf.Round(orderedData.damagePerSecond * 10) / 10).ToString() :
                Mathf.Round(orderedData.damagePerSecond).ToString();

            Vector2 pos = effectData.effectRect.rect.size / 2;

            pos.x += (xSpacing * i);
            effectData.effectRect.localPosition = pos;
            UpdateTimer(effectData.effectRadialSlider,orderedData,true);
        }

        void UpdateTimer(Slider radialSlider, EffectSortData effectData, bool getRealValue)
        {
            radialSlider.value = (getRealValue) ?effectData.currentDuration/effectData.initalDuration:(effectData.currentDuration-((Health.tickRate-playerHealth.nextTick)))/effectData.initalDuration;

    }
    }

    EffectSortData[] ArrageByHighestScore(EffectSortData[] effectArray)
    {
        Array.Sort(effectArray, (y, x) => x.score.CompareTo(y.score));

        return effectArray;
    }

    public struct EffectSortData
    {
        public string name;
        public float score;
        public Color color;
        public float damagePerSecond;
        public float currentDuration;
        public float initalDuration;

        public EffectSortData(string name_,float score_,Color color_,float damagePerSecond_,float currentDuration_, float initalDuration_)
        {
            name = name_;
            score = score_;
            color = color_;
            damagePerSecond = damagePerSecond_;
            currentDuration = currentDuration_;
            initalDuration = initalDuration_;
        }

        public EffectSortData ConstructEmpty()
        {
            
            name = "empty";
            score = 0;
            color = Color.clear;
            damagePerSecond = 0;
            currentDuration = 0;
            initalDuration = 0;

            return this;
        }

        public bool Comparison(EffectSortData other)
        {
            bool equal = (name==other.name&&score==other.score
                &&color==other.color&&damagePerSecond==other.damagePerSecond
                &&initalDuration==other.initalDuration)?true:false;

            return equal;
        }
    }

    public struct StoredComponents
    {
        public GameObject effect;
        public Image effectImage;
        public RectTransform effectRect;
        public Text effectText;
        public Slider effectRadialSlider;
        
    }
}
