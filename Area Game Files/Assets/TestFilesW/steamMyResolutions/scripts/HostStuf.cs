using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using BEPUutilities;
using BEPUphysics.Entities;
using BEPUphysics;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.Character;
using Vector3 = BEPUutilities.Vector3;
using Vector2 = BEPUutilities.Vector2;
using Quaternion = BEPUutilities.Quaternion;
using Space = BEPUphysics.Space;

namespace wolfingame.steamTest
{
    public enum MOVE_DIRECTION : byte
    {
        NONE = 0,
        UP_LEFT = 1,
        UP = 2,
        UP_RIGHT = 3,
        RIGHT = 4,
        DOWN_RIGHT = 5,
        DOWN = 6,
        DOWN_LEFT = 7,
        LEFT = 8
    }

    public enum BOOL_STUF : byte
    {
        NONE = 0,
        JUMP = 1,
        SHOOT = 2,
        ACTION = 4,
        JUMP_SHOOT_ACTION = 7,
        JUMP_SHOOT = 3,
        JUMP_ACTION = 5,
        SHOOT_ACTION = 6,
    }

    public enum BOOL_STUF_SHORT : byte
    {
        NONE = 0,
        JUMP = 1,
        SHOOT = 2,
        JUMP_SHOOT = 3
    }

    public class ClientPlayer
    {
        public Vector3 position;
        public Transform obTransform;

    }

    public class HostedPlayer
    {
        public ulong id;
        public float moveX, moveY;
        public bool jump;
        public bool shoot;
        public float rotPitch, rotYaw;
        public Vector3 position;
        public float delayTime;
        public float clientTime;
        public SphereCharacterController sphereChar;
        public Space space;


        ushort damageTaken, damageDelivered;
        bool change = false;
        ushort pistoStepBreak;
        ushort stepBreak;
        public int playerHealth;
        ushort lastShootStep;
        float interpolation;


        Inputs[] inputs;
        Inputs zeroInput;
        Inputs lastInput, beforeLastInput;
        int lastInputIndex;
        int endIndex;
        bool firstTime;

        // SphereCharacterController spherChar;
        // Space space;

        Inputs helpInput;
        Vector3 helpVec;
        Vector3 applyVec;

        // ushort stepToCalculation = 1;
        ushort lastReceiveStep = 1;
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

        public struct PlayerMotionValues
        {
            public float moveForce;
            public float jumpForce;
        }

        public struct Inputs2
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

        public struct Inputs
        {
            public Vector2 moveDir;
            public Vector3 position;
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

        public struct InputsENUM_SHOORT
        {
            public MOVE_DIRECTION moveDir;
            public Vector3 position;
            public BOOL_STUF_SHORT boolStuf;
            public bool jump, shoot, waitForDim;
            public float rotYaw, rotPitch;
            public float inputTime;
            public float clientTime;
            public ushort clientStep;
            public float releaseTime;
            public bool used;
            public bool shootInThisStep;

            public void FillInput(ref Inputs input)
            {
                DirectionSet(ref input.moveDir);
            }

            void DirectionSet(ref Vector2 vec)
            {
                switch (moveDir)
                {
                    case MOVE_DIRECTION.DOWN:
                        vec.X = 0;
                        vec.Y = -1;
                        break;
                    case MOVE_DIRECTION.DOWN_LEFT:
                        vec.X = -1;
                        vec.Y = -1;
                        vec.Normalize();
                        break;
                    case MOVE_DIRECTION.DOWN_RIGHT:
                        vec.X = 1;
                        vec.Y = -1;
                        vec.Normalize();
                        break;
                    case MOVE_DIRECTION.LEFT:
                        vec.X = -1;
                        vec.Y = 0;
                        break;
                    case MOVE_DIRECTION.NONE:
                        vec.X = 0;
                        vec.Y = 0;
                        break;
                    case MOVE_DIRECTION.RIGHT:
                        vec.X = 1;
                        vec.Y = 0;
                        break;
                    case MOVE_DIRECTION.UP:
                        vec.X = 0;
                        vec.Y = 1;
                        break;
                    case MOVE_DIRECTION.UP_LEFT:
                        vec.X = -1;
                        vec.Y = 1;
                        vec.Normalize();
                        break;
                    case MOVE_DIRECTION.UP_RIGHT:
                        vec.X = 1;
                        vec.Y = 1;
                        vec.Normalize();
                        break;
                    default:
                        Debug.LogError("This should not happen");
                        break;

                }
            }
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
            position = sphereChar.Body.Position;
            //position.X = sphereChar.Body.Position.X;
            //position. = spherChar.Body.Position.Y;
            //posZ = spherChar.Body.Position.Z;
            //lastCalculatedStep = tempCalc;
        }


        public void BodySetMove(float fromTime)
        {
            //GetLastInputStep(fromTime, ref helpInput);
            //helpVec.X = helpInput.moveX;
            //helpVec.Y = helpInput.moveY;
            GetLastInputStep(fromTime);
            helpVec *= 1f;
            sphereChar.HorizontalMotionConstraint.MovementDirection = new Vector2(helpVec.X, helpVec.Z);
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
                if (inputs[i].clientStep == step)
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
                    // helpVec = inputs[i].moveDir
                    helpVec.X += inputs[i].moveDir.X;
                    helpVec.Z += inputs[i].moveDir.Y;
                    tempCalc = inputs[i].clientStep;



                    //return;
                }
            // return helpVec;

            //inp = zeroInput;
            //lastCalculatedStep = 0;
        }

        void GetLastInputStep(float fromTime, ref Inputs inp)
        {
            helpVec = Vector3.Zero;
            int steps = 0;

            for (int i = inputs.Length - 1; i >= 0; i--)
                if (inputs[i].releaseTime <= fromTime && !inputs[i].used)
                {
                    steps++;
                    if (steps >= 2)
                        // Console.WriteLine(steps);
                        Debug.Log("I calculate " + steps + ". Calculation is bad in this place if this happen frequent - change this code");
                    inputs[i].used = true;
                    inp = inputs[i];
                    helpVec.X += inputs[i].moveDir.X;
                    helpVec.Z += inputs[i].moveDir.Y;
                    tempCalc = inputs[i].clientStep;
                    //return;
                }
            inp = zeroInput;
            //lastCalculatedStep = 0;
        }


        public HostedPlayer(Space sp, SphereCharacterController sc, int inputsSize = 30)
        {
            space = sp;
            sphereChar = sc;
            inputs = new Inputs[inputsSize];
            endIndex = inputsSize - 1;
            lastInputIndex = 0;
            firstTime = true;
        }


        public HostedPlayer(Space sp, SphereCharacterController sc, PlayerMotionValues val, int inputsSize = 30)
        {
            space = sp;
            sphereChar = sc;
            inputs = new Inputs[inputsSize];
            endIndex = inputsSize - 1;
            lastInputIndex = 0;
            firstTime = true;
        }

        //public PlayerInputs(SomePhysic someP, ushort id, float moveHor, float moveVer, bool shoo, bool waitFor, float positionX, float positionY, float positionZ, float rotation, SphereCharacterController body, Space worl, float interp, ushort pistolStepBr, int inputsSize = 30)
        //{
        //    sP = someP;
        //    ID = id;
        //    moveHorizontal = moveHor;
        //    moveVertical = moveVer;
        //    shoot = shoo;
        //    waitForDim = waitFor;
        //    posX = positionX;
        //    posY = positionY;
        //    posZ = positionZ;
        //    rot = rotation;
        //    spherChar = body;
        //    space = worl;
        //    interpolation = interp;
        //    pistoStepBreak = pistolStepBr;

        //    inputs = new Inputs[inputsSize];
        //    endIndex = inputsSize - 1;
        //    // lastInput = new Inputs();
        //    // beforeLastInput = new Inputs();
        //    lastInputIndex = 0;
        //    firstTime = true;
        //}

        public void RemoveBody()
        {
            space.Remove(sphereChar);

            //Console.WriteLine("Usuniety");
        }


        void SortInputs(int to)
        {
            for (int i = inputs.Length - 2; i >= to; i--)
            {
                inputs[i + 1] = inputs[i];
            }
        }


        public void RegisterInput(Inputs input, ushort step)
        {
            if (step < lastReceiveStep)
            {
                for (int i = 0; i < inputs.Length; i++)
                {
                    if (inputs[i].clientStep < step)
                    {
                        SortInputs(i);
                        PushInput(i, input);
                        return;
                    }
                }
            }
            else
            {
                lastReceiveStep = step;
                SortInputs(0);
                PushInput(0, input);
            }
        }


        //public void RegisterInput(float x, float y, float posX, float posY, float posZ, float time, float clientTime, bool jump, bool shoot, float rotYaw, float rotPitch, ushort step)
        //{
        //    if (step < lastReceiveStep)
        //    {
        //        for (int i = 0; i < inputs.Length; i++)
        //        {
        //            if (inputs[i].clientStep < step)
        //            {
        //                SortInputs(i);
        //                PushInput(i, x, y, posX, posY, posZ, time, clientTime, jump, shoot, rotYaw, rotPitch, step);
        //                return;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        lastReceiveStep = step;
        //        SortInputs(0);
        //        PushInput(0, x, y, posX, posY, posZ, time, clientTime, jump, shoot, rotYaw, rotPitch, step);
        //    }
        //}

        void PushInput(int index, Inputs input)
        {
            inputs[index] = input;
        }

        //void PushInput(int index, float x, float y, float posX, float posY, float posZ, float time, float clientTime, bool jump, bool shoot, float rotYaw, float rotPitch, ushort step)
        //{
        //    inputs[index].moveX = x;
        //    inputs[index].moveY = y;
        //    inputs[index].positX = posX;
        //    inputs[index].positY = posY;
        //    inputs[index].positZ = posZ;
        //    inputs[index].inputTime = time;
        //    inputs[index].clientStep = step;
        //    inputs[index].releaseTime = time + interpolation;
        //    inputs[index].clientTime = clientTime;
        //    inputs[index].used = false;
        //    inputs[index].jump = jump;
        //    inputs[index].shoot = shoot;
        //    inputs[index].rotYaw = rotYaw;
        //    inputs[index].rotPitch = rotPitch;
        //    //inputs[index].sh
        //}

        //float DegToRad(float deg)
        //{
        //    return (float)((deg / 360f) * (2 * Math.PI));
        //}

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

    public class Player
    {
        public SphereCharacterController sphereChar;
    }


    public static class BepuHelp
    {
        public static UnityEngine.Vector3 BepuToUnityVector3(Vector3 vec)
        {
            return new UnityEngine.Vector3(vec.X, vec.Y, vec.Z);
        }

        public static Vector3 UnityToBepuVector3(UnityEngine.Vector3 vec)
        {
            return new Vector3(vec.x, vec.y, vec.z);
        }

        public static UnityEngine.Quaternion BepuToUnityQuaternion(Quaternion q)
        {
            return new UnityEngine.Quaternion(q.W, q.Z, -q.Y, -q.X);
        }

        public static Quaternion UnityToBepuQuaternion(UnityEngine.Quaternion q)
        {
            return new Quaternion(q.w, q.z, -q.y, -q.x);
        }
    }


    public static class WeaponBreak
    {
        public static float pistolBreak = 0.2f;
        public static ushort pistolBreakStep;
    }


    public static class ServerNetworkStats
    {
        public static float timeStep;
        public static float interpolationFromServer;
        public static string collisionLevelName = "mapCol.txt";
        public static float delay;
    }


    public class HostStuf : MonoBehaviour
    {
        protected BEPUphysics.Space space;
        Dictionary<ulong, HostedPlayer> players = new Dictionary<ulong, HostedPlayer>();
        bool runSpace = false;

        void CreatePhysicsWorld()
        {
            space = new BEPUphysics.Space();
            space.ForceUpdater.Gravity = new BEPUutilities.Vector3(0, -10f, 0);
        }


        void CreateStaticWorld()
        {
            SphereCollider[] sphereCol = FindObjectsOfType<SphereCollider>();
            BoxCollider[] boxCol = FindObjectsOfType<BoxCollider>();

            foreach (SphereCollider sc in sphereCol)
            {
                float radius = sc.transform.localScale.x * sc.radius;
                space.Add(
                    AddObiect(BepuHelp.UnityToBepuVector3(sc.transform.position), new SphereShape(radius))
                    );
            }

            foreach (BoxCollider bc in boxCol)
            {
                UnityEngine.Vector3 scale = bc.transform.localScale;
                Quaternion rot = BepuHelp.UnityToBepuQuaternion(bc.transform.rotation);
                space.Add(
                    AddObiect(BepuHelp.UnityToBepuVector3(bc.transform.position), rot, new BoxShape(scale.x, scale.y, scale.z))
                    );
            }
        }


        Entity AddObiect(Vector3 place, EntityShape shape)
        {
            Entity ent = new Entity(shape, 0);
            ent.Position = place;
            return ent;
        }


        Entity AddObiect(Vector3 place, Quaternion rot, EntityShape shape)
        {
            Entity ent = AddObiect(place, shape);
            ent.Orientation = rot;
            return ent;
        }


        // Start is called before the first frame update
        void Start()
        {
            CreatePhysicsWorld();
            CreateStaticWorld();

            //dupa
            //ClientManager.ClientConnected += ClientConnected;
            //ClientManager.ClientDisconnected += ClientDisconnected;
            //Game.Instance.Steam.
        }



        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            SpaceUpdate();
        }

        private void SpaceUpdate()
        {
            if (runSpace)
            {
                if (players.Count > 0)
                {
                    foreach (HostedPlayer pl in players.Values)
                        pl.BodySetMove(Time.time);
                    space.Update(Time.fixedDeltaTime);
                }
            }
        }
    }
}


//namespace Dim
//{


//    public class SomePhysic : Plugin
//    {
//        public override bool ThreadSafe => false;
//        public override Version Version => new Version(1, 0, 0);

//        public float interpolationTime = .3f;
//        public float worldStep = 0.02f;
//        public long worldIntStep;
//        public const int MiilsecondsDrop = 5;
//        public int helpDebuStep = 0;

//        Dictionary<IClient, PlayerInputs> players = new Dictionary<IClient, PlayerInputs>();
//        Space space;
//        //public CollisionConfiguration CollisionConf;
//        //public CollisionDispatcher Dispatcher;
//        //public BroadphaseInterface Broadphase;
//        //public ConstraintSolver Solver;
//        Thread th;
//        float lastStepTime;
//        Stopwatch stopWatch;
//        DateTime dt;
//        float lastNotRequestedTime;
//        float Timerek = 0;
//        float tt;
//        long tim;
//        PlayerInputs helpPlayerInput;
//        string collisionLevelName = "mapCol.txt";
//        DamageDeliveryGameMachine ddgm;


//        public SomePhysic(PluginLoadData pluginLoadData) : base(pluginLoadData)
//        {
//            SaveJSONColliders obj;
//            using (StreamReader sr = new StreamReader(collisionLevelName))
//            {
//                string str = sr.ReadToEnd();

//                // Console.WriteLine(str);

//                obj = JsonConvert.DeserializeObject<SaveJSONColliders>(str);


//                // Console.WriteLine(obj.boxColliderList.Count + "   " + obj.circleColliderList.Count);

//                //  Jsonu


//            }
//            WeaponBreak.pistolBreakStep = (ushort)(WeaponBreak.pistolBreak / worldStep);

//            worldIntStep = (long)(1000 * worldStep);
//            stopWatch = Stopwatch.StartNew();

//            CreatePhysicsWorld();

//            AddCollidersToWorld(obj);
//            th = new Thread(ExecuteInForeground2);
//            th.Start();
//            ddgm = new DamageDeliveryGameMachine(players);

//            ClientManager.ClientConnected += ClientConnected;
//            ClientManager.ClientDisconnected += ClientDisconnected;
//        }

//        void CreatePhysicsWorld()
//        {
//            space = new Space();
//            space.ForceUpdater.Gravity = new Vector3(0, -10f, 0);
//        }

//        #region taken

//        //public virtual RigidBody LocalCreateRigidBody(float mass, Matrix startTransform)
//        //{
//        //    CollisionShape sh = new SphereShape(0.5f);
//        //    //rigidbody is dynamic if and only if mass is non zero, otherwise static
//        //    bool isDynamic = (mass != 0.0f);

//        //    BulletSharp.Math.Vector3 localInertia = BulletSharp.Math.Vector3.Zero;
//        //    if (isDynamic)
//        //        sh.CalculateLocalInertia(mass, out localInertia);

//        //    //using motionstate is recommended, it provides interpolation capabilities, and only synchronizes 'active' objects
//        //    DefaultMotionState myMotionState = new DefaultMotionState(startTransform);

//        //    RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, sh, localInertia);
//        //    RigidBody body = new RigidBody(rbInfo);
//        //    rbInfo.Dispose();

//        //    world.AddRigidBody(body);

//        //    return body;
//        //}

//        Entity AddPlayer(float mass, EntityShape shape)
//        {
//            Entity ent = new Entity(shape, mass);
//            ent.Position = Vector3.Zero;
//            space.Add(ent);
//            return ent;
//        }

//        void AddObiect(Vector3 place, EntityShape shape)
//        {
//            Entity ent = new Entity(shape, 0);
//            ent.Position = place;
//            space.Add(ent);
//        }

//        //public virtual RigidBody LocalCreateRigidBody(float mass, Matrix startTransform, CollisionShape shape)
//        //{
//        //    //rigidbody is dynamic if and only if mass is non zero, otherwise static
//        //    bool isDynamic = (mass != 0.0f);

//        //    BulletSharp.Math.Vector3 localInertia = BulletSharp.Math.Vector3.Zero;
//        //    if (isDynamic)
//        //        shape.CalculateLocalInertia(mass, out localInertia);

//        //    //using motionstate is recommended, it provides interpolation capabilities, and only synchronizes 'active' objects
//        //    DefaultMotionState myMotionState = new DefaultMotionState(startTransform);

//        //    RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, shape, localInertia);
//        //    RigidBody body = new RigidBody(rbInfo);
//        //    rbInfo.Dispose();

//        //    world.AddRigidBody(body);

//        //    return body;
//        //}

//        void AddCollidersToWorld(SaveJSONColliders sjc)
//        {
//            foreach (CubeColliterItem bI in sjc.boxColliderList)
//            {
//                AddObiect(new Vector3(bI.posX, bI.posY, bI.posZ), new BoxShape(bI.width, bI.height, bI.longg));
//            }

//            foreach (SphereColliderItem cI in sjc.circleColliderList)
//            {
//                AddObiect(new Vector3(cI.posX, cI.posY, cI.posZ), new SphereShape(cI.radius));
//            }
//        }

//        //void AddCollidersToWorld(SaveJSONColliders sjc)
//        //{
//        //    foreach (BoxColliderItem bI in  sjc.boxColliderList)
//        //    {
//        //        BodyFactory.CreateRectangle(world, bI.width, bI.height, 1, new Vector2(bI.posX, bI.posY));
//        //    }

//        //    foreach (CircleColliderItem cI in sjc.circleColliderList)
//        //    {
//        //        BodyFactory.CreateCircle(world,cI.radius,1, new Vector2(cI.posX, cI.posY));
//        //    }
//        //}
//        #endregion

//        private void ExecuteInForeground2()
//        {
//            while (true)
//            {
//                if (players.Count > 0)
//                {
//                    while (stopWatch.ElapsedMilliseconds - tim >= worldIntStep)
//                    {
//                        tim += worldIntStep;

//                        tt = (stopWatch.ElapsedMilliseconds - (stopWatch.ElapsedMilliseconds - tim)) / 1000f;

//                        foreach (PlayerInputs pi in players.Values)
//                        {
//                            pi.BodySetMove(tt);
//                        }
//                        space.Update(worldStep);
//                        // Console.WriteLine(worldIntStep);
//                        // world.StepSimulation(worldStep);

//                        foreach (PlayerInputs pi in players.Values)
//                        {
//                            pi.SetCalculatedPosition();
//                        }

//                        ddgm.Update();
//                    }
//                }
//                Thread.Sleep(MiilsecondsDrop);
//            }
//        }


//        SphereCharacterController AddPlayer(bool stat = true)
//        {
//            SphereCharacterController ent = new SphereCharacterController();
//            if (stat)
//                ent.Body.BecomeKinematic();
//            // ent.Body.mass = 0;
//            ent.Body.Radius = 1f;
//            ent.Body.Position = Vector3.Zero;
//            space.Add(ent);
//            return ent;
//        }


//        void ClientConnected(object sender, ClientConnectedEventArgs e)
//        {
//            PlayerInputs pI = new PlayerInputs(
//                this,
//                e.Client.ID,
//                (float)0,
//                (float)0,
//                (bool)false,
//                (bool)false,
//                (float)0,
//                (float)0,
//                 (float)0,
//                (float)0,
//                //LocalCreateRigidBody(1f, Matrix.Identity),
//                AddPlayer(false),
//                space,
//                interpolationTime,
//                WeaponBreak.pistolBreakStep
//                );


//            // Console.WriteLine(e.Client + "    " + e.Client.ID);

//            using (DarkRiftWriter newPlayerWriter = DarkRiftWriter.Create())
//            {
//                newPlayerWriter.Write(e.Client.ID);
//                //newPlayerWriter.Write(pI.moveHorizontal);
//                //newPlayerWriter.Write(pI.moveVertical);
//                //newPlayerWriter.Write(pI.shoot);
//                //newPlayerWriter.Write(pI.waitForDim);
//                newPlayerWriter.Write(pI.posX);
//                newPlayerWriter.Write(pI.posY);
//                newPlayerWriter.Write(pI.posZ);

//                //newPlayerWriter.Write(pI.rot);

//                // players.Add(e.Client, pI);
//                using (Message newPlayerMessage = Message.Create(DimTag.SpawnPlayerTag, newPlayerWriter))
//                {
//                    foreach (IClient client in ClientManager.GetAllClients().Where(x => x != e.Client))
//                    {
//                        client.SendMessage(newPlayerMessage, SendMode.Reliable);
//                        Console.WriteLine(client.ID + "   wyslane");
//                    }
//                }
//            }

//            players.Add(e.Client, pI);

//            using (DarkRiftWriter playerWriter = DarkRiftWriter.Create())
//            {
//                foreach (PlayerInputs playerInput in players.Values)
//                {
//                    playerWriter.Write(playerInput.ID);
//                    // playerWriter.Write(pI.moveHorizontal);
//                    // playerWriter.Write(pI.moveVertical);
//                    // playerWriter.Write(pI.shoot);
//                    //playerWriter.Write(pI.waitForDim);
//                    playerWriter.Write(playerInput.posX);
//                    playerWriter.Write(playerInput.posY);
//                    playerWriter.Write(playerInput.posZ);
//                    // playerWriter.Write(pI.rot);
//                }
//                using (Message playerMessage = Message.Create(DimTag.SpawnPlayerTag, playerWriter))
//                {
//                    e.Client.SendMessage(playerMessage, SendMode.Reliable);
//                    Console.WriteLine(e.Client.ID + "   do wchodzącego");
//                }
//            }

//            e.Client.MessageReceived += MovementMessageReceived;
//        }


//        void MovementMessageReceived(object sender, MessageReceivedEventArgs e)
//        {
//            using (Message message = e.GetMessage() as Message)
//            {
//                if (message.Tag == DimTag.MovePlayerTag)
//                {
//                    using (DarkRiftReader reader = message.GetReader())
//                    {
//                        float moveHor = reader.ReadSingle();
//                        float moveVer = reader.ReadSingle();

//                        float rotYaw = reader.ReadSingle();
//                        float rotPitch = reader.ReadSingle();

//                        bool jump = reader.ReadBoolean();
//                        //Console.WriteLine("Odbieram " + rot);
//                        bool shoot = reader.ReadBoolean();

//                        float newX = reader.ReadSingle();
//                        float newY = reader.ReadSingle();
//                        float newZ = reader.ReadSingle();
//                        float clientSendTime = reader.ReadSingle();
//                        float time = reader.ReadSingle();
//                        ushort st = reader.ReadUInt16();

//                        float timer = (stopWatch.ElapsedMilliseconds / 1000f);



//                        if (helpDebuStep > 0)
//                        {
//                            Console.WriteLine(clientSendTime + "    " + st);
//                            // Console.WriteLine(Timerek + "  " + clientSendTime + "    " + stopWatch.ElapsedMilliseconds + "    " + st);

//                            helpDebuStep--;
//                        }

//                        helpPlayerInput = players[e.Client];

//                        //sprawdzenie czy nie ma już takiego kroku w inputach - może duplikat
//                        if (helpPlayerInput.CheckIfStepNotExist(st))
//                        {
//                            helpPlayerInput.RegisterInput(moveHor, moveVer, newX, newY, newZ, timer - time, clientSendTime, jump, shoot, rotYaw, rotPitch, st);


//                            using (DarkRiftWriter writer = DarkRiftWriter.Create())
//                            {
//                                writer.Write(helpPlayerInput.ID);
//                                //writer.Write(moveHor);
//                                //writer.Write(moveVer);
//                                //writer.Write(rot);
//                                writer.Write(helpPlayerInput.posX);
//                                writer.Write(helpPlayerInput.posY);
//                                writer.Write(helpPlayerInput.posZ);
//                                //writer.Write(clientSendTime);
//                                //writer.Write(st);

//                                writer.Write(helpPlayerInput.GetLastCalculatedStep());
//                                writer.Write(helpPlayerInput.GetLastInputStepTime());
//                                //writer.Write(clientSendTime);
//                                message.Serialize(writer);
//                                //  message.Tag = DimTag.MovePlayerTag;
//                            }

//                            e.Client.SendMessage(message, SendMode.Unreliable);
//                            //dupa nie wiem czy tak można
//                            message.Dispose();



//                            using (DarkRiftWriter writer = DarkRiftWriter.Create())
//                            {
//                                writer.Write(helpPlayerInput.ID);
//                                writer.Write(helpPlayerInput.posX);
//                                writer.Write(helpPlayerInput.posY);
//                                writer.Write(helpPlayerInput.posZ);
//                                writer.Write(helpPlayerInput.GetLastCalculatedStep());
//                                writer.Write(time);
//                                writer.Write(moveHor);
//                                writer.Write(moveVer);
//                                writer.Write(rotYaw);
//                                writer.Write(rotPitch);
//                                writer.Write(jump);
//                                writer.Write(shoot);
//                                writer.Write(helpPlayerInput.GetLastShootStep());
//                                writer.Write(st);

//                                message.Serialize(writer);
//                                //  message.Tag = DimTag.MovePlayerTag;
//                            }

//                            // e.Client.SendMessage(message, SendMode.Unreliable);

//                            //using (DarkRiftWriter writerForEveryone = DarkRiftWriter.Create())
//                            //{
//                            //    writerForEveryone.Write(helpPlayerInput.ID);
//                            //    writerForEveryone.Write(helpPlayerInput.posX);
//                            //    writerForEveryone.Write(helpPlayerInput.posY);
//                            //    writerForEveryone.Write(clientSendTime);
//                            //    writerForEveryone.Write(st);
//                            //    message.Serialize(writerForEveryone);

//                            //}

//                            foreach (IClient c in ClientManager.GetAllClients().Where(x => x != e.Client))
//                            {
//                                c.SendMessage(message, SendMode.Unreliable);
//                            }
//                        }
//                    }
//                    return;
//                }

//                if (message.Tag == DimTag.TimeTag)
//                {
//                    using (DarkRiftReader reader = message.GetReader())
//                    {
//                        ushort index = reader.ReadUInt16();
//                        float time = reader.ReadSingle();

//                        using (DarkRiftWriter writer = DarkRiftWriter.Create())
//                        {
//                            writer.Write(index);
//                            writer.Write(time);
//                            writer.Write(worldStep);
//                            writer.Write(interpolationTime);
//                            message.Serialize(writer);
//                            //  message.Tag = DimTag.MovePlayerTag;
//                        }

//                        e.Client.SendMessage(message, SendMode.Reliable);
//                    }
//                    return;
//                }
//            }
//        }


//        void ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
//        {
//            //Console.WriteLine("Usuwam");


//            using (DarkRiftWriter writer = DarkRiftWriter.Create())
//            {
//                writer.Write(e.Client.ID);

//                using (Message message = Message.Create(DimTag.DespawnPlayerTag, writer))
//                {
//                    foreach (IClient client in ClientManager.GetAllClients())
//                        client.SendMessage(message, SendMode.Reliable);
//                }
//            }

//            players[e.Client].RemoveBody();
//            players.Remove(e.Client);
//            Console.WriteLine("Klient usuniety");
//        }


//        //public bool TryHitPlayer(Body b)
//        //{
//        //    foreach (PlayerInputs pi in players.Values)
//        //    {
//        //        if (pi.HitSingle(b))
//        //        {
//        //            // Console.WriteLine("Ktos dostal");
//        //            return true;

//        //        }
//        //    }
//        //    return false;
//        //}
//    }
//}
