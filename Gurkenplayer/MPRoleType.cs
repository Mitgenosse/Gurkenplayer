using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurkenplayer
{
    /// <summary>
    /// MultiplayerRole indicates the current role of the player.
    /// </summary>
    public enum MPRoleType
    {
        None = 0,
        Server,
        Client,
        Resetting //Resetting is used to dispose the current instance. 
    }
}