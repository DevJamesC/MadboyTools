using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPlayerHealth : MonoBehaviour
{

    public Slider healthBar;
    public Slider shieldbar;


    //value will be gotten from Lobby
    Health playerHealth;
  
   
    void Start()
    {
        
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
   
    }

    // Update is called once per frame
    void Update()
    {
        DisplayVitals();
    }

    void DisplayVitals()
    {
        HealthData vitals = playerHealth.GetVitals();

        shieldbar.value = vitals.currentShield / vitals.maxShield;
        healthBar.value = vitals.currentHealth / vitals.maxHealth;


    }
}
