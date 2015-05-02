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
        static long mpInternalMoneyAmount;//MP global internal money amount
        static long mpLastInternalMoneyAmount;
        static long mpCashChangeAmount;
        bool firstRun = true;

        //Properties
        public static long MPInternalMoneyAmount
        {
            get { return EcoExtBase.mpInternalMoneyAmount; }
            set { EcoExtBase.mpInternalMoneyAmount = value; }
        }
        public static long MPLastInternalMoneyAmount
        {
            get { return EcoExtBase.mpLastInternalMoneyAmount; }
            set { EcoExtBase.mpLastInternalMoneyAmount = value; }
        }
        public static long MPCashChangeAmount
        {
            get { return EcoExtBase.mpCashChangeAmount; }
            set { EcoExtBase.mpCashChangeAmount = value; }
        }
        public bool FirstRun
        {
            get { return firstRun; }
            set { firstRun = value; }
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
            if (AreaExtBase.IsNewTileAvailableToUnlock) //Not a beautiful way to solve the problem. Maybe I can take care of it later.
            {       // Check if a new tile has to be unlocked.
                managers.areas.UnlockArea((int)AreaExtBase._XCoordinate, (int)AreaExtBase._ZCoordinate, true);
                AreaExtBase.SetIsNewTileAvailableToUnlockFalse();
            }
            if (FirstRun)
            {
                MPInternalMoneyAmount = internalMoneyAmount;
                MPCashChangeAmount = 0;
            }
            //MPLastInternalMoneyAmount = MPInternalMoneyAmount;  //Not important atm

            if (MPManager.Instance.MPRole == MPRoleType.Server)
            {
                //Starting thread to inform the clients every 250ms about the new MPInternalMoneyAmount
                if (FirstRun)
                {
                    Thread thread = new Thread(ServerEconomyInformationUpdateThread);
                    thread.Start();
                    FirstRun = false;
                }
                //MPInternalMoneyAmount = internalMoneyAmount; //Not important atm
                MPInternalMoneyAmount = MPInternalMoneyAmount - MPCashChangeAmount;

                EcoExtBase.MPCashChangeAmount = 0; //Reset the MPCashChangeAmount after updating the MPInternalMoneyAmount
                return MPInternalMoneyAmount; //If user is not netServer or netClient, he should not be connected. Return original value
            }

            if (MPManager.Instance.MPRole == MPRoleType.Client)
            {
                FirstRun = false;
                //The client just sends his expenses to the server and receives the MPInternalMoneyAmount update.
                MPManager.Instance.MPClient.SendEconomyInformationUpdateToServer(); 
                return MPInternalMoneyAmount; //If user is not netServer or netClient, he should not be connected. Return original value
            }

            //If the user is not a server or a client, just go on like normal.
            return internalMoneyAmount;
        }

        /// <summary>
        /// Fires when for example a building cost is withdrawed.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="amount"></param>
        /// <param name="service"></param>
        /// <param name="subService"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public override int OnFetchResource(EconomyResource resource, int amount, Service service, SubService subService, Level level)
        {
            MPCashChangeAmount += base.OnFetchResource(resource, amount, service, subService, level);
            return base.OnFetchResource(resource, amount, service, subService, level);
        }

        /// <summary>
        /// Sends the current MPInternalMoneyAmount to all clients every 250 ms.
        /// </summary>
        public static void ServerEconomyInformationUpdateThread()
        {
            Log.Warning("Entering ServerEconomyInformationUpdateThread.");
            while (!MPManager.StopProcessMessageThread.Condition)
            {
                Thread.Sleep(250);
                MPManager.Instance.MPServer.SendEconomyInformationUpdateToAll();
            }
            Log.Warning("Leaving ServerEconomyInformationUpdateThread");
        }
    }
}
