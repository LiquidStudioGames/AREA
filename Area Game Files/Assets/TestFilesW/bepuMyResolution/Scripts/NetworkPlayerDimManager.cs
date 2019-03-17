using UnityEngine;
using System.Collections;
using DarkRift.Client.Unity;
using System.Collections.Generic;
using DarkRift.Client;
using System;
using DarkRift;
//using BulletSharp;
//using BulletSharp.Math;
//using VelcroPhysics.Dynamics;
//using VelcroPhysics.Factories;
using System.IO;

using Newtonsoft.Json;


namespace Dim
{
    [Serializable]
    public class ColliderItem
    {
        public float posX, posY, posZ;

        public ColliderItem(float x, float y, float z)
        {
            posX = x;
            posY = y;
            posZ = z;
        }
    }

    [Serializable]
    public class SphereColliderItem : ColliderItem
    {
        public float radius;

        public SphereColliderItem(float x, float y, float z, float r) : base(x, y,z)
        {
            radius = r;
        }
    }

    [Serializable]
    public class CubeColliterItem : ColliderItem
    {
        public float width, height, longg;

        public CubeColliterItem(float x, float y, float z, float w, float h, float t) : base(x, y,z)
        {
            width = w;
            height = h;
            longg = t;
        }
    }

    [Serializable]
    public class SaveJSONColliders
    {
        // public CircleColliderItem[] circleCollider;
        //  public BoxColliderItem[] boxCollider;

        public List<SphereColliderItem> circleColliderList = new List<SphereColliderItem>();
        public List<CubeColliterItem> boxColliderList = new List<CubeColliterItem>();


        public SaveJSONColliders()
        {
        }

        public SaveJSONColliders(SphereCollider[] circle, BoxCollider[] box)
        {
            // circleCollider = new CircleColliderItem[circle.Length];
            // boxCollider = new BoxColliderItem[box.Length];

            for (int i = 0; i < circle.Length; i++)
            {
                circleColliderList.Add(new SphereColliderItem(circle[i].transform.position.x, circle[i].transform.position.y, circle[i].transform.position.z, circle[i].radius * circle[i].transform.localScale.x));
            }

            for (int k = 0; k < box.Length; k++)
            {
                boxColliderList.Add(new CubeColliterItem(box[k].transform.position.x, box[k].transform.position.y, box[k].transform.position.z, box[k].size.x * box[k].transform.localScale.x, box[k].size.y * box[k].transform.localScale.y, box[k].size.z * box[k].transform.localScale.z));
            }

            // Debug.Log(circleCollider.Length);
            //   Debug.Log(boxCollider.Length);
        }

    }

    public static class ServerNetworkStats
    {
        public static float timeStep;
        public static float interpolationFromServer;
        public static string collisionLevelName = "mapCol.txt";
        public static float delay;
    }


    public class NetworkPlayerDimManager : MonoBehaviour
    {
        BEPUphysics.Space space;

        //public DynamicsWorld World { get; protected set; }

        //public CollisionConfiguration CollisionConf;
        //public CollisionDispatcher Dispatcher;
        //public BroadphaseInterface Broadphase;
        //public ConstraintSolver Solver;
        // [SerializeField]
        [Tooltip("The DarkRift client to communicate on.")]
        public UnityClient client;

        ushort ownerId;
       // BulletSharp.DiscreteDynamicsWorld world;

        Dictionary<ushort, SetPositionFromServer> networkPlayers = new Dictionary<ushort, SetPositionFromServer>();

        public int GetPlayersCount()
        {
            return networkPlayers.Count;
        }

        public ref BEPUphysics.Space GetWorld
        {
            get
            {
                return ref space;
            }
        }

        public void ClearAllPlayerDic()
        {
            networkPlayers.Clear();

            foreach (BEPUphysics.Entities.Entity b in space.Entities)
            {
                if (b.mass > 0)
                    space.Remove(b);

            }
        }

        public void Awake()
        {

            SaveJSONColliders obj;
            using (StreamReader sr = new StreamReader(ServerNetworkStats.collisionLevelName))
            {
                string str = sr.ReadToEnd();

                // Console.WriteLine(str);

                obj = JsonConvert.DeserializeObject<SaveJSONColliders>(str);


                // Console.WriteLine(obj.boxColliderList.Count + "   " + obj.circleColliderList.Count);

                //  Jsonu


            }
            //CollisionConf = new DefaultCollisionConfiguration();
            //Dispatcher = new CollisionDispatcher(CollisionConf);

            //Broadphase = new DbvtBroadphase();
            //Solver = new SequentialImpulseConstraintSolver();

            //World = new DiscreteDynamicsWorld(Dispatcher, Broadphase, Solver, CollisionConf);
            //World.DispatchInfo.AllowedCcdPenetration = 0.0001f;
            //World.Gravity = new BulletSharp.Math.Vector3(0, -10, 0);
            // world = new Bunew VelcroPhysics.Dynamics.World(Microsoft.Xna.Framework.Vector2.Zero);

            space = new BEPUphysics.Space();
            space.ForceUpdater.Gravity = new BEPUutilities.Vector3(0, -10f, 0);
            AddCollidersToWorld(obj);
            client.MessageReceived += CleneMessageReveiveMessageReceived;
        }


        BEPUphysics.Entities.Entity AddObiect(float mass, BEPUutilities.Vector3 place, BEPUphysics.CollisionShapes.EntityShape shape)
        {
            BEPUphysics.Entities.Entity ent = new BEPUphysics.Entities.Entity(shape, mass);
            ent.Position = place;
            space.Add(ent);
            return ent;
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

        //    World.AddRigidBody(body);

        //    return body;
        //}

         Transform tr;

        private void Start()
        {
            tr = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        }

            void AddCollidersToWorld(SaveJSONColliders sjc)
        {
            foreach (CubeColliterItem bI in sjc.boxColliderList)
            {
                AddObiect(0, new BEPUutilities.Vector3(bI.posX, bI.posY, bI.posZ), new BEPUphysics.CollisionShapes.ConvexShapes.BoxShape(bI.width,bI.height, bI.longg));
            }

            foreach (SphereColliderItem cI in sjc.circleColliderList)
            {
                AddObiect(0, new BEPUutilities.Vector3(cI.posX, cI.posY, cI.posZ), new BEPUphysics.CollisionShapes.ConvexShapes.SphereShape(cI.radius));
            }
        }

        public void SetOwnerId(ushort id)
        {
            ownerId = id;
        }

        void CleneMessageReveiveMessageReceived(object sender, MessageReceivedEventArgs e)
        {

            //using (Message message = e.GetMessage() as Message)
            //{
            //    //Check the message has a zero tag
            //    if (message.Tag == MOVE_TAG)
            //    {
            //        //Get the reader from the message so we can read the data
            //        using (DarkRiftReader reader = message.GetReader())
            //        {
            //            //If it's for us...
            //            if (reader.ReadByte() == dragID)
            //            {
            //                //... then update our position!
            //                targetPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), 0);
            //            }
            //        }
            //    }
            //}
            using (Message message = e.GetMessage() as Message)
            {

                //Debug.Log("message " + message.Tag);
                if(networkPlayers.Count > 0)
                if (message.Tag == DimTag.MovePlayerTag)
                {
                    using (DarkRiftReader reader = message.GetReader())
                    {
                        ushort id = reader.ReadUInt16();
                        {
                                if (id == ownerId)
                                {
                                    float calcPositionX = reader.ReadSingle();
                                    float calcPositionY = reader.ReadSingle();
                                    float calcPositionZ = reader.ReadSingle();
                                    ushort lastCalculatedStep = reader.ReadUInt16();
                                    float time = reader.ReadSingle();
                                    tr.position = new Vector3(calcPositionX, calcPositionY, calcPositionZ);
                                    if (networkPlayers.ContainsKey(id))
                                        networkPlayers[id].ReciveOwnerMessage(lastCalculatedStep, calcPositionX, calcPositionY, calcPositionZ, time);
                                }
                                else
                                {
                                    float calcPositionX = reader.ReadSingle();
                                    float calcPositionY = reader.ReadSingle();
                                    float calcPositionZ = reader.ReadSingle();
                                    ushort lastCalculatedStep = reader.ReadUInt16();
                                    float time = reader.ReadSingle();
                                    float moveX = reader.ReadSingle();
                                    float moveY = reader.ReadSingle();
                                    float rotYaw = reader.ReadSingle();
                                    float rotPitch = reader.ReadSingle();
                                    bool jump = reader.ReadBoolean();
                                    bool shoot = reader.ReadBoolean();
                                    ushort lastShootStep = reader.ReadUInt16();
                                    ushort step = reader.ReadUInt16();

                                    if (networkPlayers.ContainsKey(id))
                                        networkPlayers[id].ReciveOtherPlayerMessage(step, calcPositionX, calcPositionY, calcPositionZ, moveX, moveY, rotYaw, rotPitch, lastShootStep, step, shoot, jump, time);
                                    //networkPlayers[id].SetDelayTimeForOther(reader.ReadSingle(), reader.ReadUInt16());
                                }
                        }
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

        //update world time step need be the same on client and server
        private void FixedUpdate()
        {
            if (networkPlayers.ContainsKey(ownerId))
            {
               // world.Step(Time.fixedDeltaTime);

            }
        }

        public void Add(ushort id, SetPositionFromServer player)
        {
            if(!networkPlayers.ContainsKey(id))
            networkPlayers.Add(id, player);
        }

        public void DestroyPlayer(ushort id)
        {
            if (networkPlayers.ContainsKey(id))
            {
                GameObject gob =  networkPlayers[id].gameObject;
                // o.DestroyBody();


                Debug.Log("Usuwam gracza " + id);
                networkPlayers.Remove(id);
                Destroy(gob);
            }
        }
    }
}
