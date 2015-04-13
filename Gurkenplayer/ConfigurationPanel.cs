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
        private static ConfigurationPanel panel;

        public static ConfigurationPanel Instance
        {
            get
            {
                if (panel == null)
                    panel = new ConfigurationPanel();

                return panel;
            }
        }
        private ConfigurationPanel()
        {

        }

        public override void Start()
        {
            Log.MessageUnity("Configuration panel start.");
            SimulationManager.instance.ForcedSimulationPaused = true;

            try
            {
                //Configures this window
                this.backgroundSprite = "GenericPanel";
                this.color = new Color32(255, 0, 0, 100);
                this.width = 300;
                this.height = 500;
                //this.position = new Vector3(Screen.width / 2, Screen.height / 2);
            }
            catch (Exception ex)
            {
                Log.ErrorUnity("ABC" + ex.ToString());
            }
        }
    }
}
//Info https://media.readthedocs.org/pdf/skylines-modding-docs/master/skylines-modding-docs.pdf
// http://skylines-modding-docs.readthedocs.org/en/latest/modding/Development/How-to-Use-ColossalFramework.UI.html