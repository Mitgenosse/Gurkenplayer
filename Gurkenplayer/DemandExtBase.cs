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
        static int mpResidentalDemand = 100; //MP residental demand
        static int mpCommercialDemand = 0; //Multiplayer commercial demand
        static int mpWorkPlaceDemand = 0; //MP workplace demand

        //Properties
        public static int MPCommercialDemand
        {
            get { return mpCommercialDemand; }
            set { mpCommercialDemand = value; }
        }
        public static int MPResidentalDemand
        {
            get { return mpResidentalDemand; }
            set { mpResidentalDemand = value; }
        }
        public static int MPWorkplaceDemand
        {
            get { return mpWorkPlaceDemand; }
            set { mpWorkPlaceDemand = value; }
        }

        //Methods
        /// <summary>
        /// Invoked when the game calculates commercial demand. Value between 0 and 100.
        /// </summary>
        /// <param name="originalDemand">Demand calculated by the game.</param>
        /// <returns>Modified demand.</returns>
        public override int OnCalculateCommercialDemand(int originalDemand)
        {
            if (MPManager.Instance.MPRole == MPRoleType.Server) // Update all clients
            {   // Only the server should send the demand information? Test
                MPCommercialDemand = base.OnCalculateWorkplaceDemand(originalDemand);
                MPManager.Instance.MPServer.SendDemandInformationUpdateToAll();
                MPManager.Instance.MPServer.SendCitizenInformationUpdateToAll();
                return MPCommercialDemand;
            }
            else if (MPManager.Instance.MPRole == MPRoleType.Client)
            {
                // MPManager.Instance.MPClient.SendDemandInformationUpdateToServer();
                return MPCommercialDemand;
            }
            return MPCommercialDemand;
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
                MPResidentalDemand = base.OnCalculateWorkplaceDemand(originalDemand);
                return MPResidentalDemand;
            }
            else if (MPManager.Instance.MPRole == MPRoleType.Client)
            {
                return MPResidentalDemand;
            }
            return MPResidentalDemand;
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
                MPWorkplaceDemand = base.OnCalculateWorkplaceDemand(originalDemand);
                return MPWorkplaceDemand;
            }
            else if (MPManager.Instance.MPRole == MPRoleType.Client)
            {
                return MPWorkplaceDemand;
            }
            return MPWorkplaceDemand;
        }
    }
}
