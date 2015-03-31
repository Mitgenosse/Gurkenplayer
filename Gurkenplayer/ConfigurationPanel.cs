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
        public override void Start()
        {
            SimulationManager.instance.ForcedSimulationPaused = true;
            //Configures this window
            this.backgroundSprite = "GenericPanel";
            this.color = new Color32(255, 0, 0, 100);
            this.width = 300;
            this.height = 500;
            this.position = new Vector3(Screen.width / 2, Screen.height / 2);

            //Configures the child elements
            UILabel lbl_Header = this.AddUIComponent<UILabel>();
            lbl_Header.text = "Gurkenplayer";
            lbl_Header.eventClick += lbl_HeaderClickHandler;
            lbl_Header.isEnabled = true;

            UILabel lbl_Username = this.AddUIComponent<UILabel>();
            lbl_Username.name = "lbl_Username";
            lbl_Username.text = "Username:";
            lbl_Username.position = new Vector3(20, 20);
            lbl_Username.width = 100;
            lbl_Username.height = 30;
            lbl_Username.isEnabled = true;

            UITextField txb_Username = this.AddUIComponent<UITextField>();
            txb_Username.name = "txb_Username";
            txb_Username.text = string.Empty;
            txb_Username.position = new Vector3(120, 20);
            txb_Username.width = 100; //hard code
            txb_Username.height = 30;
            txb_Username.isEnabled = true;

            UILabel lbl_ClientHeader = this.AddUIComponent<UILabel>();
            lbl_ClientHeader.name = "lbl_ClientHeader";
            lbl_ClientHeader.text = "Client setup";
            lbl_ClientHeader.position = new Vector3(20, 120);
            lbl_ClientHeader.autoSize = true; //Test
            lbl_ClientHeader.autoHeight = true;
            lbl_ClientHeader.isEnabled = true;

            //And so on
        }

        private void lbl_HeaderClickHandler(UIComponent component, UIMouseEventParameter eventParam)
        {
            Log.Message("lbl_Header: clicked");
        }
    }
}
//Info https://media.readthedocs.org/pdf/skylines-modding-docs/master/skylines-modding-docs.pdf