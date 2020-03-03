using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DisplayAmmo : MonoBehaviour
{
    //Lobby will pass PlayerID so we know who is the "active" player
    SingleFireGun singleFireGun;
    Text ammoDisplay;
    // Start is called before the first frame update
    void Start()
    {
       GameObject.FindGameObjectWithTag("Player").TryGetComponent(out singleFireGun);
        ammoDisplay = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
       AmmoDisplay();
    }

    void AmmoDisplay()
    {
        if(singleFireGun)
        {
           
            string display = singleFireGun.GetClip().currentAmmo + " / " + singleFireGun.GetAmmo().currentAmmo;
            float progress=singleFireGun.GetReloadProgress();
            display = (progress>=1) ?display:display+"   RELOADING   "+Mathf.RoundToInt(progress*100)+"%";
            ammoDisplay.text = display;
        }
    }
}
