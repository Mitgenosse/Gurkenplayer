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
                {
                    Log.IsDebugging = !Log.IsDebugging;
                    Log.Message((Log.IsDebugging) ? "Log.IsDebugging is true." : "Log.IsDebugging is false");
                }

                if (Input.GetKeyDown(KeyCode.S))
                {
                    MPManager.Instance.Reset();
                }

                if (Input.GetKeyDown(KeyCode.B))
                {
                    if (MPManager.Instance.MPRole == MPRoleType.Client)
                        Log.Message(String.Format(">B>Status: Current MPRoleType: {0}; IsClientConnected: {1}; Client StopMessageProcessThread : {2};", MPManager.Instance.MPRole, MPManager.Instance.MPClient.IsClientConnected, MPManager.Instance.MPClient.StopMessageProcessingThread.Condition));
                    else if (MPManager.Instance.MPRole == MPRoleType.Server)
                        Log.Message(String.Format(">B>Status: Current MPRoleType: {0}; IsServerStarted: {1}; Server StopMessageProcessThread : {2};", MPManager.Instance.MPRole, MPManager.Instance.MPServer.IsServerStarted, MPManager.Instance.MPServer.StopMessageProcessingThread.Condition));
                    else if (MPManager.Instance.MPRole == MPRoleType.Resetting)
                        Log.Message("mpManager.MPRole is Resetting");
                    else
                        Log.Message("No information provided. MPRole is probably set to None.");
                }
            }

            SimulationManager.instance.ForcedSimulationPaused = (MPGlobalValues.IsConfigurationFinished) ? false : true;
        }
    }
}
