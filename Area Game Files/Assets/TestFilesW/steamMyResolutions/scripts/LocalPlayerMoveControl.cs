using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using System.Collections.Generic;
using UnityEngine;
//using VelcroPhysics.Dynamics;

using System;


namespace wolfingame.steamTest
{

    static class DimTag
    {
        public static readonly ushort SpawnPlayerTag = 0;
        public static readonly ushort MovePlayerTag = 1;
        public static readonly ushort DespawnPlayerTag = 2;
        public static readonly ushort TimeTag = 3;

        public static readonly ushort DeliveryDamageInfo = 4;
        public static readonly ushort DeliveryDamageBreakStart = 5;
        public static readonly ushort DeliveryDamageMatchStart = 6;
        public static readonly ushort DeliveryDamageMatchEndStart = 7;

    }

    public class LocalPlayerMoveControl : MonoBehaviour
    {
        struct TimerCheck
        {
            public int index;
            public float sendTime;
            public float reciveTime;
        }


        private NetworkTag networkTag;
        public NetworkTarget target;
        private void Start()
        {
            networkTag = GetComponent<NetworkTag>();

        }



        BEPUphysics.Space space;
        BEPUphysics.Character.SphereCharacterController sphereChar;//, entity2;
        //BulletSharp.DynamicsWorld world;
        //BulletSharp.RigidBody body;

        public int howManyPacketForTimeCheck = 1;
       // public SetPositionFromServer setPosFromServer;

        int TimerPacketRecived;
        float timeDelay;
        TimerCheck[] timerCheck;
        Vector3 moveDirection;
        UnityClient uC;
        ushort thisClentstep;
        ushort lastReceivedFormServerSte;
        bool run = false;
        ushort lastStepWhichComeBack;
        bool shoot = false;

        BEPUutilities.Vector3 helpVec, applyVec;

        Vector3 mousePos, mouseWorldPos, toSet;
        //dataPlayer toSend;

        public void InitWorldAndBody(BEPUphysics.Space w, BEPUphysics.Character.SphereCharacterController b)
        {
            space = w;
            sphereChar = b;

            //space.Add(entity2);
            //bodyToWatchServer.SetParent(null);
        }

        public UnityClient Clientt
        {
            get
            {
                return uC;
            }
            set
            {
                uC = value;
                uC.MessageReceived += MessageTimeRecive;
                timerCheck = new TimerCheck[howManyPacketForTimeCheck];
                TimeCheck();
            }
        }


        void SendTimeCheck(ushort index)
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(index);
                writer.Write(Time.time);

                using (Message message = Message.Create(DimTag.TimeTag, writer))
                {
                    uC.SendMessage(message, SendMode.Reliable);
                }
            }
        }

        void TimeCheck()
        {
            TimerPacketRecived = 0;
            for (ushort i = 0; i < howManyPacketForTimeCheck; i++)
            {
                SendTimeCheck(i);
            };
        }

        //  Transform tr;

      //  private void Start()
       // {
            //tr = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
            //entity2 = new BEPUphysics.Entities.Entity(new BEPUphysics.CollisionShapes.ConvexShapes.SphereShape(1f), 1f);
            //entity2.position = new BEPUutilities.Vector3(0, 2, 0);
            // space.Add(entity);
            //  TimeCheck();
       // }


        void MessageTimeRecive(object sender, MessageReceivedEventArgs e)
        {
            using (Message message = e.GetMessage() as Message)
            {

                // Debug.Log("message " + message.Tag);
                if (message.Tag == DimTag.TimeTag)
                {
                    using (DarkRiftReader reader = message.GetReader())
                    {
                        ushort index = reader.ReadUInt16();
                        float sendTime = reader.ReadSingle();
                        Time.fixedDeltaTime = reader.ReadSingle();
                        ServerNetworkStats.timeStep = Time.fixedDeltaTime;
                        ServerNetworkStats.interpolationFromServer = reader.ReadSingle();

                        // float recTime = reader.ReadSingle();


                        timerCheck[index].index = index + 1;
                        timerCheck[index].sendTime = sendTime;
                        timerCheck[index].reciveTime = Time.time;
                        TimerPacketRecived++;
                        CalculateTimeDelay();
                        //Debug.Log("Otrzymalem");
                        //Vector3 newPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), 0);
                    }
                }
                //else if (message.Tag == Tags.SetRadiusTag)
                //{
                //    using (DarkRiftReader reader = message.GetReader())
                //    {
                //        ushort id = reader.ReadUInt16();

                //        if (networkPlayers.ContainsKey(id))
                //            networkPlayers[id].SetRadius(reader.ReadSingle());
                //    }
                //}
            }
        }


        void CalculateTimeDelay()
        {
            // if (TimerPacketRecived >= howManyPacketForTimeCheck - 1)
            //{
            // uC.MessageReceived -= MessageTimeRecive;
            float timek = 0;
            int sub = 0;

            for (int i = 0; i < howManyPacketForTimeCheck; i++)
            {
                if (timerCheck[i].index != 0)
                {
                    timek += (timerCheck[i].reciveTime - timerCheck[i].sendTime);
                    sub++;
                }
            }

            timeDelay = (timek / 2f) / sub;
            run = true;
            // Debug.Log("Time delay " + timeDelay);

            // }
        }

        public void SetDelayTime(float tim)
        {
            timeDelay = (Time.time - tim) / 2f;
        }

        public void SetReceivedStep(ushort step)
        {
            lastReceivedFormServerSte = step;
        }


        private void Update()
        {




            //body.

            if (run)
            {
                mousePos = Input.mousePosition;
                //mousePos.z = 0;

                mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);

                toSet.z = Mathf.Atan2((mouseWorldPos.y - transform.position.y), (mouseWorldPos.x - transform.position.x)) * Mathf.Rad2Deg;
                // transform.eulerAngles = toSet;

                if (Input.GetButton("Fire1"))
                {
                    shoot = true;
                }
                else
                {
                    shoot = false;
                }

                //if(Unitye
                //   bodyToWatchServer.position = Vector3.LerpUnclamped(transform.position, moveDirection, Time.deltaTime * 100f);
                //  bodyToWatchServer.eulerAngles = toSet;
            }

            //if (autoAim)
            //    AutoAim();

            //if (nearestEnemy == null)
            //{
            //    mousePos = Input.mousePosition;
            //    mousePos.z -= Camera.main.transform.position.z;
            //    mousePosWorld = Camera.main.ScreenToWorldPoint(mousePos);
            //}

            //toSet.z = Mathf.Atan2((mousePosWorld.y - transform.position.y), (mousePosWorld.x - transform.position.x)) * Mathf.Rad2Deg - 90;
            //transform.eulerAngles = toSet;

            //timer += Time.deltaTime;

            //if (Input.GetButton("Fire1") && timer >= reloadTime)
            //{
            //    Shoot(transform.rotation);
            //}
        }

        public BEPUphysics.Character.SphereCharacterController GetBody()
        {
            return sphereChar;
        }



        //private void FixedUpdate()
        //{
        //    float x = 0f;
        //    if (Input.GetKey(KeyCode.A)) x -= speed;
        //    if (Input.GetKey(KeyCode.D)) x += speed;
        //    networkTag.Call(Move, target, new BitStream().Write(x));
        //}

        [NetworkCall]
        private void Move(BitStream stream, SteamPlayer sender)
        {
            float x = stream.ReadFloat();
            float y = stream.ReadFloat();
            transform.position += new Vector3(x, y);
            Debug.Log(x + "    " + y);
        }

        void FixedUpdate()
        {
            if (run)
            {
                //{
                //    BEPUutilities.Vector3 vec = new BEPUutilities.Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
                //    vec *= 1;
                //    //entity.ApplyLinearImpulse(ref vec);
                //    entity2.ApplyImpulse(entity.position, vec);
                //    tr.position = new Vector3(entity2.Position.X, entity2.Position.Y, entity.Position.Z);
                //}

                // Debug.Log("Send");
                moveDirection.x = Input.GetAxisRaw("Horizontal");
                moveDirection.z = -Input.GetAxisRaw("Vertical");
                moveDirection.Normalize();

                helpVec.X = moveDirection.x;
                helpVec.Z = moveDirection.z;
                //helpVec *= 1;
                //BEPUutilities.Vector3 applyVec1 = new BEPUutilities.Vector3(helpVec.X, helpVec.Y, helpVec.Z) ;
                // entity.ApplyLinearImpulse(ref applyVec);
                // entity.linearVelocity = BEPUutilities.Vector3.Zero;
                // entity.ApplyLinearImpulse(ref applyVec1);
                //entity.ApplyImpulse(entity.position, helpVec);
                // entity.LinearVelocity = (helpVec) * 10f;
                sphereChar.HorizontalMotionConstraint.MovementDirection = new BEPUutilities.Vector2(helpVec.X, helpVec.Z);
                space.Update(Time.fixedDeltaTime);

                using (DarkRiftWriter writer = DarkRiftWriter.Create())
                {
                    writer.Write(moveDirection.x);
                    writer.Write(moveDirection.z);
                    //na razie brak rotacji
                    writer.Write(0f);
                    writer.Write(0f);
                    //writer.Write((float)(Mathf.Deg2Rad*toSet.z));
                    // Debug.Log("Wysylam  " + (float)(Mathf.Deg2Rad * toSet.z));
                    //na razie brak skoku
                    writer.Write(false);
                    writer.Write(shoot);
                    writer.Write(transform.position.x);
                    writer.Write(transform.position.y);
                    writer.Write(transform.position.z);
                    writer.Write(Time.time);
                    writer.Write(timeDelay);
                    writer.Write(thisClentstep);

                    BitStream bs = new BitStream();
                    bs.Write(moveDirection.x);
                    bs.Write(moveDirection.y);


                    networkTag.Call(Move, target, bs);

                    //using (Message message = Message.Create(DimTag.MovePlayerTag, writer))
                    //{
                    //    uC.SendMessage(message, SendMode.Unreliable);
                    //    // Debug.Log("Wysłałem wiadomosc");
                    //}
                    //dupa
                    //toSend.move.X = moveDirection.x;
                    //toSend.move.Z = moveDirection.z;
                    //toSend.pos = sphereChar.Body.Position;
                    //toSend.time = Time.time;
                    //toSend.step = thisClentstep;
                    //setPosFromServer.SendedData(toSend);

                    thisClentstep++;
                }

                moveDirection.x = sphereChar.Body.Position.X;
                moveDirection.y = sphereChar.Body.Position.Y;
                moveDirection.z = sphereChar.Body.Position.Z;

                transform.position = moveDirection;
                //transform.eulerAngles = toSet;
            }
        }
    }
}
