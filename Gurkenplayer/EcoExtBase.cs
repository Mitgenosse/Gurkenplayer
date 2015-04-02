using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using ICities;
using UnityEngine;
using ColossalFramework;
using System.Threading;

namespace Gurkenplayer
{
    public class EcoExtBase : EconomyExtensionBase
    {
        //Fields
        static long _currentMoneyAmount; //Multiplayer current money amount.
        static long _internalMoneyAmount; //MP internal money amount. (The value which the game uses for calculations)
        private bool isEcoUpdateThreadStarted = false;

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
        public bool IsEcoUpdateThreadStarted
        {
            get { return isEcoUpdateThreadStarted; }
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
            if (!IsEcoUpdateThreadStarted)
            {
                ThreadStart ts = new ThreadStart(SynchronizeMoneyAmount);
                Thread thread = new Thread(ts);
                thread.Start();
                isEcoUpdateThreadStarted = !isEcoUpdateThreadStarted;
            }

            if (AreaExtBase.IsNewTileAvailableToUnlock) //Not a beautiful way to solve the problem. Maybe I can take care of it later.
            {       // Check if a new tile has to be unlocked.
                managers.areas.UnlockArea((int)AreaExtBase._XCoordinate, (int)AreaExtBase._ZCoordinate, true);
                AreaExtBase.SetIsNewTileAvailableToUnlockFalse();
            }

            Log.Warning("internalMoneyAmount: " + internalMoneyAmount + " abc: " + economyManager.currentMoneyAmount);
            _internalMoneyAmount = internalMoneyAmount;
            if (GurkenplayerMod.MPRole == MultiplayerRole.Server)
            {
                Server.Instance.SendEconomyInformationUpdateToAll();
                return _internalMoneyAmount;
            }
            if (GurkenplayerMod.MPRole == MultiplayerRole.Client)
            {
                Client.Instance.SendEconomyInformationUpdateToServer();
                return _internalMoneyAmount;
                
            }
            Log.Warning("internalMoneyAmount: " + internalMoneyAmount + " abc: " + economyManager.currentMoneyAmount);
            return internalMoneyAmount; //If user is not server or client, he should not be connected. Return original value
        }

        /// <summary>
        /// Started in separate thread for synchronization purposes. Synchronizes every 444ms.
        /// </summary>
        void SynchronizeMoneyAmount()
        {
            if (GurkenplayerMod.MPRole == MultiplayerRole.Server)
            {   //If user is a server and there are clients connected, enter the while loop.
                while (Server.Instance.CanSendMessage)
                {
                    if (!IsEcoUpdateThreadStarted) //If it is not running anymore. Go home.
                        break;
                    Thread.Sleep(444);
                    Server.Instance.SendEconomyInformationUpdateToAll();
                }
            }
            else if (GurkenplayerMod.MPRole == MultiplayerRole.Client)
            {   //If user is a client and is connected to a server, enter the wile loop.
                while (Server.Instance.CanSendMessage)
                {
                    if (!IsEcoUpdateThreadStarted)
                        break;
                    Thread.Sleep(444);
                    Client.Instance.SendEconomyInformationUpdateToServer();
                }
            }
            isEcoUpdateThreadStarted = false;
        }
    }
}
