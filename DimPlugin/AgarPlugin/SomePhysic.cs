using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using VelcroPhysics.Dynamics;
//using VelcroPhysics.Factories;
using DarkRift;
using DarkRift.Server;
//using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Timers;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using Dim.GameTypes;
using BEPUutilities;
using BEPUphysics.Entities;
using BEPUphysics;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.Character;

namespace Dim
{
    public static class WeaponBreak
    {
        public static float pistolBreak = 0.2f;
        public static ushort pistolBreakStep;
    }

    public class SomePhysic : Plugin
    {
        public override bool ThreadSafe => false;
        public override Version Version => new Version(1, 0, 0);

        public float interpolationTime = .3f;
        public float worldStep = 0.02f;
        public long worldIntStep;
        public const int MiilsecondsDrop = 5;
        public int helpDebuStep = 0;

        Dictionary<IClient, PlayerInputs> players = new Dictionary<IClient, PlayerInputs>();
        Space space;
        //public CollisionConfiguration CollisionConf;
        //public CollisionDispatcher Dispatcher;
        //public BroadphaseInterface Broadphase;
        //public ConstraintSolver Solver;
        Thread th;
        float lastStepTime;
        Stopwatch stopWatch;
        DateTime dt;
        float lastNotRequestedTime;
        float Timerek = 0;
        float tt;
        long tim;
        PlayerInputs helpPlayerInput;
        string collisionLevelName = "mapCol.txt";
        DamageDeliveryGameMachine ddgm;


        public SomePhysic(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            SaveJSONColliders obj;
            using (StreamReader sr = new StreamReader(collisionLevelName))
            {
                string str = sr.ReadToEnd();

               // Console.WriteLine(str);

                 obj = JsonConvert.DeserializeObject<SaveJSONColliders>(str);


               // Console.WriteLine(obj.boxColliderList.Count + "   " + obj.circleColliderList.Count);

                //  Jsonu


            }
            WeaponBreak.pistolBreakStep = (ushort)(WeaponBreak.pistolBreak / worldStep);

            worldIntStep = (long)(1000 * worldStep);
            stopWatch = Stopwatch.StartNew();

            CreatePhysicsWorld();

            AddCollidersToWorld(obj);
            th = new Thread(ExecuteInForeground2);
            th.Start();
            ddgm = new DamageDeliveryGameMachine(players);

            ClientManager.ClientConnected += ClientConnected;
            ClientManager.ClientDisconnected += ClientDisconnected;
        }

        void CreatePhysicsWorld()
        {
            space = new Space();
            space.ForceUpdater.Gravity = new Vector3(0, -10f, 0);
        }



        //public virtual RigidBody LocalCreateRigidBody(float mass, Matrix startTransform)
        //{
        //    CollisionShape sh = new SphereShape(0.5f);
        //    //rigidbody is dynamic if and only if mass is non zero, otherwise static
        //    bool isDynamic = (mass != 0.0f);

        //    BulletSharp.Math.Vector3 localInertia = BulletSharp.Math.Vector3.Zero;
        //    if (isDynamic)
        //        sh.CalculateLocalInertia(mass, out localInertia);

        //    //using motionstate is recommended, it provides interpolation capabilities, and only synchronizes 'active' objects
        //    DefaultMotionState myMotionState = new DefaultMotionState(startTransform);

        //    RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, sh, localInertia);
        //    RigidBody body = new RigidBody(rbInfo);
        //    rbInfo.Dispose();

        //    world.AddRigidBody(body);

        //    return body;
        //}

        Entity AddPlayer(float mass, EntityShape shape)
        {
            Entity ent = new Entity(shape, mass);
            ent.Position = Vector3.Zero;
            space.Add(ent);
            return ent;
        }

        void AddObiect(Vector3 place, EntityShape shape)
        {
            Entity ent = new Entity(shape, 0);
            ent.Position = place;
            space.Add(ent);
        }

        //public virtual RigidBody LocalCreateRigidBody(float mass, Matrix startTransform, CollisionShape shape)
        //{
        //    //rigidbody is dynamic if and only if mass is non zero, otherwise static
        //    bool isDynamic = (mass != 0.0f);

        //    BulletSharp.Math.Vector3 localInertia = BulletSharp.Math.Vector3.Zero;
        //    if (isDynamic)
        //        shape.CalculateLocalInertia(mass, out localInertia);

        //    //using motionstate is recommended, it provides interpolation capabilities, and only synchronizes 'active' objects
        //    DefaultMotionState myMotionState = new DefaultMotionState(startTransform);

        //    RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, shape, localInertia);
        //    RigidBody body = new RigidBody(rbInfo);
        //    rbInfo.Dispose();

        //    world.AddRigidBody(body);

        //    return body;
        //}

        void AddCollidersToWorld(SaveJSONColliders sjc)
        {
            foreach (CubeColliterItem bI in sjc.boxColliderList)
            {
                AddObiect(new Vector3(bI.posX, bI.posY, bI.posZ), new BoxShape(bI.width, bI.height, bI.longg));
            }

            foreach (SphereColliderItem cI in sjc.circleColliderList)
            {
                AddObiect(new Vector3(cI.posX, cI.posY, cI.posZ), new SphereShape(cI.radius));
            }
        }

        //void AddCollidersToWorld(SaveJSONColliders sjc)
        //{
        //    foreach (BoxColliderItem bI in  sjc.boxColliderList)
        //    {
        //        BodyFactory.CreateRectangle(world, bI.width, bI.height, 1, new Vector2(bI.posX, bI.posY));
        //    }

        //    foreach (CircleColliderItem cI in sjc.circleColliderList)
        //    {
        //        BodyFactory.CreateCircle(world,cI.radius,1, new Vector2(cI.posX, cI.posY));
        //    }
        //}

        private void ExecuteInForeground2()
        {
            while(true)
            {
                if (players.Count > 0)
                {
                    while(stopWatch.ElapsedMilliseconds - tim >= worldIntStep)
                    {
                        tim += worldIntStep;

                            tt = (stopWatch.ElapsedMilliseconds - (stopWatch.ElapsedMilliseconds - tim)) / 1000f;

                            foreach (PlayerInputs pi in players.Values)
                            {
                                pi.BodySetMove(tt);
                            }
                            space.Update(worldStep);
                       // Console.WriteLine(worldIntStep);
                           // world.StepSimulation(worldStep);

                            foreach (PlayerInputs pi in players.Values)
                            {
                                pi.SetCalculatedPosition();
                            }

                        ddgm.Update();
                    }
                }
              Thread.Sleep(MiilsecondsDrop);
            }
        }


        SphereCharacterController AddPlayer(bool stat = true)
        {
            SphereCharacterController ent = new SphereCharacterController();
            if (stat)
                ent.Body.BecomeKinematic();
            // ent.Body.mass = 0;
            ent.Body.Radius = 1f;
            ent.Body.Position = Vector3.Zero;
            space.Add(ent);
            return ent;
        }


        void ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            PlayerInputs pI = new PlayerInputs(
                this,
                e.Client.ID,
                (float)0,
                (float)0,
                (bool)false,
                (bool)false,
                (float)0,
                (float)0,
                 (float)0,
                (float)0,
                //LocalCreateRigidBody(1f, Matrix.Identity),
                AddPlayer(false),
                space,
                interpolationTime,
                WeaponBreak.pistolBreakStep
                );

            using (DarkRiftWriter newPlayerWriter = DarkRiftWriter.Create())
            {
                newPlayerWriter.Write(pI.ID);
                //newPlayerWriter.Write(pI.moveHorizontal);
                //newPlayerWriter.Write(pI.moveVertical);
                //newPlayerWriter.Write(pI.shoot);
                //newPlayerWriter.Write(pI.waitForDim);
                newPlayerWriter.Write(pI.posX);
                newPlayerWriter.Write(pI.posY);
                newPlayerWriter.Write(pI.posZ);

                //newPlayerWriter.Write(pI.rot);

                // players.Add(e.Client, pI);
                using (Message newPlayerMessage = Message.Create(DimTag.SpawnPlayerTag, newPlayerWriter))
                {
                    foreach (IClient client in ClientManager.GetAllClients().Where(x => x != e.Client))
                        client.SendMessage(newPlayerMessage, SendMode.Reliable);
                }
            }

            players.Add(e.Client, pI);

            using (DarkRiftWriter playerWriter = DarkRiftWriter.Create())
            {
                foreach (PlayerInputs playerInput in players.Values)
                {
                    playerWriter.Write(playerInput.ID);
                    // playerWriter.Write(pI.moveHorizontal);
                    // playerWriter.Write(pI.moveVertical);
                    // playerWriter.Write(pI.shoot);
                    //playerWriter.Write(pI.waitForDim);
                    playerWriter.Write(playerInput.posX);
                    playerWriter.Write(playerInput.posY);
                    playerWriter.Write(playerInput.posZ);
                    // playerWriter.Write(pI.rot);
                }
                using (Message playerMessage = Message.Create(DimTag.SpawnPlayerTag, playerWriter))
                    e.Client.SendMessage(playerMessage, SendMode.Reliable);
            }

            e.Client.MessageReceived += MovementMessageReceived;
        }


        void MovementMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            using (Message message = e.GetMessage() as Message)
            {
                if (message.Tag == DimTag.MovePlayerTag)
                {
                    using (DarkRiftReader reader = message.GetReader())
                    {
                        float moveHor = reader.ReadSingle();
                        float moveVer = reader.ReadSingle();

                        float rotYaw = reader.ReadSingle();
                        float rotPitch = reader.ReadSingle();

                        bool jump = reader.ReadBoolean();
                        //Console.WriteLine("Odbieram " + rot);
                        bool shoot = reader.ReadBoolean();

                        float newX = reader.ReadSingle();
                        float newY = reader.ReadSingle();
                        float newZ = reader.ReadSingle();
                        float clientSendTime = reader.ReadSingle();
                        float time = reader.ReadSingle();
                        ushort st = reader.ReadUInt16();
                        
                        float timer = (stopWatch.ElapsedMilliseconds / 1000f);
                    


                    if (helpDebuStep > 0)
                    {
                            Console.WriteLine(clientSendTime+ "    " + st);
                            // Console.WriteLine(Timerek + "  " + clientSendTime + "    " + stopWatch.ElapsedMilliseconds + "    " + st);

                            helpDebuStep--;
                    }

                    helpPlayerInput = players[e.Client];

                        //sprawdzenie czy nie ma już takiego kroku w inputach - może duplikat
                        if (helpPlayerInput.CheckIfStepNotExist(st)) 
                        {
                            helpPlayerInput.RegisterInput(moveHor, moveVer, newX, newY, newZ, timer - time,clientSendTime,jump, shoot, rotYaw, rotPitch, st);

                            
                            using (DarkRiftWriter writer = DarkRiftWriter.Create())
                            {
                                writer.Write(helpPlayerInput.ID);
                                //writer.Write(moveHor);
                                //writer.Write(moveVer);
                                //writer.Write(rot);
                                writer.Write(helpPlayerInput.posX);
                                writer.Write(helpPlayerInput.posY);
                                writer.Write(helpPlayerInput.posZ);
                                //writer.Write(clientSendTime);
                                //writer.Write(st);

                                writer.Write(helpPlayerInput.GetLastCalculatedStep());
                                writer.Write(helpPlayerInput.GetLastInputStepTime());
                                //writer.Write(clientSendTime);
                                message.Serialize(writer);
                                //  message.Tag = DimTag.MovePlayerTag;
                            }

                            e.Client.SendMessage(message, SendMode.Unreliable);
                            //dupa nie wiem czy tak można
                            message.Dispose();



                            using (DarkRiftWriter writer = DarkRiftWriter.Create())
                            {
                                writer.Write(helpPlayerInput.ID);
                                writer.Write(helpPlayerInput.posX);
                                writer.Write(helpPlayerInput.posY);
                                writer.Write(helpPlayerInput.posZ);
                                writer.Write(helpPlayerInput.GetLastCalculatedStep());
                                writer.Write(moveHor);
                                writer.Write(moveVer);
                                writer.Write(rotYaw);
                                writer.Write(rotPitch);
                                writer.Write(jump);
                                writer.Write(shoot);

                                writer.Write(helpPlayerInput.GetLastShootStep());

                                writer.Write(time);
                                writer.Write(st);
                               
                                message.Serialize(writer);
                                //  message.Tag = DimTag.MovePlayerTag;
                            }

                           // e.Client.SendMessage(message, SendMode.Unreliable);

                            //using (DarkRiftWriter writerForEveryone = DarkRiftWriter.Create())
                            //{
                            //    writerForEveryone.Write(helpPlayerInput.ID);
                            //    writerForEveryone.Write(helpPlayerInput.posX);
                            //    writerForEveryone.Write(helpPlayerInput.posY);
                            //    writerForEveryone.Write(clientSendTime);
                            //    writerForEveryone.Write(st);
                            //    message.Serialize(writerForEveryone);

                            //}

                            foreach (IClient c in ClientManager.GetAllClients().Where(x => x != e.Client))
                                {
                                    c.SendMessage(message, SendMode.Unreliable);
                                }
                        }
                }
                    return;
                }

                if (message.Tag == DimTag.TimeTag)
                {
                    using (DarkRiftReader reader = message.GetReader())
                    {
                        ushort index = reader.ReadUInt16();
                        float time = reader.ReadSingle();

                        using (DarkRiftWriter writer = DarkRiftWriter.Create())
                        {
                            writer.Write(index);
                            writer.Write(time);
                            writer.Write(worldStep);
                            writer.Write(interpolationTime);
                            message.Serialize(writer);
                            //  message.Tag = DimTag.MovePlayerTag;
                        }

                        e.Client.SendMessage(message, SendMode.Reliable);
                    }
                        return;
                }
            }
        }


        void ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
           //Console.WriteLine("Usuwam");
            players[e.Client].RemoveBody();
            players.Remove(e.Client);
            Console.WriteLine("Klient usuniety");

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(e.Client.ID);

                using (Message message = Message.Create(DimTag.DespawnPlayerTag, writer))
                {
                    foreach (IClient client in ClientManager.GetAllClients())
                        client.SendMessage(message, SendMode.Reliable);
                }
            }
        }


        //public bool TryHitPlayer(Body b)
        //{
        //    foreach (PlayerInputs pi in players.Values)
        //    {
        //        if (pi.HitSingle(b))
        //        {
        //            // Console.WriteLine("Ktos dostal");
        //            return true;

        //        }
        //    }
        //    return false;
        //}
    }
}
