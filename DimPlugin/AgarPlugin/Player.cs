using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUutilities;
using BEPUphysics.Entities;
using BEPUphysics;
using BEPUphysics.Character;

//using VelcroPhysics.Dynamics;
//using Microsoft.Xna.Framework;
//using BulletSharp;
//using BulletSharp.Math;


namespace Dim
{
    public class PlayerInputs
    {


        ushort damageTaken, damageDelivered;
        bool change = false;
        SomePhysic sP;
        ushort pistoStepBreak;
        ushort stepBreak;
        public ushort ID;
        public float moveHorizontal;
        public float moveVertical;
        public bool shoot;
        public bool waitForDim;
        public float posX;
        public float posY;
        public float posZ;
        public float rot;
        public int playerHealth;
        ushort lastShootStep;

        float interpolation;

        Inputs[] inputs;
        Inputs zeroInput;
        Inputs lastInput, beforeLastInput;
        int lastInputIndex;
        int endIndex;
        bool firstTime;

        SphereCharacterController spherChar;
        Space space;

        Inputs helpInput;
        Vector3 helpVec;
        Vector3 applyVec;

       // ushort stepToCalculation = 1;
        ushort lastReceiveStep=1;
        ushort lastCalculatedStep = 1;
        ushort tempCalc;


        public void ResetDamage()
        {
             damageTaken = damageDelivered = 0;
             change = false;
        }

        public bool Change
        {
            get
            {
                if (change)
                {
                    change = false;
                    return !change;
                }
                return change;

            }
        }


        public ushort DamageDeliverdd()
        {
            return damageDelivered;
        }

        public ushort DamageTaken()
        {
            return damageTaken;
        }


        public struct Inputs
        {
            public float moveX, moveY;
            public float positX, positY, positZ;
            public bool jump, shoot, waitForDim;
            public float rotYaw, rotPitch;
            public float inputTime;
            public float clientTime;
            public ushort clientStep;
            public float releaseTime;
            public bool used;
            public bool shootInThisStep;
           // public bool change;
        }


        public bool CheckIfStepNotExist(ushort step)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                if (step == inputs[i].clientStep)
                    return false;
            }
            return true;
        }


        public void SetCalculatedPosition()
        {
            posX = spherChar.Body.Position.X;
            posY = spherChar.Body.Position.Y;
            posZ = spherChar.Body.Position.Z;
            lastCalculatedStep = tempCalc;
        }


        public void BodySetMove(float fromTime)
        {
            //GetLastInputStep(fromTime, ref helpInput);
            //helpVec.X = helpInput.moveX;
            //helpVec.Y = helpInput.moveY;
            GetLastInputStep(fromTime);
            helpVec *= 1f;
            spherChar.HorizontalMotionConstraint.MovementDirection = new Vector2(helpVec.X, helpVec.Z);
            //applyVec = new Vector3(helpVec.X, helpVec.Y, helpVec.Z);
          //  entity.ApplyImpulse(entity.Position, helpVec);
            //entity.ApplyLinearImpulse(ref applyVec);
        }


        //void RaycastWeapon(Inputs input)
        //{
        //    if (input.shoot)
        //    {
                
        //       List<Fixture> list =  world.RayCast(body.Position, input.rot * Vector2.UnitX * 100f);
        //        foreach (Fixture fi in list)
        //        {
        //            fi.OnCollision = OnCol;
        //        }
        //    }
        //}

        //void OnCol(Fixture a, Fixture b, VelcroPhysics.Collision.ContactSystem.Contact cont)
        //{
            
        //}

        //VelcroPhysics.Collision.Handlers.OnCollisionHandler Func()
        //{

        //}

        public ushort GetLastCalculatedStep()
        {
            return lastCalculatedStep;
        }



        public float GetLastInputStepTime(ushort step)
        {
            for (int i = 0; i < inputs.Length; i++)
                if (inputs[i].clientStep ==step)
                {
                    return inputs[i].inputTime; ;
                }
            return 0;
            //lastCalculatedStep = 0;
        }

        public float GetLastInputStepTime()
        {
            for (int i = 0; i < inputs.Length; i++)
                if (inputs[i].clientStep == lastCalculatedStep)
                {
                    return inputs[i].clientTime;
                }
            return 0;
            //lastCalculatedStep = 0;
        }


        void TryShoot(ref Inputs input)
        {
                if (stepBreak > 0)
                {
                    stepBreak++;
                }

                if (stepBreak >= pistoStepBreak)
                    stepBreak = 0;

                if (input.shoot && stepBreak == 0)
                {
                //dupa na razie nie ma strzelania
                   // RaycastWeaponShoot(input);
                    input.shootInThisStep = true;
                    lastShootStep = input.clientStep;
                    stepBreak = 1;
                }           
        }

        public ushort GetLastShootStep()
        {
            return lastShootStep;
        }

        void GetLastInputStep(float fromTime)
        {
            helpVec = Vector3.Zero;
            for (int i = inputs.Length - 1; i >= 0; i--)
                if (inputs[i].releaseTime <= fromTime && !inputs[i].used)
                {
                    TryShoot(ref inputs[i]);
                    inputs[i].used = true;
                    helpVec.X += inputs[i].moveX;
                    helpVec.Z += inputs[i].moveY;
                    tempCalc = inputs[i].clientStep;
                    
                    
                    
                    //return;
                }
           // return helpVec;

            //inp = zeroInput;
            //lastCalculatedStep = 0;
        }

        void GetLastInputStep(float fromTime, ref Inputs inp )
        {
            helpVec = Vector3.Zero;
            int steps = 0;

            for (int i = inputs.Length-1; i >=0; i--)
                if (inputs[i].releaseTime <= fromTime && !inputs[i].used)
                {
                    steps++;
                    if (steps >= 2)
                        Console.WriteLine(steps);
                    inputs[i].used = true;
                    inp = inputs[i];
                    helpVec.X += inputs[i].moveX;
                    helpVec.Z += inputs[i].moveY;
                    tempCalc = inputs[i].clientStep ;
                    //return;
                }
            inp = zeroInput;
            //lastCalculatedStep = 0;
        }


        public PlayerInputs(SomePhysic someP, ushort id, float moveHor, float moveVer, bool shoo, bool waitFor, float positionX, float positionY, float positionZ, float rotation,SphereCharacterController body, Space worl,float interp, ushort pistolStepBr, int inputsSize = 30)
        {
            sP = someP;
            ID = id;
            moveHorizontal = moveHor;
            moveVertical = moveVer;
            shoot = shoo;
            waitForDim = waitFor;
            posX = positionX;
            posY = positionY;
            posZ = positionZ;
            rot = rotation;
            spherChar = body;
            space = worl;
            interpolation = interp;
            pistoStepBreak = pistolStepBr;

            inputs = new Inputs[inputsSize];
            endIndex = inputsSize - 1;
           // lastInput = new Inputs();
           // beforeLastInput = new Inputs();
            lastInputIndex = 0;
            firstTime = true;
        }

        public void RemoveBody()
        {
            space.Remove(spherChar);
            Console.WriteLine("Usuniety");
        }


        void SortInputs(int to)
        {
            for (int i = inputs.Length - 2; i >= to; i--)
            {
                inputs[i + 1] = inputs[i];
            }
        }


        public void RegisterInput(float x, float y, float posX, float posY,float posZ, float time,float clientTime,bool jump, bool shoot, float rotYaw, float rotPitch, ushort step)
        {
            if (step < lastReceiveStep)
            {
                for (int i = 0; i <inputs.Length; i++)
                {
                    if(inputs[i].clientStep<step)
                    {
                        SortInputs(i);
                        PushInput(i, x, y, posX, posY, posZ, time,clientTime,jump, shoot, rotYaw, rotPitch, step);
                        return;
                    }
                }
            }
            else
            {
                lastReceiveStep = step;
                SortInputs(0);
                PushInput(0, x, y, posX, posY, posZ, time, clientTime, jump, shoot, rotYaw, rotPitch, step);
            }
        }


        void PushInput(int index, float x, float y, float posX, float posY, float posZ, float time, float clientTime, bool jump, bool shoot, float rotYaw, float rotPitch, ushort step)
        {
            inputs[index].moveX = x;
            inputs[index].moveY = y;
            inputs[index].positX = posX;
            inputs[index].positY = posY;
            inputs[index].positZ = posZ;
            inputs[index].inputTime = time;
            inputs[index].clientStep = step;
            inputs[index].releaseTime = time + interpolation;
            inputs[index].clientTime = clientTime;
            inputs[index].used = false;
            inputs[index].jump = jump;
            inputs[index].shoot = shoot;
            inputs[index].rotYaw = rotYaw;
            inputs[index].rotPitch = rotPitch;
            //inputs[index].sh
        }

        float DegToRad(float deg)
        {
            return (float)((deg / 360f) * (2 * Math.PI));
        }

        //Vector2 hitPoint;
        //Body toHit;

        //void RaycastWeaponShoot(Inputs input)
        //{
        //    Vector2 one = body.Position;
        //    Vector2 v2;
        //    Vector2 v = new Vector2((float)(Math.Cos(input.rot)), (float)(Math.Sin(input.rot)));
        //    v.Normalize();
        //    v2 = v * -0.01f;
        //    v *= 20f;

        //    int count = world.RayCast(one, one + v).Count;

        //    if (count > 0)
        //    {
        //        if (count > 1)
        //        {
        //            hitPoint = one + v;
        //            for (int i = 0; i < count; i++)
        //            {
        //                if (i == 0)
        //                    world.RayCast(ReportRayFixture2, one, hitPoint);
        //                else
        //                    world.RayCast(ReportRayFixture2, one, hitPoint + v2);
        //            }
        //        }
        //        else
        //        {
        //            world.RayCast(ReportRayFixture2, one, one + v);
        //        }

        //        HitPlayer(toHit);
        //    }
        //}

        //float ReportRayFixture2(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
        //{
        //    hitPoint = point;
        //    toHit = fixture.Body;
        //    return 0;
        //}

        //void HitPlayer(Body body)
        //{
        //    if(body.BodyType == BodyType.Dynamic)
        //    if (sP.TryHitPlayer(body))
        //    {
        //        damageDelivered += 20;
        //        change = true;
        //    }
        //}

        //public bool HitSingle(Body b)
        //{
        //    if (body == b)
        //    {
        //        change = true;
        //        damageTaken += 20;
        //        return true;
        //    }
        //    return false;
        //}

        //public ushort GetDamageDeliveredAmount()
        //{
        //    return damageDelivered;
        //}
    }
}
