using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICities;
using UnityEngine;
using ColossalFramework;

namespace Gurkenplayer
{
    public class AreaExtBase : AreasExtensionBase
    {
        //Fields
        static int? mpXCoordinate = null; // null == no area to unlock
        static int? mpZCoordinate = null;

        //Properties
        public static int? MPZCoordinate
        {
            get { return AreaExtBase.mpZCoordinate; }
            set { AreaExtBase.mpZCoordinate = value; }
        }
        public static int? MPXCoordinate
        {
            get { return AreaExtBase.mpXCoordinate; }
            set { AreaExtBase.mpXCoordinate = value; }
        }
        public static bool IsNewTileAvailableToUnlock
        {
            get
            {
                if (MPXCoordinate != null && MPZCoordinate != null) 
                    return true;
                else
                    return false;
            }
        } //If X and Z != null return true
        public static void SetIsNewTileAvailableToUnlockFalse()
        {
            AreaExtBase.MPXCoordinate = null;
            AreaExtBase.MPZCoordinate = null;
        } //Set nullable X and Z to null

        //Methods
        /// <summary>
        /// Invoked when a new area is unlocked. It sends a message to netServer/clients containing the coordinates of the new tile.
        /// </summary>
        /// <param name="x">X coordinate of the new tile.</param>
        /// <param name="z">Z coordinate of the new tile.</param>
        public override void OnUnlockArea(int x, int z)
        {
            if (MPManager.Instance.MPRole == MPRoleType.Server)
            {
                managers.areas.UnlockArea(x, z, true);
                MPManager.Instance.MPServer.SendAreaInformationUpdateToAll(x, z);
                //Send unlocked area to clients
            }
            else if (MPManager.Instance.MPRole == MPRoleType.Client)
            {
                managers.areas.UnlockArea(x, z, true);
                MPManager.Instance.MPClient.SendAreaInformationUpdateToServer(x, z);
                //Send unlocked area to netServer
            }
            managers.areas.UnlockArea(x, z, true);
        }
        //A received tile will be unlocked simutaniously with the EcoExtBase.OnUpdateMoneyAmount(long internalMoneyAmount)
    }
}

//managers.areas.UnlockArea(x, z, true); or areaManager.UnlockArea(x, z, true);