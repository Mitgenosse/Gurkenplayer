using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Lidgren.Network;
using System.Threading;

using ICities;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.Plugins;

namespace Gurkenplayer
{
    public delegate void ClientEventHandler(object sender, EventArgs e);
    public class MPClient : IDisposable
    {
        //Event stuff
        #region Events and Eventmethods
        public event ClientEventHandler clientConnectedEvent;
        public event ClientEventHandler clientDisconnectedEvent;
        public event ClientEventHandler clientLeftProcessMessageThread;

        //EventMethods
        /// <summary>
        /// Fires when the client is 100% connected.
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnClientConnected(EventArgs e)
        {
            if (clientConnectedEvent != null)
                clientConnectedEvent(this, e);
        }
        /// <summary>
        /// Fires when the client is 100% disconnected.
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnClientDisconnected(EventArgs e)
        {
            if (clientDisconnectedEvent != null)
                clientDisconnectedEvent(this, e);
        }
        /// <summary>
        /// Fires when the ProcessMessage thread is right about to fly into nonexistence.
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnClientLeftProcessMessageThread(EventArgs e)
        {
            if (clientLeftProcessMessageThread != null)
                clientLeftProcessMessageThread(this, e);
        }
        #endregion

        //Fields
        #region Fields
        NetPeerConfiguration config;
        NetClient netClient;
        string appIdentifier = "Gurkenplayer";
        string serverIP = "localhost";
        int serverPort = 4420;
        string serverPassword = "Password";
        static bool isClientConnected = false;
        bool disposed = false;
        string username = "usr";
        MPSharedCondition stopMessageProcessingThread;
        ParameterizedThreadStart pts;
        Thread messageProcessingThread;
        #endregion

        //Properties
        #region props
        /// <summary>
        /// Returns the IP-Address of the netServer.
        /// </summary>
        public string ServerIP
        {
            get { return serverIP; }
            set { serverIP = value; }
        }

        /// <summary>
        /// Returns the used netServer password.
        /// </summary>
        public string ServerPassword
        {
            get { return serverPassword; }
            set { serverPassword = value; }
        }

        /// <summary>
        /// Returns the used netServer port.
        /// </summary>
        public int ServerPort
        {
            get { return serverPort; }
            set { serverPort = value; }
        }

        /// <summary>
        /// Indicates if the netClient is connected to a netServer. Is set to "true" inside the StatusChanged of ProcessMessage.
        /// </summary>
        public bool IsClientConnected
        {
            get 
            {
                if (netClient.ConnectionStatus == NetConnectionStatus.Connected)
                    return true; //Check first if the ConnectionStatus is set to connected
                else if (isClientConnected)
                    return true; //If not check if the bool isClientConnected is set to true
                else
                    return false; //Otherwise it is not connected
            }
            set { MPClient.isClientConnected = value; }
        }

        /// <summary>
        /// Returns the username of the netClient.
        /// </summary>
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        /// <summary>
        /// Returns true when the netClient is initialized and connected to a netServer.
        /// </summary>
        public bool CanSendMessage
        {
            get
            {
                if (IsClientConnected)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Returns the current NetConnectionStatus of the netClient.
        /// </summary>
        public NetConnectionStatus ClientConnectionStatus
        {
            get { return netClient.ConnectionStatus; }
        }

        /// <summary>
        /// Indicates if the message processing thread of the netClient should be stopped gracefully.
        /// </summary>
        public MPSharedCondition StopMessageProcessingThread
        {
            get { return stopMessageProcessingThread; }
            set { stopMessageProcessingThread = value; }
        }
        #endregion

        //Constructor
        /// <summary>
        /// MPClient constructor.
        /// </summary>
        public MPClient()
        {
            StopMessageProcessingThread = new MPSharedCondition(false);
            ResetConfig();
            netClient = new NetClient(config);
            pts = new ParameterizedThreadStart(this.ProcessMessage);
        }
        /// <summary>
        /// MPCLient constructor with MPThreadStopCondition as a parameter.
        /// </summary>
        /// <param name="condition">The condition to stop the ProcessMessageThread.</param>
        public MPClient(MPSharedCondition condition)
        {
            StopMessageProcessingThread = condition ?? new MPSharedCondition(false);
            ResetConfig();
            netClient = new NetClient(config);
            pts = new ParameterizedThreadStart(this.ProcessMessage);
        }

        /// <summary>
        /// Destructor logic.
        /// </summary>
        ~MPClient()
        {
            Dispose();
        }

        //Methods
        /// <summary>
        /// Method with optional parameters which is used to connect to an existing netServer.
        /// Empty arguments take the default value.
        /// </summary>
        /// <param name="ip">The netServer ip to connect to. Default: localhost</param>
        /// <param name="port">The netServer port which is used to connect. Default: 4230</param>
        /// <param name="password">The netServer password which is used to connect. Default: none</param>
        public void ConnectToServer(string ip = "localhost", int port = 4230, string password = "")
        {
            Log.Message("Client connecting to server. if(IsClientConnected) -> Disconnect...(): " + IsClientConnected + ". IsClientConnected: " + IsClientConnected + " Current MPRole: " + MPManager.Instance.MPRole);
            DisconnectFromServer();

            //Throw Exception when ip is not valid
            if (ip != "localhost" && ip.Split('.').Length != 4)
                throw new MPException("Invalid server ip address. Check it please.");

            Log.Warning(String.Format("Client trying to connect to ip:{0} port:{1} password:{2} maxretry:{3} retryinterval:{4} MPRole:{5}", ip, port, password, netClient.Configuration.MaximumHandshakeAttempts, netClient.Configuration.ResendHandshakeInterval, MPManager.Instance.MPRole));

            //Manipulating fields
            ServerIP = ip;
            ServerPort = port;
            ServerPassword = password;

            //Write approval message with password
            NetOutgoingMessage approvalMessage = netClient.CreateMessage();
            approvalMessage.Write(ServerPassword);
            approvalMessage.Write(Username);
            approvalMessage.Write(PluginManager.instance.enabledModCount);
            netClient.Start();
            Log.Message("Client started. Trying to connect.");
            netClient.Connect(ServerIP, ServerPort, approvalMessage);
            Log.Message("after client.Connect. Starting Thread now.");

            IsClientConnected = true;
            StopMessageProcessingThread.Condition = false;

            //Separate thread in which the received messages are handled
            messageProcessingThread = new Thread(pts);
            messageProcessingThread.Start(netClient);

            Log.Message("Client should be connected. Current MPRole: " + MPManager.Instance.MPRole + " MessageProcessingThread alive? " + messageProcessingThread.IsAlive);
        }

        /// <summary>
        /// Disconnects the netClient from the netServer
        /// </summary>
        public void DisconnectFromServer()
        {
            if (!IsClientConnected)
                return; //If netClient is not  connected, return

            Log.Message("Disconnecting from the server. Current MPRole: " + MPManager.Instance.MPRole);

            try
            {
                netClient.Disconnect("Bye Bye Client.");
            }
            catch (Exception ex)
            {
                Log.Error("Exception while disconnecting client. ex: " + ex.ToString());
            }
            finally
            {
                //Reconfiguration
                ResetConfig();
                netClient = new NetClient(config);

                StopMessageProcessingThread.Condition = true;

                Log.Error("Disconnected. Is thread still alive? " + messageProcessingThread.IsAlive);
            }
        }

        /// <summary>
        /// Gets rid of the Client instance.
        /// </summary>
        public void Dispose()
        {
            try
            {
                Log.Message("Disposing client. Current MPRole: " + MPManager.Instance.MPRole);
                DisconnectFromServer();
                
                Log.Message("Client disposed. Current MPRole: " + MPManager.Instance.MPRole);
            }
            catch (Exception ex)
            {
                throw new MPException("Exception in Client.Dispose()", ex);
            }
            finally
            {
                if (!disposed)
                {
                    GC.SuppressFinalize(this);
                    disposed = true;
                }
            }
        }

        private void ResetConfig()
        {
            config = new NetPeerConfiguration(appIdentifier);
            config.MaximumHandshakeAttempts = 1;
            config.ResendHandshakeInterval = 1;
            config.AutoFlushSendQueue = false; 
            // netClient.SendMessage(message, NetDeliveryMethod); is needed for sending
        }

        /// <summary>
        /// ProcessMessage runs in a separate thread and manages the received netServer messages.
        /// </summary>
        /// <param name="obj">object obj represents a NetClient object.</param>
        private void ProcessMessage(object obj)
        {
            try
            {
                Log.Warning("Entering ProcessMessage");
                NetClient client = (NetClient)obj;
                NetIncomingMessage msg;

                MPManager.Instance.IsProcessMessageThreadRunning = true;
                StopMessageProcessingThread.Condition = false;
                while (!MPManager.StopProcessMessageThread.Condition)
                {
                    while ((msg = client.ReadMessage()) != null)
                    {
                        switch (msg.MessageType)
                        {
                            // Debug
                            #region NetIncomingMessageType Debug
                            case NetIncomingMessageType.VerboseDebugMessage: //Debug
                            case NetIncomingMessageType.DebugMessage: //Debug
                            case NetIncomingMessageType.WarningMessage: //Debug
                            case NetIncomingMessageType.ErrorMessage: //Debug
                                Log.Warning("DebugMessage: " + msg.ReadString());
                                break;
                            #endregion

                            // StatusChanged -> Client connected or disconnected
                            #region NetIncomingMessageType.StatusChanged
                            case NetIncomingMessageType.StatusChanged:
                                NetConnectionStatus state = (NetConnectionStatus)msg.ReadByte();
                                Log.Warning("ProcessMessage entry state: " + state);
                                if (state == NetConnectionStatus.Connected)
                                {
                                    IsClientConnected = true;
                                    OnClientConnected(EventArgs.Empty);
                                    Log.Message("You connected. Client IP: " + msg.SenderEndPoint);
                                }
                                else if (state == NetConnectionStatus.Disconnected || state == NetConnectionStatus.Disconnecting || state == NetConnectionStatus.None)
                                {
                                    IsClientConnected = false;
                                    StopMessageProcessingThread.Condition = true;
                                    OnClientDisconnected(EventArgs.Empty);
                                    Log.Message("You disconnected. Client IP: " + msg.SenderEndPoint);
                                }
                                break;
                            #endregion

                            // Client received data to synchronize
                            #region NetIncomingMessageType.Data
                            case NetIncomingMessageType.Data:
                                int type = msg.ReadInt32();
                                ProgressData((MPMessageType)type, msg); //Test
                                break;
                            #endregion

                            // Not important for the MPClient
                            #region NetIncomingMessageType.ConnectionApproval
                            case NetIncomingMessageType.ConnectionApproval:
                                break;
                            #endregion

                            default:
                                Log.Warning(String.Format("Client ProcessMessage: Unhandled type: {0}", msg.MessageType));
                                break;
                        }
                    }
                }
            }
            catch (NetException ex)
            {
                throw new MPException("NetException (Lidgren) in Client.ProcessMessage. Message: " + ex.ToString(), ex);
            }
            catch (Exception ex)
            {
                throw new MPException("Exception in Client.ProcessMessage(). Message: " + ex.ToString(), ex);
            }
            finally
            {
                IsClientConnected = false;
                StopMessageProcessingThread.Condition = false;
                OnClientLeftProcessMessageThread(EventArgs.Empty);
            }
        }
        
        /// <summary>
        /// Method to process the received information.
        /// </summary>
        /// <param name="type">Type of the message. Indicates what the message's contents are.</param>
        /// <param name="msg">The message to process.</param>
        private void ProgressData(MPMessageType msgType, NetIncomingMessage msg)
        {
            switch (msgType)
            {
                case MPMessageType.MoneyUpdate: // Receiving money
                    Log.Message("Client received " + msgType);
                    long t = msg.ReadInt64();
                    EcoExtBase.MPInternalMoneyAmount = t;
                    break;
                case MPMessageType.DemandUpdate: // Receiving demand
                    Log.Message("Client received " + msgType);
                    DemandExtBase.MPCommercialDemand = msg.ReadInt32();
                    DemandExtBase.MPResidentalDemand = msg.ReadInt32();
                    DemandExtBase.MPWorkplaceDemand = msg.ReadInt32();
                    break;
                case MPMessageType.TileUpdate: // Receiving tile coordinates to unlock
                    Log.Message("Client received " + msgType);
                    AreaExtBase.MPXCoordinate = msg.ReadInt32();
                    AreaExtBase.MPZCoordinate = msg.ReadInt32();
                    //INFO: The unlock process is activated simutaniously with the
                    //EcoExtBase.OnUpdateMoneyAmount(long internalMoneyAmount).
                    //Maybe I find a direct way to unlock a tile within AreaExtBase
                    break;
                case MPMessageType.SimulationUpdate: // Receiving update on simulation information
                    Log.Message("Client received " + msgType);
                    SimulationManager.instance.SelectedSimulationSpeed = msg.ReadInt32();
                    SimulationManager.instance.SimulationPaused = msg.ReadBoolean();
                    break;
                default: //Unhandled ID (MPMessageType)
                    Log.Warning(String.Format("Client ProgressData: Unhandled ID/type: {0}/{1} ", (int)msgType, msgType));
                    break;
            }
        }

        /// <summary>
        /// Send the EconomyInformation of the MPClient to the MPServer to synchronize.
        /// </summary>
        public void SendEconomyInformationUpdateToServer()
        {
            if (CanSendMessage)
            {
                NetOutgoingMessage msg = netClient.CreateMessage();
                msg.Write((int)MPMessageType.MoneyUpdate);
                msg.Write(EcoExtBase.MPCashChangeAmount);
                netClient.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                netClient.FlushSendQueue();
                EcoExtBase.MPCashChangeAmount = 0;
            }
        }

        /// <summary>
        /// Sends the DemandInformation of the MPClient to the MPServer to synchronize.
        /// </summary>
        public void SendDemandInformationUpdateToServer()
        {
            if (CanSendMessage)
            {
                NetOutgoingMessage msg = netClient.CreateMessage();
                msg.Write((int)MPMessageType.DemandUpdate);
                msg.Write(DemandExtBase.MPCommercialDemand);
                msg.Write(DemandExtBase.MPResidentalDemand);
                msg.Write(DemandExtBase.MPWorkplaceDemand);
                netClient.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                netClient.FlushSendQueue();
            }
        }

        /// <summary>
        /// Sends a message to the netServer indicating which tile shall be unlocked.
        /// </summary>
        /// <param name="x">X coordinate of the tile.</param>
        /// <param name="z">Z coordinate of the tile.</param>
        public void SendAreaInformationUpdateToServer(int x, int z)
        {
            if (CanSendMessage)
            {
                NetOutgoingMessage msg = netClient.CreateMessage();
                msg.Write((int)MPMessageType.TileUpdate);
                msg.Write(x);
                msg.Write(z);
                netClient.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                netClient.FlushSendQueue();
            }
        }

        /// <summary>
        /// Sends a message to the MPServer containing the current simulation information.
        /// </summary>
        public void SendSimulationInformationUpdateToServer()
        {
            if (CanSendMessage)
            {
                NetOutgoingMessage msg = netClient.CreateMessage();
                msg.Write((int)MPMessageType.SimulationUpdate);
                msg.Write(SimulationManager.instance.SelectedSimulationSpeed);
                msg.Write(SimulationManager.instance.SimulationPaused);
                netClient.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                netClient.FlushSendQueue();
            }
        }
    }
}
