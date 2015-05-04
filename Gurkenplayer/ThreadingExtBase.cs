using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gurkenplayer
{

    public class ThreadingExtBase : ThreadingExtensionBase
    {
        // Fields
        bool mpLastSimulationPausedState = false;
        int mpLastSelectedSimulationSpeedState = 0;

        // Properties
        public bool MPLastSimulationPausedState
        {
            get { return mpLastSimulationPausedState; }
            set { mpLastSimulationPausedState = value; }
        }
        public int MPLastSelectedSimulationSpeedState
        {
            get { return mpLastSelectedSimulationSpeedState; }
            set { mpLastSelectedSimulationSpeedState = value; }
        }

        System.Random rdm = new System.Random((int)1125463589);
        // Methods
        /// <summary>
        /// Called once every frame.
        /// </summary>
        /// <param name="realTimeDelta">The time between the last frame.</param>
        /// <param name="simulationTimeDelta">simulationTimeDelta is smoothly interpolated to be used from main thread. On normal speed it is roughly same as realTimeDelta.</param>
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.D))
                {   // Enable and disable debug
                    Log.IsDebugging = !Log.IsDebugging;
                    Log.Message((Log.IsDebugging) ? "Log.IsDebugging is true." : "Log.IsDebugging is false");
                }

                if (Input.GetKeyDown(KeyCode.C))
                {
                    UIComponent cpanel = GameObject.Find("MPConfigurationPanel").GetComponent<UIComponent>();
                    if (cpanel != null)
                    {
                        if (cpanel.isVisible)
                            cpanel.isVisible = false;
                        else
                            cpanel.isVisible = true;
                    }
                }

                if (Input.GetKeyDown(KeyCode.S))
                {   // Reset MPManager
                    MPManager.Instance.Reset();
                }

                if (Input.GetKeyDown(KeyCode.B))
                {   // Receive status of current sesstion
                    if (MPManager.Instance.MPRole == MPRoleType.Client)
                        Log.Message(String.Format(">B>Status: Current MPRoleType: {0}; IsClientConnected: {1}; Client StopMessageProcessThread : {2};", MPManager.Instance.MPRole, MPManager.Instance.MPClient.IsClientConnected, MPManager.Instance.MPClient.StopMessageProcessingThread.Condition));
                    else if (MPManager.Instance.MPRole == MPRoleType.Server)
                        Log.Message(String.Format(">B>Status: Current MPRoleType: {0}; IsServerStarted: {1}; Server StopMessageProcessThread : {2};", MPManager.Instance.MPRole, MPManager.Instance.MPServer.IsServerStarted, MPManager.Instance.MPServer.StopMessageProcessingThread.Condition));
                    else if (MPManager.Instance.MPRole == MPRoleType.Resetting)
                        Log.Message("mpManager.MPRole is Resetting");
                    else
                        Log.Message("No information provided. MPRole is probably set to None.");
                }

                if (Input.GetKeyDown(KeyCode.R))
                {   // Place road random
                    Vector3 startPos = new Vector3(rdm.Next(1000), 0, rdm.Next(1000));
                    Vector3 endPos = new Vector3(rdm.Next(1000), 0, rdm.Next(1000));
                    try
                    {
                        RoadBuildTest.BuildRoad(startPos, endPos, 38);
                    }
                    catch (Exception ex)
                    {
                        Log.Message(ex.ToString());
                    }
                }
            }
            SimulationManager.instance.ForcedSimulationPaused = (MPGlobalValues.IsConfigurationFinished) ? false : true;
        }

        /// <summary>
        /// Updates every frame, even when the simulation is paused.
        /// </summary>
        public override void OnAfterSimulationTick()
        {
            if (MPLastSimulationPausedState != SimulationManager.instance.SimulationPaused || MPLastSelectedSimulationSpeedState != SimulationManager.instance.SelectedSimulationSpeed)
            {   // If the simulationPaused state changed since the last tick, inform the others.
                if (MPManager.Instance.MPRole == MPRoleType.Server)
                {
                    MPManager.Instance.MPServer.SendSimulationInformationUpdateToAll();
                }
                else if (MPManager.Instance.MPRole == MPRoleType.Client)
                {
                    MPManager.Instance.MPClient.SendSimulationInformationUpdateToServer();
                }
            }
            MPLastSimulationPausedState = SimulationManager.instance.SimulationPaused;
            MPLastSelectedSimulationSpeedState = SimulationManager.instance.SelectedSimulationSpeed;
        }
    }
}
