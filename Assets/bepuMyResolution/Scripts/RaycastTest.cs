//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
////using VelcroPhysics.Dynamics;
////using VelcroPhysics.Shared;


//public class RaycastTest : MonoBehaviour
//{

//    public LineRenderer[] lineRend;
//    int startLineRendererIndex = 0;
//    int lastLineRendererIndex;
//    public LineRenderer prefab;

//    public Dim.MoveControl mc;
//    private VelcroPhysics.Dynamics.World world;
//    private VelcroPhysics.Dynamics.Body body;
//    int currentStep = 0;
//    int stepWhenICanShoot = 10;
//    //public Dim.NetworkPlayerDimManager man;

//   // public UnityEngine.Transform start, end, shootPlace;

//    //VelcroPhysics.Collision.Handlers.OnSeparationHandler HanDler()
//    //{
//    //    return new VelcroPhysics.Collision.Handlers.OnSeparationHandler((
//    //}

//    void NaKolizje()
//    {

//    }


//    public static class WeaponBreak
//    {
//        public static float pistolBreak = 0.2f;
//    }

//    // Start is called before the first frame update
//    void Start()
//    {
//        lastLineRendererIndex = lineRend.Length;
        
//        //world = man.GetWorld;
//        world = FindObjectOfType<Dim.NetworkPlayerDimManager>().GetWorld;

//        foreach (Body bo in world.BodyList)
//        {
//            if (bo.IsStatic)
//            {
//               // bo.OnCollision += NaKolizje;
                    
//            }
//        }
//        //RaysCastCallback callback;
//        //world.RayCast(reportRayFixture, Microsoft.Xna.Framework.Vector2.Zero, Microsoft.Xna.Framework.Vector2.Zero);
//       // start.SetParent(null);
//       // end.SetParent(null);
//       // shootPlace.SetParent(null);
//    }




//    //public void RayCast(RayCastCallback callback, Vector2 point1, Vector2 point2)
//    //{
//    //    RayCastInput input = new RayCastInput();
//    //    input.MaxFraction = 1.0f;
//    //    input.Point1 = point1;
//    //    input.Point2 = point2;

//    //    _rayCastCallback = callback;
//    //    ContactManager.BroadPhase.RayCast(_rayCastCallbackWrapper, ref input);
//    //    _rayCastCallback = null;
//    //}

//    void Calback(Fixture fix, Microsoft.Xna.Framework.Vector2 pointOne, Microsoft.Xna.Framework.Vector2 pointTwo, float a, float b)
//    {

//    }
//    //kind of arena
//    private void DrawLine(Vector3 endPos)
//    {
//        LineRenderer lineRend = Instantiate(prefab);
//        // lineRend.SetPosition(0, start.position);
//        lineRend.SetPosition(0, posStart);
//        lineRend.SetPosition(1, endPos);
//       // lineRend.startColor = Color.green;
//       // lineRend.endColor = Color.green;
//        Destroy(lineRend.gameObject, .2f);
//    }

//    Vector3 posStart, posCheck, posEnd;

//    float reportRayFixture(Fixture fixture,
//                           Microsoft.Xna.Framework.Vector2 point,
//                           Microsoft.Xna.Framework.Vector2 normal,
//                           float fraction)
//    {

//        if (fixture.Body == nearest)
//        {
//            posEnd.x = point.X;
//            posEnd.y = point.Y;
//           // Vector3 p = new Vector3(point.X, point.Y, 0);
//            //shootPlace.position = p;
//            find = true;
//            // DrawLine(shootPlace.position);
//            DrawLine(posEnd);
//            return 1;
            
//        }

//        vec = point;


//        return 0;
//    }

//    bool place = false;
//    float dist = 0;
//    Body nearest;
//    bool find = false;
//    Microsoft.Xna.Framework.Vector2 vec;

//    void RaycastWeapon()
//    {
//        //  find = false;
//        // // place = false;
//        //  dist = 0;
//        //  // start.position = transform.position;
//        //  posStart = transform.position;
//        //  Microsoft.Xna.Framework.Vector2 one = mc.GetBody().Position;// new Microsoft.Xna.Framework.Vector2(transform.position.x, transform.position.y);
//        //  // Vector2 wo = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
//        //  // Microsoft.Xna.Framework.Vector2 v = new Microsoft.Xna.Framework.Vector2(wo.x, wo.y);
//        //  Microsoft.Xna.Framework.Vector2 v = new Microsoft.Xna.Framework.Vector2(Mathf.Cos(transform.eulerAngles.z*Mathf.Deg2Rad), Mathf.Sin(transform.eulerAngles.z*Mathf.Deg2Rad));
//        //  v.Normalize();
//        //  v *= 20f;
//        //  // v = v;
//        //  //end.position = new Vector3(one.X+v.X,one.Y+v.Y, 0);
//        //  posCheck = new Vector3(one.X + v.X, one.Y + v.Y, 0);


//        //  //world.r




//        //  // world.RayCast(reportRayFixture, one, (one + v));
//        //  List<Fixture> list = world.RayCast( one, one + v);
//        //  // Debug.Log(list.Count);

//        //  Microsoft.Xna.Framework.Vector2 centerVec = Microsoft.Xna.Framework.Vector2.Zero;

//        //  foreach (Fixture f in list)
//        //  {
//        //      centerVec += f.Body.Position;
//        //  }

//        //  centerVec /= list.Count;

//        //  foreach (Fixture fi in list)
//        //  {
//        //     // Microsoft.Xna.Framework.Vector2 v2 = fi.Body.Position;

//        //      Microsoft.Xna.Framework.Vector2 v3 = mc.GetBody().Position;
//        ////      fi.


//        //      float m = Microsoft.Xna.Framework.Vector2.Distance(centerVec, v3);
//        //      // Debug.Log(m);
//        //      if (dist == 0)
//        //      {
//        //          dist = m;
//        //          //posEnd = new Vector3(.X, v2.Y, 0);
//        //          nearest = fi.Body;
//        //      }
//        //      else
//        //      {
//        //          if (dist > m)
//        //          {
//        //              dist = m;
//        //            // posEnd = new Vector3(v2.X, v2.Y, 0);
//        //              nearest = fi.Body;
//        //          }
//        //      }
//        //  }

//        //  if (list.Count == 0)
//        //  {
//        //      find = true;
//        //      DrawLine(posCheck);
//        //  }
//        //  // v.Normalize();
//        //  //  v*= dist;

//        //  if (!find)
//        //  for(int i =0;i<list.Count;i++)
//        //  {
//        //         // if (find)
//        //             // break;
//        //      world.RayCast(reportRayFixture, one, (one + v));
//        //      if (!find)
//        //      {
//        //          v.Normalize();

//        //          v *= Microsoft.Xna.Framework.Vector2.Distance(one, vec) - 0.1f;
//        //      }
//        //  }


//        Microsoft.Xna.Framework.Vector2 one = mc.GetBody().Position;
//        posStart.x = one.X;
//        posStart.y = one.Y;

//        Microsoft.Xna.Framework.Vector2 v2;

//        Microsoft.Xna.Framework.Vector2 v = new Microsoft.Xna.Framework.Vector2(Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad));
//        v.Normalize();
//        v2 = v * -0.01f;
//        v *= 20f;

//        posCheck.x = (one + v).X;
//        posCheck.y = (one + v).Y;

//        int count = world.RayCast(one, one + v).Count;

//        if (count > 0)
//        {
//            if (count > 1)
//            {
//                //distance = 0;
//                hitPoint = one + v;
//                // world.RayCast(ReportRayFixture2, one, one + v);
//                for (int i = 0; i < count; i++)
//                {
//                    if(i == 0)
//                        world.RayCast(ReportRayFixture2, one, hitPoint);
//                    else
//                    world.RayCast(ReportRayFixture2, one, hitPoint + v2);
//                }

//            }
//            else
//            {
//                world.RayCast(ReportRayFixture2, one, one + v);
//            }

//            DrawLine(posEnd);
//        }
//        else
//        {
//            DrawLine(posCheck);
//        }
//        //if (fi.Body.IsDynamic)
//        //{
//        //    Debug.Log("gracz");
//        //}
//        //if (fi.Body.IsStatic)
//        //{
//        //    Debug.Log("Sciana");
//        //}
//        // fi.
//        // shootPlace.position = new Vector3(fi.Body.Position.X, fi.Body.Position.Y, 0);
//        // shootPlace.position = new Vector3(v2.X, v2.Y);
//        // Debug.Log(fi.ToString());
//        //fi.OnCollision = OnCol;
//        // }

//        //end.position = new Vector3((one + v).X, (one + v).Y, 0);
//        //v.Normalize();
//        ////  dist = Mathf.Pow(dist, 0.5f) + 0.1f;
//        //v = v * (dist + 0.2f);

//        //// end.position = new Vector3((one + v).X, (one + v).Y, 0);

//        //Debug.Log(Microsoft.Xna.Framework.Vector2.Distance(one, one + v));
//        //world.RayCast(reportRayFixture, one, (one + v));
//        //dist = 0;

//    }

//    float distance = 0;
//    Microsoft.Xna.Framework.Vector2 hitPoint = Microsoft.Xna.Framework.Vector2.Zero;
    


//    float ReportRayFixture2(Fixture fixture,
//                      Microsoft.Xna.Framework.Vector2 point,
//                      Microsoft.Xna.Framework.Vector2 normal,
//                      float fraction)
//    {
//        hitPoint = point;
//        SetEndPoint();
//        return 0;
//    }

//    int tryCatch = 0;
//    float ReportRayFixture(Fixture fixture,
//                          Microsoft.Xna.Framework.Vector2 point,
//                          Microsoft.Xna.Framework.Vector2 normal,
//                          float fraction)
//    {
//       // if (distance == 0)
//       // {
//          //  distance = Microsoft.Xna.Framework.Vector2.DistanceSquared(mc.GetBody().Position, point);
//            hitPoint = point;
//            //Debug.Log(hitPoint);
//            SetEndPoint();
//      //  }
//       // else
//       // {
//           // float d = Microsoft.Xna.Framework.Vector2.DistanceSquared(mc.GetBody().Position, point);
//          //  if (d < distance)
//           // {
//             //   hitPoint = point;
//               // Debug.Log(hitPoint);
//                // world.RayCast(ReportRayFixture2, mc.GetBody().Position, point);
//              //  SetEndPoint();
//           // }
//      //  }

//        //if (fixture.Body == nearest)
//        //{
//        //    posEnd.x = point.X;
//        //    posEnd.y = point.Y;
//        //    // Vector3 p = new Vector3(point.X, point.Y, 0);
//        //    //shootPlace.position = p;
//        //    find = true;
//        //    // DrawLine(shootPlace.position);
//        //    DrawLine(posEnd);
//        //    return 1;

//        //}

//        //vec = point;


//        return 0;
//    }


   

//    void SetEndPoint()
//    {
//        posEnd.x = hitPoint.X;
//        posEnd.y = hitPoint.Y;
//    }

//    void OnCol(Fixture a, Fixture b, VelcroPhysics.Collision.ContactSystem.Contact cont)
//    {
//        Debug.Log(a.ToString() + "   " + b.ToString());
//    }

//    // Update is called once per frame
//    void FixedUpdate()
//    {
//        if (Input.GetButton("Fire1") && currentStep ==0)
//        {
//            RaycastWeapon();
//            currentStep = 1;
//        }

//        if (currentStep > 0)
//        {
//            currentStep++;
//            if (currentStep >= stepWhenICanShoot)
//                currentStep = 0;
//        }
//    }
//}
