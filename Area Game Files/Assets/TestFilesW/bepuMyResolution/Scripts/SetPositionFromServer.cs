using UnityEngine;
using System.Collections.Generic;
//using VelcroPhysics.Dynamics;
//using BulletSharp.Math;
//using BulletSharp;

namespace Dim
{
   public struct dataPlayer
    {
        public ushort step;
        public float time;
        public BEPUutilities.Vector3 move;
        public BEPUutilities.Vector3 pos;
        public float rotYaw;
        public float rotPitch;
        public bool jump;
        public bool send;
        public bool shoot;
        public bool usedShoot;

    }

    public class SetPositionFromServer : MonoBehaviour
    {

        public LineRenderer ownerShootPrefab, otherShootPrefab;
        dataPlayer[] data = new dataPlayer[40];
        List<dataPlayer> sendButNotBackDataList = new List<dataPlayer>();
        // Microsoft.Xna.Framework.Vector2 receivePost;
        BEPUutilities.Vector3 receivePost;

        public MoveControl moveControl;
        public float recalculatePhysicIfDifr = 0.000000001f;
        UnityEngine.Vector3 helpPos = UnityEngine.Vector3.zero;
        
        float lastSendTime = 0;
        float pingToNotOwner;
        ushort lastSendStep = 0;
        ushort lastReceiveStep = 0;
        BEPUphysics.Character.SphereCharacterController sphereChar;
        // BulletSharp.KinematicCharacterController charBody;
        // BulletSharp.DynamicsWorld world;
        BEPUphysics.Space space;


        //VelcroPhysics.Dynamics.Body body;
        //VelcroPhysics.Dynamics.World world;
        bool isNotMy = true;
        //public Transform bodyToWatchServer;


        public void DestroyBody()
        {
            space.Remove(sphereChar);
           // world.RemoveCollisionObject(charBody.
        }

        public void SendedData(dataPlayer dp)
        {
            SortDataFromIndex();
            data[0] = dp;

           // Debug.Log(dp.time + "      " + dp.step);
            //sendButNotBackDataList.Add(dp);
        }

        bool CheckIfStepExist(ushort step)
        {
            for (int i = 0; i < data.Length; i++)
                if (data[i].step == step)
                {
                    return true;
                }
            return false;
        }

        void FillIfExist(ushort step, float posX, float posY, float posZ, float time)
        {
            for (int i = 0; i < data.Length; i++)
                if (data[i].step == step)
                {
                    receivePost.X = posX;
                    receivePost.Y = posY;
                    receivePost.Z = posZ;


                    if (data[i].pos != receivePost)
                    {
                       // Debug.Log(data[i].time + "    a    " + time);
                        //Debug.Log("Poprawiam");
                        // Debug.Log(data[i].pos + "   a   " + receivePost);
                        //Debug.Log(data[i].step + "    a    " + step);

                        if (BEPUutilities.Vector3.DistanceSquared(data[i].pos, receivePost) > recalculatePhysicIfDifr)
                        {
                           // Debug.Log("duza roznica");
                            data[i].pos = receivePost;
                            sphereChar.Body.Position = receivePost;
                           // sphereChar.Body.LinearVelocity = BEPUutilities.Vector3.Zero;
                            //entity.linearVelocity = BEPUutilities.Vector3.Zero;
                            //wyliczenia błędne trzeb wtedy przeliczyć wszystkie stepy jeszcze raz
                            for (int k = i - 1; k >= 0; k--)
                            {
                                //body.LinearVelocity = data[k].move * 10f;
                                // entity.linearVelocity = data[k].move * 10f;

                                //entity.ApplyImpulse(entity.position, data[k].move * 1f);
                                sphereChar.HorizontalMotionConstraint.MovementDirection = new BEPUutilities.Vector2(data[k].move.X, data[k].move.Z);
                                space.Update(Time.fixedDeltaTime);
                                data[k].pos = sphereChar.Body.Position;

                            }
                        }
                        //else
                        //{
                        //    Debug.Log("mala roznica");
                        //    body.Position -= (receivePost - data[i].pos);
                        //    data[i].pos = receivePost;
                            
                        //}
                    }
                    return;
                }
        }


        void SetShootInStep(ushort step)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].step == step)
                {
                    data[i].shoot = true;
                    return;
                }
            }
        }


        void Shoot(ushort step)
        {
           // Debug.Log("jestem");
            for (int i = 0; i < data.Length; i++)
            {
              //  Debug.Log(data[i].step + "     a     " + step);

                if (data[i].step == step)
                {
                   // Debug.Log("cos");
                    //if (data[i].shoot && !data[i].usedShoot)
                        if (!data[i].usedShoot)
                        {
                        //Debug.Log("bedzie");
                        data[i].usedShoot = true;
                        RaycastWeapon(data[i]);
                        //rajcast shoot
                    }
                }
            }
        }


        UnityEngine.Vector3 posStart, posCheck, posEnd;

        void RaycastWeapon(dataPlayer player)
        {
            //BulletSharp.Math.Vector3 one = body.WorldTransform.Origin;
            //posStart.x = one.X;
            //posStart.y = one.Y;
            //posStart.z = one.Z;

            //BulletSharp.Math.Vector3 v2;

            //BulletSharp.Math.Vector3 v = body.WorldTransform.Orientation.Axis;// new Microsoft.Xna.Framework.Vector2(Mathf.Cos(player.rot), Mathf.Sin(player.rot));
            //v.Normalize();
            //v2 = v * -0.01f;
            //v *= 20f;

            //posCheck.x = (one + v).X;
            //posCheck.y = (one + v).Y;

            //int count = world.RayCast(one, one + v).Count;

            //if (count > 0)
            //{
            //    if (count > 1)
            //    {
            //        hitPoint = one + v;
            //        for (int i = 0; i < count; i++)
            //        {
            //            if (i == 0)
            //                world.RayCast(ReportRayFixture2, one, hitPoint);
            //            else
            //                world.RayCast(ReportRayFixture2, one, hitPoint + v2);
            //        }

            //    }
            //    else
            //    {
            //        world.RayCast(ReportRayFixture2, one, one + v);
            //    }

            //    DrawLine(posEnd);
            //}
            //else
            //{
            //    DrawLine(posCheck);
            //}
        }

        //float distance = 0;
        //BulletSharp.Math.Vector3 hitPoint = BulletSharp.Math.Vector3.Zero;



        //float ReportRayFixture2(Fixture fixture,
        //                  Microsoft.Xna.Framework.Vector2 point,
        //                  Microsoft.Xna.Framework.Vector2 normal,
        //                  float fraction)
        //{
        //    hitPoint = point;
        //    SetEndPoint();
        //    return 0;
        //}


        //void SetEndPoint()
        //{
        //    posEnd.x = hitPoint.X;
        //    posEnd.y = hitPoint.Y;
        //}


        //private void DrawLine(Vector3 endPos)
        //{
        //    LineRenderer lineRend = Instantiate(otherShootPrefab);
        //    lineRend.SetPosition(0, posStart);
        //    lineRend.SetPosition(1, endPos);
        //    Destroy(lineRend.gameObject, .2f);
        //}

        void FillIfExistNotOwner(ushort step, float posX, float posY, float posZ, float moveX, float moveY, float rotYaw, float rotPitch, ushort lastShootStep, ushort st, bool shoot, float time)
        {
        //    for (int i = 0; i < data.Length; i++)
               // if (data[i].step == step)
                {
                    receivePost.X = posX;
                    receivePost.Y = posY;

                    //body.WorldTransform = Matrix.Translation(receivePost);
                sphereChar.Body.Position = receivePost;
                    transform.position = new UnityEngine.Vector3(posX, posY, posZ);
                    transform.eulerAngles = new UnityEngine.Vector3(rotYaw, rotPitch);

               // Debug.Log(st);

                Shoot(lastShootStep);

                //if (ServerNetworkStats.delay > ServerNetworkStats.timeStep)
                //{

                //}

                dataPlayer data2 = new dataPlayer();
                data2.move.X = moveX;
                data2.move.Y = moveY;
                data2.rotYaw = rotYaw;
                data2.rotPitch = rotPitch;
                data2.step = st;
                data2.shoot = shoot;
                data2.usedShoot = false;

                //Debug.Log(data2.step);


                SendedData(data2);
                  //  Debug.Log(transform.position);
                }
        }

        void SortDataFromIndex(int ind = 0)
        {
            for (int i = data.Length - 2; i >= ind; i--)
            {
                data[i + 1] = data[i];
            }
        }

        void PushData(int ind, float moveX, float moveY, float rotYaw, float rotPitch, float calcPosX,
            float calcPosY, float ownTime, ushort step, ushort lastCalculateStep)
        {
            data[ind].step = step;
            data[ind].move.X = moveX;
            data[ind].move.Y = moveY;
            data[ind].rotYaw = rotYaw;
            data[ind].rotPitch = rotPitch;
            data[ind].time = Time.time - (Time.time - ownTime);

          //  FillIfExist(lastCalculateStep, calcPosX, calcPosY);
        }


        public void ReciveOwnerMessage(ushort calcStep, float posX, float posY, float posZ, float time)
        {
            FillIfExist(calcStep, posX, posY, posZ, time);
        }

        public void ReciveOtherPlayerMessage(ushort calcStep, float posX, float posY, float posZ, float moveX, float moveY, float rotYaw, float rotPitch, ushort lastShootSte, ushort step,bool shoot, float time)
        {
            FillIfExistNotOwner(calcStep, posX, posY, posZ,moveX, moveY, rotYaw, rotPitch,lastShootSte, step, shoot,time);
        }


        public void ReceiveOwnMessage(float moveX, float moveY, float calcPosX, 
            float calcPosY, float ownTime, ushort step, ushort lastCalculateStep)
        {
            if (!CheckIfStepExist(step))
            {

                if (step > lastReceiveStep)
                {
                    SortDataFromIndex(0);
                   // PushData(0, moveX, moveY, calcPosX, calcPosY, ownTime, step, lastCalculateStep);
                    lastReceiveStep = step;
                }
                else
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (step > data[i].step)
                        {
                            SortDataFromIndex(i);
                           // PushData(i, moveX, moveY, calcPosX, calcPosY, ownTime, step, lastCalculateStep);
                            return;
                        }
                    }
                    
                }
            }
        }

        public void ReceiveOtherMessage(float moveX, float moveY, float calcPosX,
            float calcPosY, float timeDelay, ushort step, ushort lastShoot, ushort lastCalculateStep)
        {

        }


        public void InitBody( BEPUphysics.Character.SphereCharacterController ent,  BEPUphysics.Space spa)
        {
            sphereChar = ent;
            space = spa;
          //  bodyToWatchServer.SetParent(null);
            if (moveControl != null)
            {
                moveControl.InitWorldAndBody( spa, ent);
                isNotMy = false;
            }
        }

        public void SetPosition(float x, float y)
        {
            
            helpPos.x = x;
            helpPos.y = y;
            //Debug.Log(helpPos);
            //transform.position = helpPos;
            
        }

        private void Update()
        {
            //ustawiasz cień innych graczy w przesłanym przez serwer miejscu (uwaga jest to pozycja z czasu opóźnionego o interpolację + opóźnienie) a wieć przy obecnych ustawienia
            //serwera pozycja jest z ponad 300 milisekund (interpolacja(300) + opóźnienie od serwera)
           // bodyToWatchServer.position = helpPos;
        //transform.position = Vector3.LerpUnclamped(transform.position, helpPos, Time.deltaTime * 5f);
    }


        private void FixedUpdate()
        {
            if (isNotMy)
            {
                //kalkuluj pozycję innych graczy w oparciu o przesłane do Ciebie od serwera ruchy

            }
            else
            {

            }
        }

        

        public void SetDelayTime(float time)
        {
            moveControl.SetDelayTime(time);
        }

        public void SetLastPlayerStep(ushort step)
        {

            moveControl.SetReceivedStep(step);
        }


        public void SetDelayTimeForOther(float time, ushort step)
        {
            if (step > lastSendStep)
            {
                lastSendStep = step;
                pingToNotOwner = (time - lastSendTime) / 2f;
                lastSendTime = time;
            }
        }
        //public void SetLastStep()
        //{

        //}
    }
}
