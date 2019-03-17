using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using BulletSharp;
//using BulletSharp.Math;
using System;
using Vector3 = UnityEngine.Vector3;

namespace wolfingame
{
    //public abstract class Demo : IDisposable
    //{

    //    // Frame counting
    //    // public Clock Clock { get; } = new Clock();

    //    public float FrameDelta { get; private set; }
    //    public float FramesPerSecond { get; private set; }
    //    float _frameAccumulator;

    //    // Physics
    //    public DynamicsWorld World { get; protected set; }

    //    public CollisionConfiguration CollisionConf;
    //    public CollisionDispatcher Dispatcher;
    //    public BroadphaseInterface Broadphase;
    //    public ConstraintSolver Solver;

    //    protected BoxShape _shootBoxShape;
    //    private const float ShootBoxInitialSpeed = 40;
    //    //private BodyPicker _bodyPicker;

    //    // Debug drawing
    //    bool _isDebugDrawEnabled;
    //   // DebugDrawModes _debugDrawMode = DebugDrawModes.DrawWireframe;

    //    public void Dispose()
    //    {
    //        Dispose(true);
    //        GC.SuppressFinalize(this);
    //    }

    //    protected virtual void Dispose(bool disposing)
    //    {
    //        if (disposing)
    //        {
    //            ExitPhysics();
    //        }
    //    }

    //    public virtual void ExitPhysics()
    //    {
    //       // _bodyPicker.RemovePickingConstraint();
    //       // this.StandardCleanup();
    //    }
    //    // IDebugDraw _debugDrawer;

    //}


    //    public class MyDispatcher : Dispatcher
    //{

    //}

    //public class MyBrodPhase : BroadphaseInterface
    //{
    //    public override BroadphaseProxy CreateProxy(ref BulletSharp.Math.Vector3 aabbMin, ref BulletSharp.Math.Vector3 aabbMax, int shapeType, IntPtr userPtr, int collisionFilterGroup, int collisionFilterMask, Dispatcher dispatcher)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class BulletWorld : MonoBehaviour//, IDisposable
    {
        public Transform charTransform;
        // BulletSharp.DiscreteDynamicsWorld world;

        public float FrameDelta { get; private set; }
        public float FramesPerSecond { get; private set; }
        float _frameAccumulator;

        // Physics
        BEPUphysics.Space space;
        // public DynamicsWorld World { get; protected set; }

        //public CollisionConfiguration CollisionConf;
        //public CollisionDispatcher Dispatcher;
        //public BroadphaseInterface Broadphase;
        //public ConstraintSolver Solver;

        //protected BoxShape _shootBoxShape;
        private const float ShootBoxInitialSpeed = 40;
        //private BodyPicker _bodyPicker;

        // Debug drawing
        // bool _isDebugDrawEnabled;
        // DebugDrawModes _debugDrawMode = DebugDrawModes.None;



        //private PairCachingGhostObject _ghostObject;
        //private KinematicCharacterController _character;
        //private ClosestConvexResultCallback _convexResultCallback;
        //private SphereShape _cameraSphere;
        //private CapsuleShape _capsuleShape;



        //public virtual BEPUphysics.Entities.Entity LocalCreateRigidBody(float mass, BEPUutilities.Vector3 position, BEPUphysics.Entities.Entity entity)
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

        private void CreateGround()
        {
            var groundShape = new BEPUphysics.Entities.Prefabs.Box(new BEPUutilities.Vector3(0, -4, 0), 25, 1, 25);
            space.Add(groundShape);

            var band = new BEPUphysics.Entities.Prefabs.Box(new BEPUutilities.Vector3(0, -4, 24), 25, 5, 1);
            space.Add(groundShape);

            var band1 = new BEPUphysics.Entities.Prefabs.Box(new BEPUutilities.Vector3(0, -4, -24), 25, 5, 1);
            space.Add(groundShape);

            var band2 = new BEPUphysics.Entities.Prefabs.Box(new BEPUutilities.Vector3(24, -4, 0), 1, 5, 25);
            space.Add(groundShape);

            var band3 = new BEPUphysics.Entities.Prefabs.Box(new BEPUutilities.Vector3(-24, -4, 0), 1, 5, 25);
            space.Add(groundShape);
        }
    }

        // Start is called before the first frame update
    //    void Start()
    //    {
    //        // collision configuration contains default setup for memory, collision setup
    //        CollisionConf = new DefaultCollisionConfiguration();
    //        Dispatcher = new CollisionDispatcher(CollisionConf);

    //        Broadphase = new DbvtBroadphase();
    //        Solver = new SequentialImpulseConstraintSolver();

    //        World = new DiscreteDynamicsWorld(Dispatcher, Broadphase, Solver, CollisionConf);
    //        World.DispatchInfo.AllowedCcdPenetration = 0.0001f;
    //        World.Gravity = new BulletSharp.Math.Vector3(0, -10, 0);

    //        CreateGround();

    //        Broadphase.OverlappingPairCache.SetInternalGhostPairCallback(new GhostPairCallback());

    //        const float characterHeight = 1.75f;
    //        const float characterWidth = 1;
    //        _capsuleShape = new CapsuleShape(characterWidth, characterHeight);
    //        _ghostObject = new PairCachingGhostObject()
    //        {
    //            CollisionShape = _capsuleShape,
    //            CollisionFlags = BulletSharp.CollisionFlags.CharacterObject,
    //            WorldTransform = Matrix.Translation(0, 0, 0)
    //        };
    //        World.AddCollisionObject(_ghostObject, CollisionFilterGroups.CharacterFilter, CollisionFilterGroups.StaticFilter | CollisionFilterGroups.DefaultFilter);



    //       // BulletSharp.Math.Vector3 vec = BulletSharp.Math.Vector3.Zero;
    //        const float stepHeight = 0.35f;
    //        _character = new BulletSharp.KinematicCharacterController(_ghostObject, _capsuleShape, stepHeight);
    //        World.AddAction(_character);

    //        //BulletSharp.RigidBody rig = new RigidBody(
    //    }

    //    // Update is called once per frame
    //    void Update()
    //    {

    //    }


    //    public static Vector3 VectUnityFromBulletVec(BulletSharp.Math.Vector3 vec)
    //    {
    //        return new Vector3(vec.X, vec.Y, vec.Z);
    //    }

    //    private void FixedUpdate()
    //    {
    //        BulletSharp.Math.Vector3 walkDirection = new BulletSharp.Math.Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    //        walkDirection.Normalize();
    //        float walkSpeed = 10f * Time.fixedDeltaTime;

    //        if (Input.GetButton("Jump"))
    //            _character.Jump();

    //        charTransform.position = VectUnityFromBulletVec(_character.GhostObject.WorldTransform.Origin);

    //        _character.SetWalkDirection(walkDirection * walkSpeed);
    //        World.StepSimulation(Time.fixedDeltaTime);
    //       // World.StepSimulation(Time.time, 3, Time.fixedDeltaTime);
    //    }


    //    private void OnDestroy()
    //    {
    //        Dispose();
    //    }

    //    public void Dispose()
    //    {
    //        // Dispose(true);
    //        //GC.SuppressFinalize(this);
    //        //_ghostObject.Dispose();
    //        //Solver.Dispose();
    //        //Broadphase.Dispose();
    //        //CollisionConf.Dispose();
    //        //Dispatcher.Dispose();
    //        //World.Dispose();

    //        World.Dispose();
    //    }

    //    protected virtual void Dispose(bool disposing)
    //    {
    //        if (disposing)
    //        {
    //            ExitPhysics();
    //        }
    //    }

    //    public virtual void ExitPhysics()
    //    {
            
    //        // _bodyPicker.RemovePickingConstraint();
    //        // this.StandardCleanup();
    //    }
    //}
}
