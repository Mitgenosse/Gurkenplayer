using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICities;
using UnityEngine;
using ColossalFramework;

namespace Gurkenplayer
{
    public class DemandExtBase : DemandExtensionBase
    {
        //Fields
        static int _commercialDemand;
        static int _residentalDemand;
        static int _workplaceDemand;
        //Properties
        public static int _CommercialDemand
        {
            get { return _commercialDemand; }
            set { _commercialDemand = value; }
        }
        public static int _ResidentalDemand
        {
            get { return _residentalDemand; }
            set { _residentalDemand = value; }
        }
        public static int _WorkplaceDemand
        {
            get { return _workplaceDemand; }
            set { _workplaceDemand = value; }
        }

        //Methods
        public override int OnCalculateCommercialDemand(int originalDemand)
        {
            if (GurkenplayerMod.MPRole == MultiplayerRole.Server)
            {
                Server.Instance.SendDemandInformationUpdateToAll();
                return _CommercialDemand;
            }
            else if (GurkenplayerMod.MPRole == MultiplayerRole.Client)
            {
                Client.Instance.SendDemandInformationUpdateToServer();
                return _CommercialDemand;
            }
            return 1;
        }
    }
}
