using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fox
{
    public class ControllerFox : RaycastController
    {
        public float maxSlopeAngle = 80;

        public CollisionInfo collisions;
        public LayerMask defaultGroundMask;
        public LayerMask grindrailMask;
        public LayerMask wallRunMask;
        public float wallRunThreashold;
        public string wallRunRampTag;


        bool canGrind;

        bool overWallRunThreashold;
        [HideInInspector]
        public bool jumpedOnRamp;
       

        [HideInInspector]
        public Vector2 playerInput;

        public override void Start()
        {
            base.Start();
            collisions.faceDir = 1;
            canGrind = false;
            jumpedOnRamp = false;
           

        }

        public void SetGrind(bool grindState)
        {
            canGrind = grindState;
        }
        public void CheckWallRunThreashold(Vector2 velocity)
        {
            float speed = velocity.x + velocity.y;
            speed = Mathf.Abs(speed);
            if (velocity.y >= 0)
            {
                overWallRunThreashold = (speed >= wallRunThreashold) ? true : false;
            }
            else
            {
                overWallRunThreashold = false;
            }
        }


        public void Move(Vector2 moveAmount, bool standingOnPlatform)
        {
            Move(moveAmount, Vector2.zero, standingOnPlatform);
        }

        public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
        {
            UpdateRaycastOrigins();

            collisions.Reset();
            collisions.moveAmountOld = moveAmount;
            playerInput = input;

            if (moveAmount.y < 0)
            {
                DescendSlope(ref moveAmount);
            }

            if (moveAmount.x != 0)
            {
                collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
            }

            HorizontalCollisions(ref moveAmount);
            if (moveAmount.y != 0 || collisions.onGrindRail)
            {
                VerticalCollisions(ref moveAmount);
            }

            transform.Translate(moveAmount);

            if (standingOnPlatform)
            {
                collisions.below = true;
            }

            if (!canGrind)
            {
                collisions.onGrindRail = false;
            }

        }

        void HorizontalCollisions(ref Vector2 moveAmount)
        {
            float directionX = collisions.faceDir;
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

               // Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

                if (hit)
                {
                    
                    
                    if (hit.distance == 0||(grindrailMask==(grindrailMask|1<<hit.collider.gameObject.layer)&&!collisions.onGrindRail))
                    {
                        continue;
                    }
                    
                    if (wallRunMask == (wallRunMask | 1 << hit.collider.gameObject.layer) && (wallRunRampTag == hit.collider.gameObject.tag || collisions.onWallRunOld || collisions.jumpedFromWallRun))
                    {//if we hit something with layer wallrun, AND if it is EITHER a wallrunRamp, or we were already wallrunning

                       
                        
                        collisions.onWallRun = true;
                        collisions.jumpedFromWallRun = false;
                        if (wallRunRampTag == hit.collider.gameObject.tag)
                        {
                            collisions.onWallRunRamp = true;
                            if (!overWallRunThreashold)
                            {
                                
                                collisions.onWallRun = false;

                            }
                        }
                        else
                        {
                            collisions.onWallRunRamp = false;
                        }
                        if(jumpedOnRamp)
                        { 

                            
                            collisions.onWallRun = false;
                            collisions.onWallRunRamp = false;
                            jumpedOnRamp = false;

                        }

                    }
                    else
                    {
                        
                        collisions.onWallRun = false;
                    }

                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                    if (i == 0 && (slopeAngle <= maxSlopeAngle)||collisions.onWallRun)
                    {
                        if (collisions.descendingSlope)
                        {
                            collisions.descendingSlope = false;
                            moveAmount = collisions.moveAmountOld;
                        }
                        float distanceToSlopeStart = 0;
                        if (slopeAngle != collisions.slopeAngleOld)
                        {
                            distanceToSlopeStart = hit.distance - skinWidth;
                            moveAmount.x -= distanceToSlopeStart * directionX;
                        }
                        ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
                        moveAmount.x += distanceToSlopeStart * directionX;
                    }

                    if (!collisions.climbingSlope || (slopeAngle > maxSlopeAngle&&!collisions.onWallRun))
                    {
                        moveAmount.x = (hit.distance - skinWidth) * directionX;
                        rayLength = hit.distance;

                        if (collisions.climbingSlope)
                        {
                            moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                        }

                        collisions.left = directionX == -1;
                        collisions.right = directionX == 1;
                    }




                }
            }
        }

        void VerticalCollisions(ref Vector2 moveAmount)
        {

            float directionY = Mathf.Sign(moveAmount.y);
            float rayLength = (moveAmount.y != 0) ? Mathf.Abs(moveAmount.y) + skinWidth : skinWidth * 2;
            if (collisions.onGrindRail) { rayLength += skinWidth * 2; }
            bool changedGrindStateFlag = false;



            for (int i = 0; i < verticalRayCount; i++)
            {

                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

                // Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);



                if (hit)
                {
                    if (grindrailMask == (grindrailMask | 1 << hit.collider.gameObject.layer))
                    {
                        if (directionY == 1 || hit.distance == 0)
                        {
                            continue;
                        }
                        if (collisions.fallingThroughPlatform)
                        {
                            continue;
                        }
                        if (!canGrind)
                        {
                            collisions.fallingThroughPlatform = true;
                            Invoke("ResetFallingThroughPlatform", .5f);

                            continue;
                        }


                    }
                    if(wallRunMask==(wallRunMask|1<<hit.collider.gameObject.layer)&&(wallRunRampTag==hit.collider.gameObject.tag||collisions.onWallRunOld||collisions.jumpedFromWallRun))
                    {

                        
                        collisions.onWallRun = true;
                        collisions.jumpedFromWallRun = false;
                        if (wallRunRampTag == hit.collider.gameObject.tag)
                        {
                            collisions.onWallRunRamp = true;
                            if (!overWallRunThreashold)
                            {
                                
                                collisions.onWallRun = false;

                            }
                        }
                        else
                        {
                            collisions.onWallRunRamp = false;
                        }
                        if (jumpedOnRamp)
                        {
                            
                            collisions.onWallRun = false;
                            collisions.onWallRunRamp = false;
                            jumpedOnRamp = false;
                        }
                    }
                    else//If there is some unwallrunning errors its cause this triggers after horizontal collisions
                    {
                        
                        collisions.onWallRun = false;
                        collisions.onWallRunRamp = false;
                        jumpedOnRamp = false;
                    }


                    collisions.below = directionY == -1;
                    collisions.above = directionY == 1;

                    if (collisions.below)
                    {
                        if (defaultGroundMask == (defaultGroundMask | 1 << hit.collider.gameObject.layer))
                        {
                            if (canGrind)
                            {
                                collisions.grounded = true;
                                if (!changedGrindStateFlag)
                                {
                                    if (!changedGrindStateFlag)
                                    {
                                        collisions.onGrindRail = (grindrailMask == (grindrailMask | 1 << hit.collider.gameObject.layer));
                                    }
                                    if (collisions.onGrindRail)
                                    {
                                        changedGrindStateFlag = true;
                                        collisions.onWallRun = false;
                                        collisions.jumpedFromWallRun = false;
                                        
                                    }
                                }
                            }
                            else
                            {
                                collisions.grounded = (grindrailMask != (grindrailMask | 1 << hit.collider.gameObject.layer));
                                collisions.onGrindRail = false;
                            }


                        }
                        else
                        {
                            collisions.grounded = false;
                            collisions.onGrindRail = false;
                      
                        }
                    }
                    else
                    {
                        collisions.grounded = false;
                        collisions.onGrindRail = false;
                    }



                    moveAmount.y = (hit.distance - skinWidth) * directionY;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                    {
                        moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                    }

                }
                else
                {
                    collisions.grounded = false;
                    if (!changedGrindStateFlag)
                    {
                        collisions.onGrindRail = false;
                    }
                }


            }
            if (canGrind)
            {
                if (collisions.climbingSlopeOld && collisions.onGrindRailOld)
                {
                    collisions.onGrindRail = true;
                }
            }


            if (collisions.climbingSlope)
            {
                float directionX = Mathf.Sign(moveAmount.x);
                rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
                Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

                if (hit)
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (slopeAngle != collisions.slopeAngle)
                    {
                        moveAmount.x = (hit.distance - skinWidth) * directionX;
                        collisions.slopeAngle = slopeAngle;
                        collisions.slopeNormal = hit.normal;
                    }


                }

                hit = Physics2D.Raycast(rayOrigin + new Vector2(0, skinWidth), Vector2.right * directionX, rayLength + skinWidth * 2, grindrailMask);
                if (hit)
                {
                    collisions.onGrindRail = true;
                }

                if (!canGrind&&collisions.onGrindRail)
                {
                    collisions.onGrindRail = false;
                    moveAmount.y = collisions.moveAmountOld.y;
                }
            }
            if (collisions.onGrindRail)
            {

       
                int hits = 0;
                float directionX = Mathf.Sign(moveAmount.x);
                Vector2 origin = raycastOrigins.bottomRight + (Vector2.up * .1f);
                RaycastHit2D hitRail = Physics2D.Raycast(origin, -Vector2.up, 2f, grindrailMask);
                if (hitRail)
                { hits++; }
                origin = raycastOrigins.bottomLeft + (Vector2.up * .1f);
                hitRail = Physics2D.Raycast(origin, -Vector2.up, 2f, grindrailMask);

                if (hitRail)
                { hits++; }

                if (hits<2)
                {
                    collisions.onGrindRail = false;
                    collisions.grounded = false;
                    collisions.below = false;
                    moveAmount = collisions.moveAmountOld;
                }

                //Debug.DrawRay(origin, -Vector2.up * 2f, Color.red, .5f);
                
            }



        }

        void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal)
        {
            float moveDistance = Mathf.Abs(moveAmount.x);
            float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

            if (moveAmount.y <= climbmoveAmountY)
            {
                moveAmount.y = climbmoveAmountY;
                moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                collisions.below = true;
                collisions.climbingSlope = true;
                collisions.slopeAngle = slopeAngle;
                collisions.slopeNormal = slopeNormal;
            }


        }

        void DescendSlope(ref Vector2 moveAmount)
        {

            RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
            RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
            if (maxSlopeHitLeft ^ maxSlopeHitRight)
            {
                SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
                SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
            }

            if (!collisions.slidingDownMaxSlope)
            {
                float directionX = Mathf.Sign(moveAmount.x);
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

                if (hit)
                {


                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                    {
                        if (Mathf.Sign(hit.normal.x) == directionX)
                        {
                            if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                            {
                                float moveDistance = Mathf.Abs(moveAmount.x);
                                float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                                moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                                moveAmount.y -= descendmoveAmountY;

                                collisions.slopeAngle = slopeAngle;
                                collisions.descendingSlope = true;
                                collisions.below = true;

                                collisions.slopeNormal = hit.normal;
                            }
                        }
                    }


                }
            }
        }

        void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
        {

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle > maxSlopeAngle)
                {
                    moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                    collisions.slopeAngle = slopeAngle;
                    collisions.slidingDownMaxSlope = true;
                    collisions.slopeNormal = hit.normal;
                }
            }

        }

        void ResetFallingThroughPlatform()
        {
            collisions.fallingThroughPlatform = false;
        }

        public struct CollisionInfo
        {
            public bool above, below;
            public bool left, right;
            public bool grounded;
            public bool onGrindRail;
            public bool onGrindRailOld;
            public bool onWallRun;
            public bool onWallRunRamp;
            public bool onWallRunOld;
            public bool jumpedFromWallRun;//Not set by aything (like pressing the jump key) yet

            public bool climbingSlope;
            public bool climbingSlopeOld;
            public bool descendingSlope;
            public bool slidingDownMaxSlope;

            public float slopeAngle, slopeAngleOld;
            public Vector2 slopeNormal;
            public Vector2 moveAmountOld;
            public int faceDir;
            public bool fallingThroughPlatform;

            public void Reset()
            {
                slopeAngleOld = slopeAngle;
                onGrindRailOld = onGrindRail;
                onWallRunOld = onWallRun;
                climbingSlopeOld = climbingSlope;
                slopeAngle = 0;

                above = below = false;
                left = right = false;
                climbingSlope = false;
                descendingSlope = false;
                slidingDownMaxSlope = false;
                slopeNormal = Vector2.zero;


            }
        }
    }
}
