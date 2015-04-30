using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Lidgren.Network;
using System.Threading;
using ICities;

namespace Gurkenplayer
{
    public delegate void ServerEventHandler(object sender, EventArgs e);
    public class MPServer : IDisposable
    {
        //Event stuff
        #region Events and Eventmethods
        public event ServerEventHandler serverStartedEvent;
        public event ServerEventHandler serverStoppedEvent;
        public event ServerEventHandler serverLeftProcessingMessageThread;

        /// <summary>
        /// Fires when the server is 100% started.
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnServerStarted(EventArgs e)
        {
            if (serverStartedEvent != null)
                serverStartedEvent(this, e);
        }
        /// <summary>
        /// Fires when the server is 100% stopped.
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnServerStopped(EventArgs e)
        {
            if (serverStoppedEvent != null)
                serverStoppedEvent(this, e);
        }
        /// <summary>
        /// Fires when the ProcessMessage thread is about to end.
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnServerLeftProcessingMessageThread(EventArgs e)
        {
            if (serverLeftProcessingMessageThread != null)
                serverLeftProcessingMessageThread(this, e);
        }
        #endregion

        //Fields
        #region Fields (and objects <3)
        NetPeerConfiguration config; //Is used to create the server
        NetServer server;
        string appIdentifier = "Gurkenplayer";
        string serverPassword = "Password";
        int serverPort = 4230;
        int serverMaximumPlayerAmount = 1; //1 extra player (coop)
        bool isServerStarted = false;
        string username = "Host";
        bool disposed = false;
        MPSharedCondition stopProcessMessageThread;
        //Message handle thread
        ParameterizedThreadStart pts; //Initializes in the constructor
        Thread messageProcessingThread;
        #endregion

        //Properties
        #region Props
        /// <summary>
        /// Returns the used server password.
        /// </summary>
        public string ServerPassword
        {
            get { return serverPassword; }
            set { serverPassword = value; }
        }

        /// <summary>
        /// Returns the used server port.
        /// </summary>
        public int ServerPort
        {
            get { return serverPort; }
            set { serverPort = value; }
        }

        /// <summary>
        /// Returns the max amount of connections.
        /// </summary>
        public int ServerMaximumPlayerAmount
        {
            get { return serverMaximumPlayerAmount; }
            set { serverMaximumPlayerAmount = value; }
        }

        /// <summary>
        /// Returns true if the server is running.
        /// </summary>
        public MPSharedCondition StopMessageProcessingThread
        {
            get { return stopProcessMessageThread; }
            set { stopProcessMessageThread = value; }
        }

        /// <summary>
        /// Indicates wether the server is started or not.
        /// </summary>
        public bool IsServerStarted
        {
            get { return isServerStarted; }
            set { isServerStarted = value; }
        }

        /// <summary>
        /// Returns the username.
        /// </summary>
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        /// <summary>
        /// Returns true if the server is initialized and a netClient is connected to it.
        /// </summary>
        public bool CanSendMessage
        {
            get
            {
                if (server != null)
                    if (server.ConnectionsCount > 0)
                        return true;

                return false;
            }
        }
        #endregion


        //Constructor
        /// <summary>
        /// Private Server constructor used in the singleton pattern. It initialized the config.
        /// </summary>
        public MPServer(MPSharedCondition condition)
        {
            stopProcessMessageThread = condition ?? new MPSharedCondition(false);
            ResetConfig();
            pts = new ParameterizedThreadStart(this.ProcessMessage);
            messageProcessingThread = new Thread(pts);
        }

        /// <summary>
        /// Destructor logic.
        /// </summary>
        ~MPServer()
        {
            Dispose();
        }

        //Methods
        /// <summary>
        /// Method to start the server with 3 optional parameters. Arguments which are left blank take
        /// the default value.
        /// </summary>
        /// <param name="port">Port of the server. Default: 4230</param>
        /// <param name="password">Password of the server. Default: none</param>
        /// <param name="maximumPlayerAmount">Amount of players. Default: 2</param>
        public void StartServer(int port = 4230, string password = "", int maximumPlayerAmount = 1)
        {
            Log.Message("Starting the server. IsServerStarted? " + IsServerStarted + " -> Instance.Stop(). StopMessageProcess...: " + StopMessageProcessingThread);
            Stop();

            Log.Message("Argument check.");
            if (port < 1000 || port > 65535)
                throw new MPException("I am not going to bind a port under 1000.");

            if (maximumPlayerAmount < 1)
                throw new MPException("You cannot play alone!");
            Log.Message("Argument check finished.");

            //Field configuration
            ServerPort = port;
            ServerPassword = password;
            ServerMaximumPlayerAmount = maximumPlayerAmount;

            //NetPeerConfiguration configuration
            ResetConfig();

            //Initializes the NetServer object with the config and start it
            server = new NetServer(config);
            server.Start();
            IsServerStarted = true;

            //Separate thread in which the received messages are handled
            messageProcessingThread = new Thread(pts);
            messageProcessingThread.Start(server);
            OnServerStarted(new EventArgs());
            Log.Message("Server started successfuly.");
        }

        /// <summary>
        /// Stops the running server gracefully.
        /// </summary>
        public void Stop()
        {
            if (!IsServerStarted) //If server is not started, return
                return;

            try
            {
                Log.Message("Shutting down the server");
                server.Shutdown("Bye bye Server!");
            }
            catch (NetException ex)
            {
                throw new MPException("NetException (Lidgren) in Server.ProcessMessage. Message: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new MPException("Exception in Server.Stop()", ex);
            }
            finally
            {
                ResetConfig();
                StopMessageProcessingThread.Condition = true;
                IsServerStarted = false;
                OnServerStopped(new EventArgs());
                Log.Message("Server shut down");
            }
        }

        /// <summary>
        /// Method to create a new config.
        /// </summary>
        private void ResetConfig()
        {
            config = new NetPeerConfiguration(appIdentifier);
            config.Port = ServerPort;
            config.MaximumConnections = ServerMaximumPlayerAmount;
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.AutoFlushSendQueue = false;
        }

        /// <summary>
        /// Gets rid of the Server instance.
        /// </summary>
        public void Dispose()
        {
            try
            {
                Log.Message("Disposing server. Current MPRole: " + MPManager.Instance.MPRole);

                Stop();
                
                if (!disposed)
                {
                    GC.SuppressFinalize(this);
                    disposed = true;
                }

                Log.Message("Server disposed. Current MPRole: " + MPManager.Instance.MPRole);
            }
            catch (Exception ex)
            {
                throw new MPException("Exception in Server.Dispose()", ex);
            }
        }

        /// <summary>
        /// ProcessMessage runs in a separate thread and manages the incoming messages of the clients.
        /// </summary>
        /// <param name="obj">object obj represents a NetServer object.</param>
        private void ProcessMessage(object obj)
        {
            try
            {
                server = (NetServer)obj;
                NetIncomingMessage msg;
                Log.Message("Server thread started. ProcessMessage(). Current MPRole: " + MPManager.Instance.MPRole);

                StopMessageProcessingThread.Condition = false;
                MPManager.Instance.IsProcessMessageThreadRunning = true;
                while (!StopMessageProcessingThread.Condition)
                { //As long as the server is started
                    while ((msg = server.ReadMessage()) != null)
                    {
                        switch (msg.MessageType)
                        {
                            //Debuggen
                            #region Debug
                            case NetIncomingMessageType.VerboseDebugMessage:
                            case NetIncomingMessageType.DebugMessage:
                            case NetIncomingMessageType.WarningMessage:
                            case NetIncomingMessageType.ErrorMessage:
                                Log.Warning("DebugMessage: " + msg.ReadString());
                                break;
                            #endregion

                            //StatusChanged
                            #region NetIncomingMessageType.StatusChanged
                            case NetIncomingMessageType.StatusChanged:
                                NetConnectionStatus state = (NetConnectionStatus)msg.ReadByte();
                                if (state == NetConnectionStatus.Connected)
                                {
                                    Log.Message("Client connected. Client IP: " + msg.SenderEndPoint);
                                    Log.Error("ConnectionsCount new connected:::" + server.ConnectionsCount.ToString());
                                }
                                else if (state == NetConnectionStatus.Disconnected || state == NetConnectionStatus.Disconnecting)
                                {
                                    Log.Message("Client disconnected. Client IP: " + msg.SenderEndPoint);
                                    Log.Error("ConnectionsCount new disconnect:::" + server.ConnectionsCount.ToString());
                                }
                                break;
                            #endregion

                            //If the message contains data
                            #region NetIncomingMessageType.Data
                            case NetIncomingMessageType.Data:
                                Log.Message("Server Incoming Message Data.");
                                int type = msg.ReadInt32();
                                Log.Message("Type: " + type + "; MPMessageType: " + (MPMessageType)type);
                                ProgressData((MPMessageType)type, msg); //Test
                                break;
                            #endregion

                            //Connectionapproval
                            #region NetIncomingMessageType.ConnectionApproval
                            case NetIncomingMessageType.ConnectionApproval:
                                //Connection logic. Is the user allowed to connect
                                //Receive information to process
                                string sentPassword = msg.ReadString();
                                string sentUsername = msg.ReadString();

                                if (server.ConnectionsCount <= ServerMaximumPlayerAmount)
                                {
                                    Log.Warning("User (" + sentUsername + ") trying to connect. Sent password ->" + sentPassword);
                                    if (ServerPassword == sentPassword)
                                    {
                                        msg.SenderConnection.Approve();
                                        Log.Warning("User (" + sentUsername + ") approved.");
                                    }
                                    else
                                    {
                                        msg.SenderConnection.Deny();
                                        Log.Warning("User (" + sentUsername + ") denied. Wrong password.");
                                    }
                                }
                                else
                                {
                                    msg.SenderConnection.Deny();
                                    Log.Warning("User (" + sentUsername + ") denied. Game is full.");
                                }
                                break;
                            #endregion

                            default:
                                Log.Warning(String.Format("Server ProcessMessage: Unhandled Type: {0}", msg.MessageType));
                                break;
                        }
                    }
                }
            }
            catch (NetException ex)
            {
                throw new MPException("NetException (Lidgren) in Server.ProcessMessage. Message: " + ex.ToString(), ex);
            }
            catch (Exception ex)
            {
                throw new MPException("Exception in Server.ProcessMessage(). Message: " + ex.ToString(), ex);
            }
            finally
            {
                Log.Warning("Leaving Server processmessage");
                IsServerStarted = false;
                StopMessageProcessingThread.Condition = false;
                OnServerLeftProcessingMessageThread(new EventArgs());
            }
        }
        
        /// <summary>
        /// Message to progress the received information.
        /// </summary>
        /// <param name="type">Type of the message. Indicates what the message's contents are.</param>
        /// <param name="msg">Received message.</param>
        private void ProgressData(MPMessageType msgType, NetIncomingMessage msg)
        {
            switch (msgType)
            {
                case MPMessageType.MoneyUpdate: //Receiving money
                    Log.Message("Server received " + msgType);
                    EcoExtBase._CurrentMoneyAmount = msg.ReadInt64();
                    EcoExtBase._InternalMoneyAmount = msg.ReadInt64();
                    break;
                case MPMessageType.DemandUpdate: //Receiving demand
                    Log.Message("Server received " + msgType);
                    DemandExtBase._CommercialDemand = msg.ReadInt32();
                    DemandExtBase._ResidentalDemand = msg.ReadInt32();
                    DemandExtBase._WorkplaceDemand = msg.ReadInt32();
                    break;
                case MPMessageType.TileUpdate:
                    Log.Message("Server received " + msgType);
                    AreaExtBase._XCoordinate = msg.ReadInt32();
                    AreaExtBase._ZCoordinate = msg.ReadInt32();
                    //INFO: The unlock process is activated once every 4 seconds simutaniously with the
                    //EcoExtBase.OnUpdateMoneyAmount(long internalMoneyAmount).
                    //Maybe I find a direct way to unlock a tile within AreaExtBase
                    break;
                default:
                    Log.Warning(String.Format("Server ProgressData: Unhandled ID/type: {0}/{1} ", (int)msgType, msgType));
                    break;
            }
        }

        /// <summary>
        /// Sends economy update to all.
        /// </summary>
        public void SendEconomyInformationUpdateToAll()
        {
            if (CanSendMessage)
            {
                NetOutgoingMessage msg = server.CreateMessage();
                msg.Write((int)MPMessageType.MoneyUpdate);
                msg.Write(EconomyManager.instance.LastCashAmount);//EcoExtBase._CurrentMoneyAmount
                msg.Write(EconomyManager.instance.InternalCashAmount);//EcoExtBase._InternalMoneyAmount
                server.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
                server.FlushSendQueue();
            }
        } //TEST: Update money from economymanager values and not from EcoExtBase.value

        /// <summary>
        /// Sends demand update to all. (Commercial demand, residental demand, workplace demand)
        /// </summary>
        public void SendDemandInformationUpdateToAll()
        {
            if (CanSendMessage)
            {
                NetOutgoingMessage msg = server.CreateMessage();
                msg.Write((int)MPMessageType.DemandUpdate);
                msg.Write(DemandExtBase._CommercialDemand);
                msg.Write(DemandExtBase._ResidentalDemand);
                msg.Write(DemandExtBase._WorkplaceDemand);
                server.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
                server.FlushSendQueue();
            }
        } //TEST: Update demant information from DemandExtBase properties

        /// <summary>
        /// Sends the information of the new unlocked tile to all.
        /// </summary>
        /// <param name="x">X coordinate of the new tile.</param>
        /// <param name="z">Z coordinate of the new tile.</param>
        public void SendAreaInformationUpdateToAll(int x, int z)
        {
            if (CanSendMessage)
            {
                NetOutgoingMessage msg = server.CreateMessage();
                msg.Write((int)MPMessageType.TileUpdate);
                msg.Write(x);
                msg.Write(z);
                server.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
                server.FlushSendQueue();
            }
        }
    }
}
