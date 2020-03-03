using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fox
{
    public class PlayerInput : MonoBehaviour
    {
       public PlayerFox player { get; protected set; }
        AttackFox attacks;

        void Start()
        {
            player = GetComponent<PlayerFox>();
            attacks = GetComponent<AttackFox>();
        }

        void Update()
        {
            Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            player.SetDirectionalInput(directionalInput);

            bool attackInput = (Input.GetAxisRaw("Fire1")!=0)?true:false;
            attacks.SetAttackInputs(attackInput);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                player.OnJumpInputDown(false);
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                player.OnJumpInputUp();
            }
        }
    }
}