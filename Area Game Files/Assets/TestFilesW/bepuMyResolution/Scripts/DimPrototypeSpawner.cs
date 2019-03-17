using UnityEngine;
using DarkRift.Client.Unity;
using DarkRift.Client;
using DarkRift;
using BEPUutilities;
using BEPUphysics;
using Vector3 = BEPUutilities.Vector3;
using BEPUphysics.Character;
//using BulletSharp;
//using BulletSharp.Math;
//using VelcroPhysics.Dynamics;
//using VelcroPhysics.Factories;

namespace Dim
{
    public class DimPrototypeSpawner : MonoBehaviour
    {
        [Tooltip("The DarkRift client to communicate on.")]
        public UnityClient client;


        [Tooltip("The controllable player prefab.")]
        public GameObject controllablePrefab;

        [Tooltip("The network controllable player prefab.")]
        public GameObject networkPrefab;

        [Tooltip("The network player manager.")]
        public NetworkPlayerDimManager networkPlayeDimrManager;

        public int startPacketLength = 14;


        void Awake()
        {
            if (client == null)
            {
                Debug.LogError("Client unassigned in PlayerSpawner.");
                Application.Quit();
            }

            if (controllablePrefab == null)
            {
                Debug.LogError("Controllable Prefab unassigned in PlayerSpawner.");
                Application.Quit();
            }

            if (networkPrefab == null)
            {
                Debug.LogError("Network Prefab unassigned in PlayerSpawner.");
                Application.Quit();
            }

            //automatic connection to server
           // client.Connect(client.Address, client.Port, client.IPVersion);
            client.MessageReceived += MessageReceived;
        }



        void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            using (Message message = e.GetMessage() as Message)
            {
                if (message.Tag == DimTag.SpawnPlayerTag)
                    SpawnPlayer(sender, e);
                else if (message.Tag == DimTag.DespawnPlayerTag)
                    DespawnPlayer(sender, e);
            }
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

        //    networkPlayeDimrManager.GetWorld.AddRigidBody(body);

        //    return body;
        //}

        BEPUphysics.Entities.Entity AddPlayer(float mass, BEPUphysics.CollisionShapes.EntityShape shape)
        {
            BEPUphysics.Entities.Entity ent = new BEPUphysics.Entities.Entity(shape, mass);
            ent.Position = Vector3.Zero;
            networkPlayeDimrManager.GetWorld.Add(ent);
            return ent;
        }

        public float playerRadius = 1f;

        SphereCharacterController AddPlayer(bool stat = true)
        {
            SphereCharacterController ent = new SphereCharacterController();
            if (stat)
                ent.Body.BecomeKinematic();
           // ent.Body.mass = 0;
            ent.Body.Radius = playerRadius;
            ent.Body.Position = Vector3.Zero;
            networkPlayeDimrManager.GetWorld.Add(ent);
            return ent;
        }

        BEPUphysics.Entities.Prefabs.Sphere AddNotLocalPlayer()
        {
            BEPUphysics.Entities.Prefabs.Sphere sphShape = new BEPUphysics.Entities.Prefabs.Sphere(BEPUutilities.Vector3.Zero, playerRadius);// new BEPUphysics.CollisionShapes.ConvexShapes.SphereShape(playerRadius);
            networkPlayeDimrManager.GetWorld.Add(sphShape);          
            return sphShape;
        }
        

        void SpawnPlayer(object sender, MessageReceivedEventArgs e)
        {
            //BEPUphysics.Entities.Entity ent = new BEPUphysics.Entities.Entity(

            using (Message message = e.GetMessage())
                if (message.Tag == DimTag.SpawnPlayerTag)
                {
                    using (DarkRiftReader reader = message.GetReader())
                    {

                        if (reader.Length % startPacketLength != 0)
                        {
                            Debug.LogWarning("Received malformed spawn packet.  " + reader.Length);
                            return;
                        }

                        //Debug.Log(reader.Length);

                        while (reader.Position < reader.Length)
                        {
                            ushort id = reader.ReadUInt16();
                            float x = reader.ReadSingle();
                            float y = reader.ReadSingle();
                            float z = reader.ReadSingle();
                            UnityEngine.Vector3 position = new UnityEngine.Vector3(x, y, z);
                            // Debug.Log(reader.Position);
                            //float radius = reader.ReadSingle();
                            // Color32 color = new Color32(
                            // reader.ReadByte(),
                            // reader.ReadByte(),
                            // reader.ReadByte(),
                            // 255
                            // );

                            Debug.Log("Spawning client for ID = " + id + ".");
                            SetPositionFromServer ob;
                            GameObject obj;
                            if (id == client.ID)
                            {
                                obj = Instantiate(controllablePrefab, position, UnityEngine.Quaternion.identity) as GameObject;



                                //Camera.main.GetComponent<CamFollow>().Target = obj.transform;
                              //  Camera.main.transform.SetParent(obj.transform);
                                networkPlayeDimrManager.SetOwnerId(id);
                                ob = obj.GetComponent<SetPositionFromServer>();
                                // ob.InitBody(LocalCreateRigidBody(1f, Matrix.Identity), networkPlayeDimrManager.GetWorld);
                               // BEPUphysics.Entities.Entity ent = AddPlayer(1, new BEPUphysics.CollisionShapes.ConvexShapes.SphereShape(1f));
                                ob.InitBody(AddPlayer(false),
                                    networkPlayeDimrManager.GetWorld); 
                                MoveControl player = obj.GetComponent<MoveControl>();
                                player.Clientt = client;
                                DeliveryDamageGameType typeGame = obj.GetComponent<DeliveryDamageGameType>();
                                typeGame.Clientt = client;


                            }
                            else
                            {
                                obj = Instantiate(networkPrefab, position, UnityEngine.Quaternion.identity) as GameObject;
                                ob = obj.GetComponent<SetPositionFromServer>();
                               // BEPUphysics.Entities.Entity ent = AddPlayer(0, new BEPUphysics.CollisionShapes.ConvexShapes.SphereShape(1f));
                                ob.InitBody(AddPlayer(false),
                                   networkPlayeDimrManager.GetWorld);
                            }

                            
                           



                            //agarObj.SetRadius(radius);
                            //agarObj.SetColor(color);

                            networkPlayeDimrManager.Add(id, ob);
                        }
                    }
                }
        }

        void DespawnPlayer(object sender, MessageReceivedEventArgs e)
        {
            using (Message message = e.GetMessage())
            using (DarkRiftReader reader = message.GetReader())
                networkPlayeDimrManager.DestroyPlayer(reader.ReadUInt16());
        }

    }
}
