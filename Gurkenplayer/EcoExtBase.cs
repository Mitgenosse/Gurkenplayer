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
        static long _currentMoneyAmount; //Multiplayer current money amount.
        static long _internalMoneyAmount; //MP internal money amount. (The value which the game uses for calculations)
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
        /// <summary>
        /// Invoked once every four seconds with the simulation set to normal speed. 
        /// Triggers every time the Economy Manager updates the current money amount.
        /// </summary>
        /// <param name="internalMoneyAmount">Current money amount calculated by the game.</param>
        /// <returns>The current money amount.</returns>
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
            return internalMoneyAmount; //If user is not server or client, he should not be connected. Return original value
        }
    }
}
