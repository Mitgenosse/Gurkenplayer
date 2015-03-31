using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using ICities;
using UnityEngine;
using ColossalFramework;

namespace Gurkenplayer
{
    public class EcoExtBase : EconomyExtensionBase
    {
        //Fields
        static long _currentMoneyAmount;
        static long _internalMoneyAmount;
        //Properties
        public static long _CurrentMoneyAmount
        {
            get { return _currentMoneyAmount; }
            set { _currentMoneyAmount = value; }
        }
        public static long _InternalMoneyAmount
        { //The value which the game uses to calculate
            get { return _currentMoneyAmount; }
            set { _currentMoneyAmount = value; }
        }

        //Override
        public override long OnUpdateMoneyAmount(long internalMoneyAmount)
        {
            Log.Warning("internalMoneyAmount: " + internalMoneyAmount + " abc: " + economyManager.currentMoneyAmount);
            _internalMoneyAmount = internalMoneyAmount;
            if (GurkenplayerMod.MPRole == MultiplayerRole.Server)
            {
                Server.Instance.SendEconomyInformationUpdateToAll();
                return _internalMoneyAmount;
            }
            if (GurkenplayerMod.MPRole == MultiplayerRole.Client)
            {
                Client.Instance.SendEconomyInformationToServer();
                return _internalMoneyAmount;
            }
            Log.Warning("internalMoneyAmount: " + internalMoneyAmount + " abc: " + economyManager.currentMoneyAmount);
            return _internalMoneyAmount;
        }
    }
}
