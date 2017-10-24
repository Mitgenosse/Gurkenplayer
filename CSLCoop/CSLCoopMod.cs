using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;
using ICities;
using UnityEngine;

namespace CSLCoop
{
    public class CSLCoopMod : IUserMod
    {
        //Necessary properties
        public string Description
        {
            get { return "Multiplayer mod for Cities: Skylines."; }
        }

        public string Name
        {
            get { return "CSLCoop"; }
        }
    }
}