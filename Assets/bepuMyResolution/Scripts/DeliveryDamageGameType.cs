using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Dim
{
    public class DeliveryDamageGameType : MonoBehaviour
    {
        float timeInfo;
        bool run = false;
        UnityClient uC;
        ushort clientId;
        public TextMeshProUGUI thisPlayerDamText, otherPlayerDamageText;
        ushort thisPlayerDamage, otherPlayerDamage;
        public Slider slider;
        public string wonInfo = "You won", lostInfo = "You lost", passInfo = "Balance";
        public TextMeshProUGUI infoText;
        NetworkPlayerDimManager np;



        private void FixedUpdate()
        {
            if (run)
            {
                timeInfo += Time.deltaTime;
                infoText.text = timeInfo.ToString("F0");
            }
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

                np = FindObjectOfType<NetworkPlayerDimManager>();
                uC.MessageReceived += MessageTimeRecive;
                clientId = uC.ID;
            }
        }

        void HidEndGameInfo()
        {

        }

        void SetSlider()
        {
            if (thisPlayerDamage + otherPlayerDamage > 0)
                slider.value = (float)((float)thisPlayerDamage / (float)(thisPlayerDamage + otherPlayerDamage));
            else
                slider.value = 0.5f;
           // Debug.Log(thisPlayerDamage + "    " + otherPlayerDamage + "      " + slider.value);
        }

        void MessageTimeRecive(object sender, MessageReceivedEventArgs e)
        {
            using (Message message = e.GetMessage() as Message)
            {
                if (message.Tag == DimTag.DeliveryDamageInfo)
                {
                    using (DarkRiftReader reader = message.GetReader())
                    {
                        ushort index = reader.ReadUInt16();
                        ushort damage = reader.ReadUInt16();
                        ushort damageTaken = reader.ReadUInt16();


                        if (index == clientId)
                        {
                            thisPlayerDamage = damage;
                            thisPlayerDamText.text = damage.ToString();
                        }
                        else
                        {
                            otherPlayerDamage = damage;
                            otherPlayerDamageText.text = damage.ToString();
                        }
                        SetSlider();

                      //  Debug.Log(slider.value);

                    }
                    return;
                }

                if (message.Tag == DimTag.DeliveryDamageBreakStart)
                {
                    infoText.text = "";
                    thisPlayerDamage = otherPlayerDamage = 0;
                    timeInfo = 0;
                    run = true;
                    return;
                }


                if (message.Tag == DimTag.DeliveryDamageMatchStart)
                {
                    thisPlayerDamage = otherPlayerDamage = 0;
                    timeInfo = 0;
                    run = true;
                    return;
                }

                if (message.Tag == DimTag.DeliveryDamageMatchEndStart)
                {
                    timeInfo = 0;
                    run = false;

                    if (np.GetPlayersCount() > 1)
                    {
                        if (thisPlayerDamage > otherPlayerDamage)
                        {
                            infoText.text = wonInfo;

                        }
                        else
                        {
                            if (otherPlayerDamage > thisPlayerDamage)
                                infoText.text = lostInfo;
                            else
                                infoText.text = passInfo;

                        }
                    }
                    else
                        infoText.text = wonInfo;
                    return;
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
    }
}
