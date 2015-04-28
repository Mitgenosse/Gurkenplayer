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
            if (GurkenplayerMod.IsDebugging)
            {
                DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Message, "[MPMOD]:" + s);
                Debug.Log("[MPMOD]:" + s);
            }
        }
        public static void MessageUnity(string s)
        {
            Debug.Log("[MPMOD]:" + s);
        }

        public static void Error(string s)
        {
            if (GurkenplayerMod.IsDebugging)
            {
                DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Error, "[MPMOD]:" + s);
                Debug.LogError("[MPMOD]:" + s);
            }
        }
        public static void ErrorUnity(string s)
        {
            Debug.Log("[MPMOD]:" + s);
        }

        public static void Warning(string s)
        {
            if (GurkenplayerMod.IsDebugging)
            {
                DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Warning, "[MPMOD]:" + s);
                Debug.LogWarning("[MPMOD]:" + s);
            }
        }
        public static void WarningUnity(string s)
        {
            Debug.Log("[MPMOD]:" + s);
        }
    }
}
