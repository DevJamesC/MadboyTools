using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerInputsTopDown : MonoBehaviour
{
    PlayerTopDown player;
    // Start is called before the first frame update
    void Start()
    {
        
        player = GetComponent<PlayerTopDown>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.SetDirectionalInput(directionalInput);
    }
}
