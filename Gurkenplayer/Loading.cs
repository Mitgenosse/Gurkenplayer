using ColossalFramework.UI;
using ICities;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Gurkenplayer
{
    public class Loading : LoadingExtensionBase
    {
        LoadMode loadMode;
        UIComponent uiComponent;

        /// <summary>
        /// Thread: Main
        /// Invoked when the extension initializes
        /// </summary>
        /// <param name="loading"></param>
        public override void OnCreated(ILoading loading) //Nachdem man start gedrückt hat
        {
        }

        /// <summary>
        /// Thread: Main
        /// Invoked when a level has completed the loading process.
        /// </summary>
        /// <param name="mode">Defines what kind of level was just loaded.</param>*
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode != LoadMode.NewGame)
                return;
            
            loadMode = mode;

            try
            {
                UIView v = UIView.GetAView();

                uiComponent = (UIComponent)v.AddUIComponent(typeof(ConfigurationPanel));
            }
            catch (Exception ex)
            {
                Log.Error("Adding UI Error: " + ex.ToString());
            }
        }

        /// <summary>
        /// Thread: Main
        /// Invoked when the level is unloading (typically when going back to the main menu
        /// or prior to loading a new level)
        /// </summary>
        public override void OnLevelUnloading()
        {
            if (MPManager.Instance != null)
                MPManager.Instance.SetMPRole(MPRoleType.Resetting);

            if (uiComponent != null)
                UnityEngine.Object.Destroy(uiComponent);
        }

        /// <summary>
        /// Thread: Main
        /// Invoked when the extension deinitializes.
        /// </summary>
        public override void OnReleased()
        {
            if (MPManager.Instance != null)
                MPManager.Instance.SetMPRole(MPRoleType.Resetting);

            if (uiComponent != null)
                UnityEngine.Object.Destroy(uiComponent);
        }
    }
}