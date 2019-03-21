using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BEPUphysics;
using BEPUutilities;
using BEPUphysics.Character;
using BEPUphysics.Entities.Prefabs;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class BepuExample : MonoBehaviour
{
    List<BEPUutilities.Vector2> moveList = new List<BEPUutilities.Vector2>();

    public bool characterRead = true;

    BEPUphysics.Space space, space2;
    public Transform[] boxTransform;
     Box[] box;

    List<Transform> boxList = new List<Transform>();
    List<Box> boxLi = new List<Box>();

    public Transform sphereTr, sphereTr2;

    BEPUphysics.Entities.Entity entity;

    SphereCharacterController sphereChar, secondSphereChar;

    // Start is called before the first frame update
    void Start()
    {
        sphereChar = new SphereCharacterController();
        sphereChar.Body.Position = new BEPUutilities.Vector3(0, 2, 0);

        secondSphereChar = new SphereCharacterController();
        secondSphereChar.Body.Position = new BEPUutilities.Vector3(0, 2, 0);

        // sphereChar.Body.BecomeKinematic();
        // sphereChar.Body.mass = 0;                          

        //single thread world
        space = new BEPUphysics.Space();
        space.ForceUpdater.Gravity = new BEPUutilities.Vector3(0, -9, 0);

        space2 = new BEPUphysics.Space();
        space2.ForceUpdater.Gravity = new BEPUutilities.Vector3(0, -9, 0);


        Box ground = new Box(BEPUutilities.Vector3.Zero, 30, 1, 30);
        Box ground2 = new Box(BEPUutilities.Vector3.Zero, 30, 1, 30);
        // entity = new BEPUphysics.Entities.Entity(new BEPUphysics.CollisionShapes.ConvexShapes.SphereShape(1f), 1f);
        /// entity.position = new BEPUutilities.Vector3(0, 2, 0);
        // space.Add(entity);


        space.Add(ground);
        space2.Add(ground2);
        space.Add(sphereChar);
        space2.Add(secondSphereChar);
        //box = new Box[boxTransform.Length];
        //for (int i = 0; i < boxTransform.Length; i++)
        //{
        //    box[i] = new Box(new BEPUutilities.Vector3(0, 4 *i, 0), 1, 1, 1, 1);
        //    space.Add(box[i]);
        //}
    }

    

    Quaternion BepuToUnityQuaternion(BEPUutilities.Quaternion q)
    {
        return new Quaternion(q.W, q.Z, -q.Y, -q.X);
    }

    UnityEngine.Vector3 BepuToUnityVector3(BEPUutilities.Vector3 vec)
    {
        return new UnityEngine.Vector3(vec.X, vec.Y, vec.Z);
    }

    BEPUutilities.Vector3 UnityToBepuVector3(Vector3 vec)
    {
        return new BEPUutilities.Vector3(vec.x, vec.y, vec.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //for (int i = 0; i < boxTransform.Length; i++)
        //{
        //    boxTransform[i].position = BepuToUnityVector3(box[i].position);
        //}


        if (moveList.Count > 100)
        {
            for (int i = 0; i < moveList.Count; i++)
            {
                secondSphereChar.HorizontalMotionConstraint.MovementDirection = moveList[i];

                space2.Update(Time.fixedDeltaTime);
            }
            sphereTr2.position = BepuToUnityVector3(secondSphereChar.Body.Position);
            moveList.RemoveAt(0);
            moveList.Clear();
           // Debug.Log(moveList.Count);
        }

        if (characterRead)
        {
            BEPUutilities.Vector2 vec = new BEPUutilities.Vector2(Input.GetAxisRaw("Horizontal"), -Input.GetAxisRaw("Vertical"));
            vec.Normalize();
            vec *= 4;
            moveList.Add(new BEPUutilities.Vector2(vec.X, vec.Y));

          //  if (vec == BEPUutilities.Vector2.Zero)
             //   sphereChar.HorizontalMotionConstraint.MovementDirection = BEPUutilities.Vector2.Zero;
          //  else
                sphereChar.HorizontalMotionConstraint.MovementDirection = vec;


            //Jumping
            //if (Input.GetButton("Jump"))
            //{
            //    sphereChar.Jump();
            //}
            sphereTr.position = BepuToUnityVector3(sphereChar.Body.Position);
            space.Update(Time.fixedDeltaTime);



        }
        else
        {
            BEPUutilities.Vector3 vec = new BEPUutilities.Vector3(Input.GetAxisRaw("Horizontal"), space.ForceUpdater.gravity.Y * Time.fixedDeltaTime, Input.GetAxisRaw("Vertical"));
            vec.Normalize();
            vec *= 4;
            //entity.ApplyLinearImpulse(ref vec);
            // entity.ApplyImpulse(entity.position,  vec);
            entity.ApplyImpulseWithoutActivating(ref entity.position, ref vec);
            space.Update(Time.fixedDeltaTime);
            entity.LinearVelocity = BEPUutilities.Vector3.Zero;
            sphereTr.position = BepuToUnityVector3(entity.position);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            boxList.Add(GameObject.CreatePrimitive(PrimitiveType.Cube).transform);
            Box toAdd = new Box(UnityToBepuVector3(Camera.main.transform.position), 1, 1, 1, 1);

            toAdd.LinearVelocity = UnityToBepuVector3(Camera.main.transform.forward) * 10;
            space.Add(toAdd);

            boxLi.Add(toAdd);
        }



        for (int i = 0; i < boxList.Count; i++)
        {
            boxList[i].position = BepuToUnityVector3(boxLi[i].position);
            boxList[i].rotation = BepuToUnityQuaternion(boxLi[i].orientation);
        }

    }
}
