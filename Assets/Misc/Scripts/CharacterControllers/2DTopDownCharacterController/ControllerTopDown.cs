using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTopDown : RaycastControllerTopDown
{
    

    public CollisionInfo collisions;
    [HideInInspector]
    public Vector2 playerInput;

    public void Move(Vector2 moveAmount,Vector2 input)
    {
        UpdateRaycastOrigins();
        collisions.Reset();

        playerInput = input;

      if(moveAmount.x!=0)
        {
            HorizontalCollisions(ref moveAmount);
        }

      if(moveAmount.y!=0)
        {
            VerticalCollisions(ref moveAmount);
        }
     

        

        transform.Translate(moveAmount, Space.World);
    }

    void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = Mathf.Sign(moveAmount.x);
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

        if (Mathf.Abs(moveAmount.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX*rayLength, Color.red);

            if (hit)
            {

               

                moveAmount.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;

                

                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }
        }

    }
    void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {

            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY*rayLength, Color.red);

            if(hit)
            {
                moveAmount.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;

            }
        }
    }


    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        //public bool climbingSlope;
        //public bool descendingSlope;
        //public bool slidingDownMaxSlope;

        //public float slopeAngle, slopeAngleOld;
        //public Vector2 slopeNormal;
        public Vector2 moveAmountOld;
        //public int faceDir;
       // public bool fallingThroughPlatform;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            //climbingSlope = false;
            //descendingSlope = false;
            //slidingDownMaxSlope = false;
            //slopeNormal = Vector2.zero;

            //slopeAngleOld = slopeAngle;
            //slopeAngle = 0;
        }
    }
}
