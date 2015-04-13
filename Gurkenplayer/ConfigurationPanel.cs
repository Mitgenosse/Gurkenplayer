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
        //Client area
        UILabel lbl_Client;
        UILabel lbl_ClientIP;
        UITextField txt_ClientIP;
        UILabel lbl_ClientPassword;
        UITextField txt_ClientPassword;
        UIButton btn_ClientConnect;
        //Server area
        UILabel lbl_Server;
        UILabel lbl_ServerPassword;
        UITextField txt_ServerPassword;
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
                this.color = new Color32(153, 255, 153, 100);
                this.width = 600;
                this.height = 500;
                this.relativePosition = new Vector3(700, 270, 0);
                this.eventPositionChanged += ConfigurationPanel_eventPositionChanged;
                Log.MessageUnity(String.Format("panel height:{0} width:{1} position x-y-z:{2}-{3}-{4} relposition x-y-z:{5}-{6}-{7} transposition x-y-z:{8}-{9}-{10}", this.height, this.width, this.position.x, this.position.y, this.position.z, this.relativePosition.x, this.relativePosition.y, this.relativePosition.z, this.transformPosition.x, this.transformPosition.y, this.transformPosition.z));

                lbl_Gurkenplayer = (UILabel)this.AddUIComponent(typeof(UILabel));
                lbl_Gurkenplayer.text = "Gurkenplayer";
                lbl_Gurkenplayer.textScale = 1.6f;
                lbl_Gurkenplayer.position = new Vector3(100, -20, 0);
                Log.MessageUnity(String.Format("label height:{0} width:{1} relposition x-y-z:{2}-{3}-{4} transposition x-y-z:{5}-{6}-{7}", lbl_Gurkenplayer.height, lbl_Gurkenplayer.width, lbl_Gurkenplayer.relativePosition.x, lbl_Gurkenplayer.relativePosition.y, lbl_Gurkenplayer.relativePosition.z, lbl_Gurkenplayer.transformPosition.x, lbl_Gurkenplayer.transformPosition.y, lbl_Gurkenplayer.transformPosition.z));

                lbl_Username = (UILabel)this.AddUIComponent(typeof(UILabel));
                lbl_Username.text = "Username:";
                lbl_Username.textScale = 1.1f;
                lbl_Username.position = new Vector3(40, -55, 0);

                //txt_Username = (UITextField)this.AddUIComponent(typeof(UITextField));
                ////txt_Username.isEnabled = true;
                ////txt_Username.color = new Color32(0, 255, 0, 100);
                //txt_Username.position = new Vector3(110, -55, 0);

                lbl_Client = (UILabel)this.AddUIComponent(typeof(UILabel));
                lbl_Username.text = "Client";
                lbl_Username.textScale = 1.3f;
                lbl_Username.position = new Vector3(110, -110, 0);

            }
            catch (Exception ex)
            {
                Log.ErrorUnity("ABC" + ex.ToString());
            }
        }

        public override void OnDisable()
        {
            SimulationManager.instance.ForcedSimulationPaused = false;
            base.OnDisable();
        }

        //Events
        void ConfigurationPanel_eventPositionChanged(UIComponent component, Vector2 value)
        {
            Log.MessageUnity(String.Format("panel height:{0} width:{1} position x-y-z:{2}-{3}-{4} relposition x-y-z:{5}-{6}-{7} transposition x-y-z:{8}-{9}-{10}", this.height, this.width, this.position.x, this.position.y, this.position.z, this.relativePosition.x, this.relativePosition.y, this.relativePosition.z, this.transformPosition.x, this.transformPosition.y, this.transformPosition.z));
            Log.MessageUnity(String.Format("label height:{0} width:{1} relposition x-y-z:{2}-{3}-{4} transposition x-y-z:{5}-{6}-{7}", lbl_Gurkenplayer.height, lbl_Gurkenplayer.width, lbl_Gurkenplayer.relativePosition.x, lbl_Gurkenplayer.relativePosition.y, lbl_Gurkenplayer.relativePosition.z, lbl_Gurkenplayer.transformPosition.x, lbl_Gurkenplayer.transformPosition.y, lbl_Gurkenplayer.transformPosition.z));
        }
    }
}
//Info https://media.readthedocs.org/pdf/skylines-modding-docs/master/skylines-modding-docs.pdf
// http://skylines-modding-docs.readthedocs.org/en/latest/modding/Development/How-to-Use-ColossalFramework.UI.html