using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurkenplayer
{
    /// <summary>
    /// Represents the type of the message. Is used in the ProcessMessage method in Server.cs and Client.cs.
    /// </summary>
    public enum MPMessageType
    {
        None = 0,
        MoneyUpdate,
        DemandUpdate,
        TileUpdate,
        StreetUpdate,
        TimeUpdate,
        PopulationUpdate
    }
}
