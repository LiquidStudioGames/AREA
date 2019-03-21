using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.Entities;
using Vector3 = BEPUutilities.Vector3;
using Vector2 = BEPUutilities.Vector2;
using Quaternion = BEPUutilities.Quaternion;
using Space = BEPUphysics.Space;
using BEPUphysics.Character;

namespace wolfingame.steamTest
{
    public class ClientConnectorScript : MonoBehaviour
    {
        public GameObject playerPrefab, otherPrefab;

        public NetworkTarget target = NetworkTarget.Host;
        protected BEPUphysics.Space space, clientSpace;
        Dictionary<int, HostedPlayer> hostPlayers = new Dictionary<int, HostedPlayer>();

        HostedPlayer thisPlayer;
        ulong thisPlayerId;
        Transform thisPlayerTr;
        Dictionary<int, ClientPlayer> clientPlayers = new Dictionary<int, ClientPlayer>();
        public NetworkTag netTag;
        bool spaceRun = false;
        bool firstDelayAsign = false;
        float timeDelay = 0;

        int index;
        int thisPlayerIndex = -1;

        // Start is called before the first frame update
        void Start()
        {
            int option = GameSomeEventFill.Instance.Option;

            if (option > 0)
            {
                if (option == 1)
                {
                    //host client
                    MakeStufForHostClient();
                }
                if (option == 2)
                {
                    //client
                    MakeStufForClientOnly();

                }

            }
            else
            {
                Destroy(GameSomeEventFill.Instance.gameObject);
                SceneManager.LoadScene(0);
            }

            netTag.Call(SpawnPlayerToHost, NetworkTarget.Host, new BitStream());
        }



        void MakeStufForClientOnly()
        {
            MakeStuffForClientAndHost();
            spaceRun = false;

        }


        void MakeStufForHostClient()
        {
            MakeStuffForClientAndHost();
            spaceRun = true;
        }

        void MakeStuffForClientAndHost()
        {
            space = new BEPUphysics.Space();
            space.ForceUpdater.Gravity = new BEPUutilities.Vector3(0, -10f, 0);
            clientSpace = new BEPUphysics.Space();
            clientSpace.ForceUpdater.Gravity = new BEPUutilities.Vector3(0, -10f, 0);
            CreateStaticWorld();
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

                clientSpace.Add(
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

                clientSpace.Add(
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

        [NetworkCall]
        private void SpawnPlayerToHost(BitStream stream, SteamPlayer sender)
        {
            // host make some stuf

            AddPlayerToTableAndCreateDiscreateWorld(sender);

            BitStream bs = new BitStream();
            bs.Write(UnityEngine.Vector3.zero);
            bs.Write(index);
            netTag.Call(SpawnPlayerToClient, NetworkTarget.All, bs);
            index++;

        }

        void AddPlayerToTableAndCreateDiscreateWorld(SteamPlayer player)
        {
            SphereCharacterController cont = new SphereCharacterController();

            HostedPlayer pl = new HostedPlayer(space, cont);
            hostPlayers.Add(index, pl);
            
        }

        [NetworkCall]
        private void SpawnPlayerToClient(BitStream stream, SteamPlayer sender)
        {
            UnityEngine.Vector3 place = stream.ReadVector3();
            int ind = stream.ReadInt();
            index = ind + 1;

            if (sender == Game.Instance.Steam.Player)
            {
                SphereCharacterController cont = new SphereCharacterController();
                clientSpace.Add(cont);
                thisPlayer = new HostedPlayer(clientSpace, cont);
                thisPlayerId = sender.ID;
                thisPlayerTr = Instantiate(playerPrefab).transform;
                thisPlayerIndex = ind;
                firstDelayAsign = true;
            }
            else
            {
                ClientPlayer cp = new ClientPlayer();
                cp.obTransform = Instantiate(otherPrefab).transform;
                clientPlayers.Add(ind, cp);
            }
        }

        private void FixedUpdate()
        {
            SpaceUpdate();
            ClientSpaceUpdate();
        }

        private void SpaceUpdate()
        {
            if (spaceRun)
            {
                if (hostPlayers.Count > 0)
                {
                    foreach (HostedPlayer pl in hostPlayers.Values)
                        pl.BodySetMove(Time.time);
                    space.Update(Time.fixedDeltaTime);
                }
            }
        }


        private void ClientSpaceUpdate()
        {
            if (firstDelayAsign)
            {
                Vector2 ve = new Vector2(Input.GetAxisRaw("Horizontal"), -Input.GetAxisRaw("Vertical"));
                ve.Normalize();
                thisPlayer.sphereChar.HorizontalMotionConstraint.MovementDirection = ve;
                clientSpace.Update(Time.fixedDeltaTime);

                Vector3 pos = thisPlayer.sphereChar.Body.Position;
                UnityEngine.Vector3 pos2 = BepuHelp.BepuToUnityVector3(pos);
                thisPlayerTr.position = BepuHelp.BepuToUnityVector3(pos);

                BitStream bs = new BitStream();
                bs.Write(thisPlayerIndex);
                bs.Write(ve.X);
                bs.Write(ve.Y);
                bs.Write(Time.time);
                bs.Write(timeDelay);

                netTag.Call(SendMove, NetworkTarget.Host, bs);
            }
            else
            {

            }
        }


        [NetworkCall]
        private void SendMove(BitStream stream, SteamPlayer sender)
        {
            int ind = stream.ReadInt();
            float x = stream.ReadFloat();
            float y = stream.ReadFloat();
            

            hostPlayers[ind].moveX = x;
            hostPlayers[ind].moveY = y;

            BitStream bs = new BitStream();
            bs.Write(ind);
            bs.Write(x);
            bs.Write(y);
           // bs.Write(sender.ID);

            netTag.Call(SendPositionToAllClient, NetworkTarget.All, bs);
            
            // host make some stuf

        }


        [NetworkCall]
        private void SendPositionToAllClient(BitStream stream, SteamPlayer sender)
        {
            //float posX = stream.ReadFloat();
            //float posY = stream.ReadFloat();
            //ulong id = stream.read

            //if(

        }


        [NetworkCall]
        private void SendTimeToHost(BitStream stream, SteamPlayer sender)
        {

        }
    }
}
