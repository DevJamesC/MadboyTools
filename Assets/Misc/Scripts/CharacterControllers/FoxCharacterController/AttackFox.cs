using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fox
{
    [RequireComponent(typeof(PlayerInput),typeof(PlayerFox))]
    public class AttackFox : MonoBehaviour
    {
        public LayerMask enemyLayer;

        PlayerFox player;
        ControllerFox controller;

        bool attackIsDown;
        private void Start()
        {
            player = GetComponent<PlayerFox>();
            controller = GetComponent<ControllerFox>();
        }

        public void SetAttackInputs(bool attackIsDown_)
        {
            attackIsDown = attackIsDown_;
            controller.SetGrind(attackIsDown_);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(enemyLayer==(enemyLayer|1<<collision.gameObject.layer))
            {

                if (attackIsDown&&!controller.collisions.grounded)
                {
                    player.IncrimentJumpIteration(false);
                    
                    player.OnJumpInputDown(true);
                }
            }
        }
    }
}
