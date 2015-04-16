using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//using System.Threading.Tasks;

namespace Gurkenplayer
{
    public static class Log
    {
        public static void Message(string s)
        {
            DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Message, s);
            Debug.Log(s);
        }
        public static void MessageUnity(string s)
        {
            Debug.Log(s);
        }

        public static void Error(string s)
        {
            DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Error, s);
            Debug.Log(s);
        }
        public static void ErrorUnity(string s)
        {
            Debug.Log(s);
        }

        public static void Warning(string s)
        {
            DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Warning, s);
            Debug.Log(s);
        }
        public static void WarningUnity(string s)
        {
            Debug.Log(s);
        }
    }
}
