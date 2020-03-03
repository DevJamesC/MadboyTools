using UnityEngine;
using System.Collections;

namespace SebastianLague
{
    public class PlayerInputPlatformer : MonoBehaviour
    {

        PlayerPlatformer player;

        void Start()
        {
            player = GetComponent<PlayerPlatformer>();
        }

        void Update()
        {
            Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            player.SetDirectionalInput(directionalInput);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                player.OnJumpInputDown();
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                player.OnJumpInputUp();
            }
        }
    }
}
