using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICities;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.UI;

namespace Gurkenplayer
{
    class ConfigurationPanel : UIPanel //Testing
    {
        //Fields
        UILabel lbl_Gurkenplayer;
        UILabel lbl_Username;
        UITextField txt_Username;
        UILabel lbl_Password;
        UITextField txt_Password;
        //Client area
        UILabel lbl_Client;
        UILabel lbl_ClientIP;
        UITextField txt_ClientIP;
        UILabel lbl_ClientPort;
        UITextField txt_ClientPort;
        UIButton btn_ClientConnect;
        //Server area
        UILabel lbl_Server;
        UILabel lbl_ServerPlayers;
        UILabel lbl_ServerPlayersValue; //Maximum players count is fixed to 2 atm
        UILabel lbl_ServerPort;
        UITextField txt_ServerPort;
        UIButton btn_ServerStart;

        //Methods
        public override void Start()
        {
            Log.MessageUnity("Configuration panel start...");
            SimulationManager.instance.ForcedSimulationPaused = true;

            UIDragHandle dh = (UIDragHandle)this.AddUIComponent(typeof(UIDragHandle)); //Activates the dragging of the window

            try
            {
                //Configures this window
                this.backgroundSprite = "GenericPanel";
                this.color = new Color32(153, 255, 153, 200);
                this.width = 385;
                this.height = 523;
                this.relativePosition = new Vector3(700, 270, 0);
                Log.MessageUnity(String.Format("panel height:{0} width:{1} position x-y-z:{2}-{3}-{4} relposition x-y-z:{5}-{6}-{7} transposition x-y-z:{8}-{9}-{10}", this.height, this.width, this.position.x, this.position.y, this.position.z, this.relativePosition.x, this.relativePosition.y, this.relativePosition.z, this.transformPosition.x, this.transformPosition.y, this.transformPosition.z));

                lbl_Gurkenplayer = (UILabel)this.AddUIComponent(typeof(UILabel));
                lbl_Gurkenplayer.text = "Gurkenplayer";
                lbl_Gurkenplayer.textScale = 1.6f;
                lbl_Gurkenplayer.position = new Vector3(110, -20, 0);

                lbl_Username = (UILabel)this.AddUIComponent(typeof(UILabel));
                lbl_Username.text = "Username:";
                lbl_Username.textScale = 1.1f;
                lbl_Username.position = new Vector3(40, -55, 0);

                txt_Username = (UITextField)this.AddUIComponent(typeof(UITextField));
                //txt_Username.width = 210;
                //txt_Username.height = 22;
                //txt_Username.color = new Color32(0, 255, 0, 150);
                //txt_Username.position = new Vector3(110, -55, 0);

                lbl_Password = (UILabel)this.AddUIComponent(typeof(UILabel));
                lbl_Password.text = "Password:";
                lbl_Password.textScale = 1.1f;
                lbl_Password.position = new Vector3(40, -90, 0);

                txt_Password = (UITextField)this.AddUIComponent(typeof(UITextField));
                //txt_Password.width = 210;
                //txt_Password.height = 22;
                //txt_Password.color = new Color32(0, 255, 0, 150);
                //txt_Password.position = new Vector3(110, -90, 0);

                lbl_Client = (UILabel)this.AddUIComponent(typeof(UILabel));
                lbl_Client.text = "Client";
                lbl_Client.textScale = 1.3f;
                lbl_Client.position = new Vector3(145, -140, 0);

                lbl_ClientIP = (UILabel)this.AddUIComponent(typeof(UILabel));
                lbl_ClientIP.text = "IP:";
                lbl_ClientIP.textScale = 1.1f;
                lbl_ClientIP.position = new Vector3(80, -175, 0);

                txt_ClientIP = (UITextField)this.AddUIComponent(typeof(UITextField));
                //txt_ClientIP.width = 210;
                //txt_ClientIP.height = 22;
                //txt_ClientIP.isEnabled = true;
                //txt_ClientIP.color = new Color32(0, 255, 0, 150);
                //txt_ClientIP.position = new Vector3(110, -175, 0);

                lbl_ClientPort = (UILabel)this.AddUIComponent(typeof(UILabel));
                lbl_ClientPort.text = "Port:";
                lbl_ClientPort.textScale = 1.1f;
                lbl_ClientPort.position = new Vector3(70, -210, 0);

                txt_ClientPort = (UITextField)this.AddUIComponent(typeof(UITextField));
                //txt_ClientPort.width = 210;
                //txt_ClientPort.height = 22;
                //txt_ClientPort.isEnabled = true;
                //txt_ClientPort.color = new Color32(0, 255, 0, 150);
                //txt_ClientPort.position = new Vector3(110, -210, 0);

                btn_ClientConnect = (UIButton)this.AddUIComponent(typeof(UIButton));
                btn_ClientConnect.text = "Connect";
                btn_ClientConnect.width = 280;
                btn_ClientConnect.height = 22;
                btn_ClientConnect.position = new Vector3(40, -240, 0);
                btn_ClientConnect.normalBgSprite = "ButtonMenu";
                btn_ClientConnect.disabledBgSprite = "ButtonMenuDisabled";
                btn_ClientConnect.hoveredBgSprite = "ButtonMenuHovered";
                btn_ClientConnect.focusedBgSprite = "ButtonMenuFocused";
                btn_ClientConnect.pressedBgSprite = "ButtonMenuPressed";
                btn_ClientConnect.textColor = new Color32(255, 51, 153, 150);
                btn_ClientConnect.disabledTextColor = new Color32(7, 7, 7, 200);
                btn_ClientConnect.hoveredTextColor = new Color32(255, 255, 255, 255);
                btn_ClientConnect.pressedTextColor = new Color32(204, 0, 0, 255);
                btn_ClientConnect.playAudioEvents = true;
                btn_ClientConnect.eventClick += btn_ClientConnect_eventClick;

                lbl_Server = (UILabel)this.AddUIComponent(typeof(UILabel));
                lbl_Server.text = "Server";
                lbl_Server.textScale = 1.3f;
                lbl_Server.position = new Vector3(145, -290, 0);

                lbl_ServerPlayers = (UILabel)this.AddUIComponent(typeof(UILabel));
                lbl_ServerPlayers.text = "Players:";
                lbl_ServerPlayers.textScale = 1.1f;
                lbl_ServerPlayers.position = new Vector3(60, -325, 0);

                lbl_ServerPlayersValue = (UILabel)this.AddUIComponent(typeof(UILabel));
                lbl_ServerPlayersValue.text = "2";
                lbl_ServerPlayersValue.textScale = 1.1f;
                lbl_ServerPlayersValue.position = new Vector3(110, -325, 0);

                lbl_ServerPort = (UILabel)this.AddUIComponent(typeof(UILabel));
                lbl_ServerPort.text = "Port:";
                lbl_ServerPort.textScale = 1.1f;
                lbl_ServerPort.position = new Vector3(70, -355, 0);

                txt_ServerPort = (UITextField)this.AddUIComponent(typeof(UITextField));
                //txt_ServerPort.width = 210;
                //txt_ServerPort.height = 22;
                //txt_ServerPort.isEnabled = true;
                //txt_ServerPort.color = new Color32(0, 255, 0, 150);
                //txt_ServerPort.position = new Vector3(110, -355, 0);

                btn_ServerStart = (UIButton)this.AddUIComponent(typeof(UIButton));
                btn_ServerStart.text = "Start lobby";
                btn_ServerStart.width = 280;
                btn_ServerStart.height = 22;
                btn_ServerStart.position = new Vector3(40, -385, 0);
                btn_ServerStart.normalBgSprite = "ButtonMenu";
                btn_ServerStart.disabledBgSprite = "ButtonMenuDisabled";
                btn_ServerStart.hoveredBgSprite = "ButtonMenuHovered";
                btn_ServerStart.focusedBgSprite = "ButtonMenuFocused";
                btn_ServerStart.pressedBgSprite = "ButtonMenuPressed";
                btn_ServerStart.textColor = new Color32(255, 51, 153, 150);
                btn_ServerStart.disabledTextColor = new Color32(7, 7, 7, 200);
                btn_ServerStart.hoveredTextColor = new Color32(255, 255, 255, 255);
                btn_ServerStart.pressedTextColor = new Color32(204, 0, 0, 255);
                btn_ServerStart.playAudioEvents = true;
                btn_ServerStart.eventClick += btn_ServerStart_eventClick;
            }
            catch (Exception ex)
            {
                Log.ErrorUnity("ABC" + ex.ToString());
            }
        }

        void btn_ServerStart_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            Log.WarningUnity("Server Start lobby click");
        }

        void btn_ClientConnect_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            Log.WarningUnity("Client Connect clicked");
        }

        public override void OnDisable()
        {
            SimulationManager.instance.ForcedSimulationPaused = false;
            base.OnDisable();
        }
    }
}
//Info https://media.readthedocs.org/pdf/skylines-modding-docs/master/skylines-modding-docs.pdf
// http://skylines-modding-docs.readthedocs.org/en/latest/modding/Development/How-to-Use-ColossalFramework.UI.html