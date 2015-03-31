using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using UnityEngine;

namespace Gurkenplayer
{ //Don't mind me
    class Panel : MonoBehaviour
    {
        private UITextField textField = null;
        
        private void Awake()
        {
            textField = UIView.GetAView().AddUIComponent(typeof(UITextField)) as UITextField;
            textField.width = 300;
            textField.height = 45;
            textField.text = "";
            textField.maxLength = 100;
            textField.enabled = true;
            textField.builtinKeyNavigation = true;
            textField.isInteractive = true;
            textField.readOnly = false;
            textField.horizontalAlignment = UIHorizontalAlignment.Center;
            textField.verticalAlignment = UIVerticalAlignment.Middle;
            textField.selectionSprite = "EmptySprite";
            textField.selectionBackgroundColor = new Color32(0, 171, 234, 255);
            textField.normalBgSprite = "TextFieldPanel";
            textField.textColor = new Color32(174, 197, 211, 255);
            textField.disabledTextColor = new Color32(254, 254, 254, 255);
            textField.textScale = 2f;
            textField.opacity = 1;
            textField.color = new Color32(58, 88, 104, 255);
            textField.disabledColor = new Color32(254, 254, 254, 255);
            textField.AlignTo(UIView.Find("FullScreenContainer"), UIAlignAnchor.BottomRight);
            textField.relativePosition -= new Vector3(10, 10);
        }
    }
}
