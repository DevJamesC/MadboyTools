using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputsTopDown), typeof(ControllerTopDown))]
public class PlayerTopDown : MonoBehaviour
{
    public float speed;
    float moveLimiter = .7f;

    ControllerTopDown controller;
    Vector2 directionalInput;
    Vector2 velocity;

    
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<ControllerTopDown>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity = speed * directionalInput;

        if(directionalInput.x!=0&&directionalInput.y!=0)
        { velocity *= moveLimiter;}


        controller.Move(velocity*Time.deltaTime,directionalInput);
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }
}
