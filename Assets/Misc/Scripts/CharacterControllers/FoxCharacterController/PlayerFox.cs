using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Fox
{
    [RequireComponent(typeof(ControllerFox), typeof(PlayerInput))]
    public class PlayerFox : MonoBehaviour
    {
        public float jumpForceOne = .4f;
        public float jumpForceTwo = .4f;
        public float jumpForceThree = .4f;
        public float jumpForceFour = .4f;
        public float minJumpHeight = 1;
        public float accelerationTimeAirborne = .2f;
        public float accelerationTimeGrounded = .1f;
        public float moveSpeed = 6;
        public float skidThreshold = 6;
        public float skidDuration = .1f;
        public float grindSpeed = 10;
        public float wallRunSpeed = 8;
        public float wallRunKeyStickTime = .2f;
        public float wallRunRampSlideSpeed;
        public float wallRunRampSlideTimeMult;
        public Vector2 wallRunJumpForce;
        public Vector2 minWallRunJumpForce;



        public float gravity;
        float maxJumpVelocityOne;
        float maxJumpVelocityTwo;
        float maxJumpVelocityThree;
        float maxJumpVelocityFour;
        float minJumpVelocity;
        int jumpIteration;

        Vector3 velocity;
        Vector3 velocityOld;
        Vector3 velocityOldTwo;
        Vector3 velocityOldThree;
        Vector3 velocityOldFour;
        float velocityXSmoothing;

        float currentSkidTime;
        float skidStartVelocity;
        bool isSkid;

        float grindDirection;
        float currentWallRunKeyStickTime;
        float currentWallRunRampSlideTime;

        ControllerFox controller;

        Vector2 directionalInput;
        Vector2 directionalInputOld;


        void Start()
        {
            controller = GetComponent<ControllerFox>();



            maxJumpVelocityOne = Mathf.Abs(gravity) * jumpForceOne;


            maxJumpVelocityTwo = Mathf.Abs(gravity) * jumpForceTwo;


            maxJumpVelocityThree = Mathf.Abs(gravity) * jumpForceThree;


            maxJumpVelocityFour = Mathf.Abs(gravity) * jumpForceFour;



            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
            jumpIteration = 0;

            currentSkidTime = 0;
            grindDirection = 0;
            currentWallRunKeyStickTime = 0;
            currentWallRunRampSlideTime = 0;
        }

        void Update()
        {
            CalculateVelocity();


            controller.Move(velocity * Time.deltaTime, directionalInput);

            if (controller.collisions.above || controller.collisions.below)
            {
                if (controller.collisions.slidingDownMaxSlope)//limit velocity if there is a collision
                {
                    velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
                }
                else
                {
                    velocity.y = 0;
                }

                if (controller.collisions.grounded && jumpIteration > 0)
                {
                    IncrimentJumpIteration(true);
                }
            }
            directionalInputOld = directionalInput;
            velocityOldFour = velocityOldThree;
            velocityOldThree = velocityOldTwo;
            velocityOldTwo = velocityOld;
            velocityOld = velocity;
        }

        public void SetDirectionalInput(Vector2 input)
        {
            directionalInput = input;
        }

        public void OnJumpInputDown(bool jumpWithoutGround)
        {
            if (!controller.collisions.onWallRun||(controller.collisions.onWallRun&&controller.collisions.onWallRunRamp))
            {
                if (controller.collisions.below || jumpWithoutGround)
                {

                    if (controller.collisions.slidingDownMaxSlope)
                    {
                        if (controller.collisions.onWallRunRamp)
                        {
                            controller.jumpedOnRamp = true;
                        }
                        if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
                        { // not jumping against max slope
                            if (jumpIteration == 0)
                            {
                                velocity.y = maxJumpVelocityOne * controller.collisions.slopeNormal.y;
                                velocity.x = maxJumpVelocityOne * controller.collisions.slopeNormal.x;
                            }
                            if (jumpIteration == 1)
                            {
                                velocity.y = maxJumpVelocityTwo * controller.collisions.slopeNormal.y;
                                velocity.x = maxJumpVelocityTwo * controller.collisions.slopeNormal.x;
                            }
                            if (jumpIteration == 2)
                            {
                                velocity.y = maxJumpVelocityThree * controller.collisions.slopeNormal.y;
                                velocity.x = maxJumpVelocityThree * controller.collisions.slopeNormal.x;
                            }
                            if (jumpIteration == 3)
                            {
                                velocity.y = maxJumpVelocityFour * controller.collisions.slopeNormal.y;
                                velocity.x = maxJumpVelocityFour * controller.collisions.slopeNormal.x;
                            }
                        }
                    }
                    else
                    {
                            controller.jumpedOnRamp = true;


                        if (jumpIteration == 0)
                        {
                            velocity.y = maxJumpVelocityOne;
                        }
                        if (jumpIteration == 1)
                        {
                            velocity.y = maxJumpVelocityTwo;

                        }
                        if (jumpIteration == 2)
                        {
                            velocity.y = maxJumpVelocityThree;

                        }
                        if (jumpIteration == 3)
                        {
                            velocity.y = maxJumpVelocityFour;

                        }
                    }

                }
            }
            else if (!controller.collisions.jumpedFromWallRun)
            {

                jumpIteration = 0;
                velocity.y = wallRunJumpForce.y;
                velocity.x = wallRunJumpForce.x * Mathf.Sign(directionalInput.x);
                controller.collisions.jumpedFromWallRun = true;
            }


        }

        public void OnJumpInputUp()
        {
            if (!controller.collisions.onWallRun)
            {
               
                if (velocity.y > minJumpVelocity)
                {
                    velocity.y = minJumpVelocity;
                }
            }
            else
            {
                if (velocity.y > minWallRunJumpForce.y)
                {
                    velocity.y = minWallRunJumpForce.y;
                }
                if (Mathf.Abs(velocity.x) > minWallRunJumpForce.x)
                {
                    velocity.x = (Mathf.Sign(directionalInput.x) == 1) ? minWallRunJumpForce.x : -minWallRunJumpForce.x;
                }
            }
        }

        public void IncrimentJumpIteration(bool setToZero)
        {
            if (setToZero)
            {
                jumpIteration = 0;
                return;
            }

            if (jumpIteration < 3)
            {
                jumpIteration++;
            }
        }

        void CalculateVelocity()
        {
            float targetVelocityX = directionalInput.x * moveSpeed;

            if (!HandleGrind(targetVelocityX) && !HandleWallRun(targetVelocityX))
            {
                if (!HandleSkid(targetVelocityX))
                {
                    velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
                }
            }
            float someNumber = 5;
            float anothernUmber = 20;
            float solution = Add(someNumber,anothernUmber);

            float Add(float num1, float num2)
            {
                return num1 + num2;
            }


            velocity.y += gravity * Time.deltaTime;
        }

        

        bool HandleSkid(float targetVelocityX)
        {
            if (!isSkid)//check to see if we need to start skidding
            {
                if (Mathf.Abs(velocity.x) >= skidThreshold)
                {
                    if (targetVelocityX == 0 || Mathf.Sign(targetVelocityX) != Mathf.Sign(velocity.x))
                    {
                        isSkid = true;
                        skidStartVelocity = velocity.x;
                        currentSkidTime = 0;
                    }
                }
            }

            if (isSkid)//perform skid
            {
                if (controller.collisions.left || controller.collisions.right)//if we collide with something, terminate skid
                {
                    isSkid = false;
                    return isSkid;
                }

                if (currentSkidTime < skidDuration)
                {

                    velocity.x = Mathf.Lerp(skidStartVelocity, 0, currentSkidTime / skidDuration);
                    currentSkidTime += Time.deltaTime;
                }
                else
                {
                    isSkid = false;
                }
            }

            return isSkid;

        }

        bool HandleGrind(float targetVelocityX)
        {
            bool isGrinding = false;
            if (controller.collisions.onGrindRail)
            {
                if (grindDirection == 0)
                {
                    grindDirection = Mathf.Sign(targetVelocityX);
                }
                velocity.x = grindSpeed * grindDirection;
                isGrinding = true;
            }
            else
            {
                grindDirection = 0;

            }

            return isGrinding;
        }

        bool HandleWallRun(float targetVelocityX)
        {

            bool isWallrunning = false;
            controller.CheckWallRunThreashold(velocity);
            if(controller.collisions.onWallRunRamp)
            {
                float speed = Mathf.Abs(velocity.x)+velocity.y;
                
                if(controller.wallRunThreashold>speed)
                {
                    currentWallRunRampSlideTime += Time.deltaTime* wallRunRampSlideTimeMult;

                    if (controller.collisions.slopeNormal.x<0)
                    {
                        if(directionalInput.x==-1||velocityOldFour.y<-1)
                        {
                            currentWallRunRampSlideTime = 1;

                            if (velocity.x>-1)
                            {
                                velocity.x = -4;
                            }
                        }
                        if(directionalInput.x==0)
                        {
                            if (velocity.x > -1)
                            {
                                velocity.x = -2;
                            }
                        }

               

                        velocity.x -= wallRunRampSlideSpeed*currentWallRunRampSlideTime;
                      

                    }
                    else
                    {
                       if(directionalInput.x==1||velocityOldFour.y<-1)
                        {
                            currentWallRunRampSlideTime = 1;

                            if (velocity.x<1)
                            {
                                velocity.x = 4;
                            }
                        }
                        if (directionalInput.x == 0)
                        {
                            if (velocity.x < 1)
                            {
                                velocity.x = 2;
                            }
                        }
                       

                        velocity.x += wallRunRampSlideSpeed*currentWallRunRampSlideTime;

                    }
                }
                return true;
            }
            else
            {
                currentWallRunRampSlideTime =0;

            }

            if (controller.collisions.onWallRun)
            {
                if ((directionalInput.x != directionalInputOld.x||directionalInput.x==0) && !controller.collisions.jumpedFromWallRun&&!controller.collisions.onWallRunRamp)
                {
                    if (currentWallRunKeyStickTime < wallRunKeyStickTime)
                    {
                        currentWallRunKeyStickTime += Time.deltaTime;
                        directionalInput.x = directionalInputOld.x;
                        targetVelocityX = directionalInput.x * moveSpeed;

                    }
                    else
                    {
                        controller.collisions.onWallRun = false;
                    }
                }
                else
                {
                    currentWallRunKeyStickTime = 0;
                }

                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
                isWallrunning = true;
            }
            return isWallrunning;
        }
    }


}


