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
        private static bool isDebugging = true;
        /// <summary>
        /// Indicates the current debug mode. The Log class uses this for example.
        /// </summary>
        public static bool IsDebugging
        {
            get { return isDebugging; }
            set { isDebugging = value; }
        }
            
        /// <summary>
        /// Writes a message to the debug panel if debugging is enabled.
        /// </summary>
        /// <param name="s">Message to log.</param>
        public static void Message(string s)
        {
            if (IsDebugging)
            {
                //DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Message, "[MPMOD]:" + s);
                Debug.Log("[MPMOD]:" + s);
            }
        }

        /// <summary>
        /// Writes a warning to the debug panel if debugging is enabled.
        /// </summary>
        /// <param name="s">The warning to log.</param>
        public static void Warning(string s)
        {
            if (IsDebugging)
            {
                //DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Warning, "[MPMOD]:" + s);
                Debug.LogWarning("[MPMOD]:" + s);
            }
        }

        /// <summary>
        /// Writes an error to the debug panel if debugging is enabled.
        /// </summary>
        /// <param name="s">Error to log.</param>
        public static void Error(string s)
        {
            if (IsDebugging)
            {
                //DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Error, "[MPMOD]:" + s);
                Debug.LogError("[MPMOD]:" + s);
            }
        }
    }
}
