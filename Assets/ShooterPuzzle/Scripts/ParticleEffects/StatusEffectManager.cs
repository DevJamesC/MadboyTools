using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    public ParticleSystem fireSystem;
    public ParticleSystem freezeSystem;
    public ParticleSystem shockSystem;
    public ParticleSystem acidSystem;

    List<Vector3> firePositions;
    List<Vector3> freezePositions;
    List<Vector3> frozenSolidPositions;
    List<Vector3> shockPositions;
    List<Vector3> acidPositions;

    public static StatusEffectManager instance;

    const float rateOfEmission=.2f;
    float timeToNextEmission;
    

    private void Awake()
    {//set instance to be most recent instanciated StatusEffectManager. CHANGE if keeping constant is better        
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        timeToNextEmission = 0;
        firePositions = new List<Vector3>();
        freezePositions = new List<Vector3>();
        frozenSolidPositions = new List<Vector3>();
        shockPositions = new List<Vector3>();
        acidPositions = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        ApplyGraphics();
    }

    public void AddEffectAtLocation(Vector3 pos, string effect)
    {
        if(effect=="fire")
        { firePositions.Add(pos); }
        if (effect == "freeze")
        { freezePositions.Add(pos); }
        if (effect == "shock")
        { shockPositions.Add(pos); }
        if (effect == "acid")
        { acidPositions.Add(pos); }
        if(effect=="frozenSolid")
        { frozenSolidPositions.Add(pos); }
    }

    void ApplyGraphics()
    {
        int length;
        if (timeToNextEmission<=0)
        {
            length = firePositions.Count;
            for (int i = 0; i < length; i++)
            {
                fireSystem.transform.position = firePositions[i];
                fireSystem.Emit(1);
            }


            length = freezePositions.Count;
            for (int i = 0; i < length; i++)
            {
                freezeSystem.transform.position = freezePositions[i];
                freezeSystem.Emit(1);
            }

            length = acidPositions.Count;
            for (int i = 0; i < length; i++)
            {
                acidSystem.transform.position = acidPositions[i];
                acidSystem.Emit(1);
            }
            timeToNextEmission = rateOfEmission;
        }
        else
        {
            timeToNextEmission -= Time.deltaTime;
        }


        length = frozenSolidPositions.Count;
        for (int i = 0; i < length; i++)
        {
            freezeSystem.transform.position = frozenSolidPositions[i];
            freezeSystem.Emit(20);
        }

        length = shockPositions.Count;
        for (int i = 0; i < length; i++)
        {
            shockSystem.transform.position = shockPositions[i];
            shockSystem.Emit(1);
        }

        firePositions.Clear();
        freezePositions.Clear();
        frozenSolidPositions.Clear();
        shockPositions.Clear();
        acidPositions.Clear();
    }

}
