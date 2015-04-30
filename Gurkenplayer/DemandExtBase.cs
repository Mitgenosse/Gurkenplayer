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
        static int _commercialDemand; //Multiplayer commercial demand
        static int _residentalDemand; //MP residental demand
        static int _workplaceDemand; //MP workplace demand
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
        /// <summary>
        /// Invoked when the game calculates commercial demand. Value between 0 and 100.
        /// </summary>
        /// <param name="originalDemand">Demand calculated by the game.</param>
        /// <returns>Modified demand.</returns>
        public override int OnCalculateCommercialDemand(int originalDemand)
        { 
            if (MPManager.Instance.MPRole == MPRoleType.Server) //Update all
            {
                MPManager.Instance.MPServer.SendDemandInformationUpdateToAll();
                return _commercialDemand;
            }
            else if (MPManager.Instance.MPRole == MPRoleType.Client)
            {
                MPManager.Instance.MPClient.SendDemandInformationUpdateToServer();
                return _commercialDemand;
            }
            return originalDemand;
        }

        /// <summary>
        /// Invoked when the game calculates residental demand. Value between 0 and 100.
        /// </summary>
        /// <param name="originalDemand">Demand calculated by the game.</param>
        /// <returns>Modified demand.</returns>
        public override int OnCalculateResidentialDemand(int originalDemand)
        {
            if (MPManager.Instance.MPRole == MPRoleType.Server)
            {
                return _residentalDemand;
            }
            else if (MPManager.Instance.MPRole == MPRoleType.Client)
            {
                return _residentalDemand;
            }
            return originalDemand;
        }

        /// <summary>
        /// Invoked when the game calculates workplace demand. Value between 0 and 100.
        /// </summary>
        /// <param name="originalDemand">Demand calculated by the game.</param>
        /// <returns>Modified demand.</returns>
        public override int OnCalculateWorkplaceDemand(int originalDemand)
        {
            if (MPManager.Instance.MPRole == MPRoleType.Server)
            {
                return _workplaceDemand;
            }
            else if (MPManager.Instance.MPRole == MPRoleType.Client)
            {
                return _workplaceDemand;
            }
            return originalDemand;
        }
    }
}
