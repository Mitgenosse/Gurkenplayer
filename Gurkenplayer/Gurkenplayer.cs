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
        private static MultiplayerRole mpRole = MultiplayerRole.None;

        public static MultiplayerRole MPRole
        {
            get { return GurkenplayerMod.mpRole; }
            set 
            {
                if (value == MultiplayerRole.None)
                {
                    if (mpRole != MultiplayerRole.None)
                        mpRole = MultiplayerRole.None;
                }
                else if (value == MultiplayerRole.Resetting)
                {
                    if (mpRole == MultiplayerRole.Server)
                        Server.Instance.Dispose();
                    else if (mpRole == MultiplayerRole.Client)
                        Client.Instance.Dispose();

                    //MPRole is going to be set to None in the Dispose() methods of each class.
                }
                else if (value == MultiplayerRole.Server)
                {
                    if (mpRole == MultiplayerRole.Client)
                        Client.Instance.Dispose();
                    Log.Message("!!!!!!!!!!!!!!!!!!Client zu Server");
                    mpRole = value;
                }
                else if (value == MultiplayerRole.Client)
                {
                    if (mpRole == MultiplayerRole.Server)
                        Server.Instance.Dispose();
                    Log.Message("!!!!!!!!!!!!!!!!!!Server zu Client");
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
