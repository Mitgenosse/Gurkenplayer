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
            try
            {
                //Log.Message("creating server instance");
                //Server server = Server.Instance;
                //server.StartServer();
                //Log.Message("created server instance");
                //if(Server.Instance != null)
                //    Log.Message("server successfully created");

                //Log.Message("creating client instance");
                //Client client = Client.Instance;
                //client.ConnectToServer();
                //Log.Message("Instance created");

                ////Starting a thread to disconnect client and close server in 15-16 seconds after launch
                //ThreadStart ts = new ThreadStart(disconnectAll);
                //Thread thread = new Thread(ts);
                //thread.Start();
            }
            catch (Exception ex)
            {
                Log.Error("Loading.cs: " + ex.ToString());
            }
        }

        /// <summary>
        /// Method in second thread to disconnect and stop server and client.
        /// </summary>
        private void disconnectAll()
        {
            Thread.Sleep(15000);
            Client.Instance.DisconnectFromServer();
            Thread.Sleep(1000);
            Server.Instance.StopServer();
        }

        /// <summary>
        /// Thread: Main
        /// Invoked when the level is unloading (typically when going back to the main menu
        /// or prior to loading a new level)
        /// </summary>
        public override void OnLevelUnloading()
        {
            if (GurkenplayerMod.MPRole == MultiplayerRole.Server)
            {
                Server.Instance.StopServer();
            }
            else if (GurkenplayerMod.MPRole == MultiplayerRole.Client)
            {
                Client.Instance.DisconnectFromServer();
            }
            GurkenplayerMod.MPRole = MultiplayerRole.None;
        }

        /// <summary>
        /// Thread: Main
        /// Invoked when the extension deinitializes.
        /// </summary>
        public override void OnReleased()
        {
            if (uiComponent != null)
                UnityEngine.Object.Destroy(uiComponent);

            if (GurkenplayerMod.MPRole != MultiplayerRole.None)
            {
                if (GurkenplayerMod.MPRole == MultiplayerRole.Server)
                {
                    Server.Instance.StopServer();
                }
                else if (GurkenplayerMod.MPRole == MultiplayerRole.Client)
                {
                    Client.Instance.DisconnectFromServer();
                }
            }
        }
    }
}