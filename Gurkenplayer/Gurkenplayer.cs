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
            set { GurkenplayerMod.mpRole = value; }
        }

        //Necessary properties
        public string Description
        {
            get { return "Test multiplayer mod for Cities: Skylines."; }
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
