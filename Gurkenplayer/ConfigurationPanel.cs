using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICities;
using UnityEngine;
using ColossalFramework;

namespace Gurkenplayer
{
    class ConfigurationPanel : UIPanel //Testing
    {
        public override void Start()
        {
            //Configures this window
            this.backgroundSprite = "GenericPanel";
            this.color = new Color32(255, 0, 0, 100);
            this.width = 300;
            this.height = 500;

            //Configures the child elements
            UILabel testLabel = this.AddUIComponent<UILabel>();
            testLabel.text = "Gurkenplayer";
            testLabel.eventClick += FooBarClickHandler;
        }
    }
}
//Info https://media.readthedocs.org/pdf/skylines-modding-docs/master/skylines-modding-docs.pdf