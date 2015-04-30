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
        static int? _xCoordinate = null; // null == no area to unlock
        static int? _zCoordinate = null;

        //Properties
        public static int? _ZCoordinate
        {
            get { return AreaExtBase._zCoordinate; }
            set { AreaExtBase._zCoordinate = value; }
        }
        public static int? _XCoordinate
        {
            get { return AreaExtBase._xCoordinate; }
            set { AreaExtBase._xCoordinate = value; }
        }
        public static bool IsNewTileAvailableToUnlock
        {
            get
            {
                if (_xCoordinate != null && _zCoordinate != null) 
                    return true;
                else
                    return false;
            }
        } //If X and Z != null return true
        public static void SetIsNewTileAvailableToUnlockFalse()
        {
            AreaExtBase._XCoordinate = null;
            AreaExtBase._ZCoordinate = null;
        } //Set nullable X and Z to null

        //Methods
        /// <summary>
        /// Invoked when a new area is unlocked. It sends a message to server/clients containing the coordinates of the new tile.
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
                //Send unlocked area to server
            }
            managers.areas.UnlockArea(x, z, true);
        }

        //A received tile will be unlocked once every 4 seconds simutaniously with the EcoExtBase.OnUpdateMoneyAmount(long internalMoneyAmount)
    }
}

//managers.areas.UnlockArea(x, z, true); or areaManager.UnlockArea(x, z, true);