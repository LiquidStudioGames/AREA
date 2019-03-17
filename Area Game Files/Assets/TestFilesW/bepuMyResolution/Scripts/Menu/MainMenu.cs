using UnityEngine;
using UnityEngine.UI;
using System.Net;
//using UnityEngine.Experimental.UIElements;
//using TMPro;


namespace Dim
{
    public class MainMenu : MonoBehaviour
    {
        public UnityEngine.UI.Button startBtn, networkChangeBtn, exitBtn;
        public InputField ipAdresInputField;
        public Text infoText;
        public Text networkChangText;
        public string lanDesc = "Lan", netDesc = "Net";
        public DarkRift.Client.Unity.UnityClient client;
        public string lanIPDesc = "127.0.0.1", netIPDesc = "127.0.0.1";
        public NetworkPlayerDimManager npdm;
        public GameObject MainMenuPanelGob;

        bool lan = true;


        void Start()
        {
            exitBtn.onClick.AddListener(EndGame);
            networkChangeBtn.onClick.AddListener(ChangeNetworkArea);
            startBtn.onClick.AddListener(StartGame);
            LanOrNet();
            infoText.text = "";
            //ipAdresInputField.onValueChanged = ChangeLocalIp();
            ipAdresInputField.onValueChanged.AddListener(delegate { ChangeLocalIp(ipAdresInputField.text); });
            // client.error
            client.Disconnected += Diconnect; 
            
        }

        void Diconnect(object sender,DarkRift.Client.DisconnectedEventArgs arg)
        {
            EndGame2();
        }

        void ChangeLocalIp(string to)
        {
            lanIPDesc = to;
            LanOrNet();
        }


        void ChangeNetworkArea()
        {
            lan = !lan;
            LanOrNet();
        }


        void LanOrNet()
        {
            if (lan)
            {
                networkChangText.text = lanDesc;
                //Client.ConnectInBackground(IPAddress.Parse(IpAdress), Port, IPVersion.IPv4, ConnectCallback);
                client.Address = IPAddress.Parse(lanIPDesc);
                ipAdresInputField.gameObject.SetActive(true) ;
                ipAdresInputField.text = lanIPDesc;
            }
            else
            {
                networkChangText.text = netDesc;
                client.Address = IPAddress.Parse(netIPDesc);
                ipAdresInputField.gameObject.SetActive(false);
                ipAdresInputField.text = netIPDesc;
            }
        }


        void EndGame()
        {
            Application.Quit();
        }


        void StartGame()
        {
            client.Connect(client.Address, client.Port, client.IPVersion);
           // client.
            MainMenuPanelGob.SetActive(false);
        }

        void EndGame2()
        {
            //sprzątnij za sobą obiekty
            MainMenuPanelGob.SetActive(true);
            SetPositionFromServer[] sc = FindObjectsOfType<SetPositionFromServer>();
            foreach (SetPositionFromServer s in sc)
                Destroy(s.gameObject);
            npdm.ClearAllPlayerDic();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {

                client.Disconnect();
                EndGame2();
            }
        }
    }
}
