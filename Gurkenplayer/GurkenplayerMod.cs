using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;
using ICities;
using UnityEngine;

namespace Gurkenplayer
{
    public class GurkenplayerMod : IUserMod
    {
        private static MPRoleType mpRole = MPRoleType.None;

        public static MPRoleType MPRole
        {
            get { return GurkenplayerMod.mpRole; }
            set 
            {
                if (value == MPRoleType.None)
                {
                    mpRole = MPRoleType.None;
                }
                else if (value == MPRoleType.Resetting)
                {   
                    //There are 2 ways of resetting an Instance of the server or client.
                    //1: Client/Server.Instance.Dispose(); AND GurkenplayerMod.MPRole = MPRoleType.Resetting;
                    if (mpRole == MPRoleType.Server)
                        Server.Instance.Dispose();
                    else if (mpRole == MPRoleType.Client)
                        Client.Instance.Dispose();

                    //MPRole is going to be set to None in the Dispose() methods of each class.
                }
                else if (value == MPRoleType.Server)
                {
                    if (mpRole == MPRoleType.Client)
                        Client.Instance.Dispose();
                    Log.Message("MPRole Property: mpRole >" + mpRole + "< and setting value >" + value + "<");
                    mpRole = value;
                }
                else if (value == MPRoleType.Client)
                {
                    if (mpRole == MPRoleType.Server)
                        Server.Instance.Dispose();
                    Log.Message("MPRole Property: mpRole >" + mpRole + "< and setting value >" + value + "<");
                    mpRole = value;
                }
            }
        }

        //Necessary properties
        public string Description
        {
            get { return "Multiplayer mod for Cities: Skylines."; }
        }

        public string Name
        {
            get { return "Gurkenplayer"; }
        }
    }
    
}
/* if (GurkenplayerMod.MPRole == MultiplayerRole.Server)
   {

   }
   else if (GurkenplayerMod.MPRole == MultiplayerRole.Client)
   {

   }
 */
