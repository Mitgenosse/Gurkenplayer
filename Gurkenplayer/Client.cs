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

namespace Gurkenplayer
{
    public class Client
    {
        //Fields
        #region Fields
        static Client instance;
        NetPeerConfiguration config;
        NetClient client;
        string appIdentifier = "Gurkenplayer";
        string serverIP = "localhost";
        int serverPort = 4420;
        string serverPassword = "Password";
        //private static bool isClientInitialized = false;
        static bool isClientConnected = false;
        string username = "usr";
        //static bool messageLoopRequestStop = false;

        //public static bool MessageLoopRequestStop
        //{
        //    get { return messageLoopRequestStop; }
        //    set { messageLoopRequestStop = value; }
        //}
        Thread messageProcessingThread;
        #endregion

        //Properties
        /// <summary>
        /// Returns the IP-Address of the server.
        /// </summary>
        public string ServerIP
        {
            get { return serverIP; }
            set { serverIP = value; }
        }
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
        /// Indicates if the client is connected to a server.
        /// </summary>
        public static bool IsClientConnected
        {
            get 
            {
                if (Instance.client.ConnectionStatus == NetConnectionStatus.Connected)
                    return true;
                else if (isClientConnected)
                    return true; //true
                else
                    return false;
            }
            set { Client.isClientConnected = value; }
        }
        /// <summary>
        /// Returns true if the client is initialized (instance != null).
        /// </summary>
        public static bool IsClientInitialized
        {
            get 
            {
                if (instance != null)
                    return true;
                
                return false;
            }
        }
        /// <summary>
        /// Returns the username of the client.
        /// </summary>
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        /// <summary>
        /// Returns true when the client is initialized and connected to a server.
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
        /// Returns the current NetConnectionStatus of the client.
        /// </summary>
        public NetConnectionStatus ClientConnectionStatus
        {
            get { return client.ConnectionStatus; }
        }

        //Singleton pattern
        /// <summary>
        /// Singleton statement of Client.
        /// </summary>
        public static Client Instance
        {
            get
            {
                if (instance == null)
                    instance = new Client();

                return instance;
            }
        }

        //Constructor
        /// <summary>
        /// Private Client constructor for the singleton pattern.
        /// </summary>
        private Client()
        {
            GurkenplayerMod.MPRole = MultiplayerRole.Client;
            config = new NetPeerConfiguration(appIdentifier);
            config.MaximumHandshakeAttempts = 1;
            config.ResendHandshakeInterval = 1;
            config.AutoFlushSendQueue = false; //client.SendMessage(message, NetDeliveryMethod); is needed for sending
            client = new NetClient(config);
        }

        /// <summary>
        /// Destructor logic.
        /// </summary>
        ~Client()
        {
        }

        //Methods
        /// <summary>
        /// Method with optional parameters which is used to connect to an existing server.
        /// Empty arguments take the default value.
        /// </summary>
        /// <param name="ip">The server ip to connect to. Default: localhost</param>
        /// <param name="port">The server port which is used to connect. Default: 4230</param>
        /// <param name="password">The server password which is used to connect. Default: none</param>
        public void ConnectToServer(string ip = "localhost", int port = 4230, string password = "")
        {
            //Throw Exception when ip is not valid
            if (ip != "localhost" && ip.Split('.').Length != 4)
                throw new ArgumentException("In ConnectToServer()", ip + " is not a valid ip address.");

            Log.Message("Client connecting to server. if(IsClientConnected) -> Disconnect...(). IsClientConnected: " + IsClientConnected + " Current MPRole: " + GurkenplayerMod.MPRole);
            if (IsClientConnected)
                DisconnectFromServer();

            Log.Warning(String.Format("Client trying to connect to ip:{0} port:{1} password:{2} maxretry:{3} retryinterval:{4} MPRole:{5}", ip, port, password, client.Configuration.MaximumHandshakeAttempts, client.Configuration.ResendHandshakeInterval, GurkenplayerMod.MPRole));

            //Manipulating fields
            ServerIP = ip;
            ServerPort = port;
            ServerPassword = password;

            //Write approval message with password
            NetOutgoingMessage approvalMessage = client.CreateMessage();  //Approval message with password
            approvalMessage.Write(ServerPassword);
            approvalMessage.Write(Username);
            client.Start();
            Log.Message("Client started. Trying to connect.");
            client.Connect(ServerIP, ServerPort, approvalMessage);
            Log.Message("after client.Connect. Starting Thread now.");

            //MessageLoopRequestStop = false;
            //Separate thread in which the received messages are handled
            ParameterizedThreadStart pts = new ParameterizedThreadStart(this.ProcessMessage);
            messageProcessingThread = new Thread(pts);
            messageProcessingThread.Start(client);
            Log.Message("Client should be connected. Current MPRole: " + GurkenplayerMod.MPRole + " MessageProcessingThread alive? " + messageProcessingThread.IsAlive);
        }
        /// <summary>
        /// Disconnects the client from the server
        /// </summary>
        public void DisconnectFromServer()
        {
            Log.Message("Disconnecting from the server. Current MPRole: " + GurkenplayerMod.MPRole);

            //Checks if the messageprocessingThread is alive/running
            Log.Error("Is MessageProcessingThread alive? " + messageProcessingThread.IsAlive);
            if (messageProcessingThread.IsAlive)
            {
                Log.Error("Trying to aboard thread. Is alive? " + Instance.messageProcessingThread.IsAlive);
                messageProcessingThread.Interrupt();
                //MessageLoopRequestStop = true;
                Log.Error("Aborted thread. Is alive? " + Instance.messageProcessingThread.IsAlive);
            }
            
            //If client is connected, disconnect it
            if (IsClientConnected)
            {
                try
                {
                    client.Disconnect("Bye Bye Client.");

                    IsClientConnected = false;
                    //MessageLoopRequestStop = true;

                    //Reconfiguration
                    config = new NetPeerConfiguration(appIdentifier);
                    config.MaximumHandshakeAttempts = 1;
                    config.ResendHandshakeInterval = 1;
                    config.AutoFlushSendQueue = false; //client.SendMessage(message, NetDeliveryMethod); is needed for sending
                    client = new NetClient(config);
                    Log.Error("Disconnected. Is thread still alive? " + Instance.messageProcessingThread.IsAlive);

                }
                catch (Exception ex)
                {
                    Log.Error("Client disconnecting error. ex:" + ex.ToString());
                }
            }
            Log.Message("Client should be disconnected. Current MPRole: " + GurkenplayerMod.MPRole + " MessageProcessingThread still alive? " + messageProcessingThread.IsAlive);
        }
        /// <summary>
        /// Gets rid of the Client instance.
        /// </summary>
        public void Dispose()
        {
            try
            {
                Log.Message("Disposing client. Current MPRole: " + GurkenplayerMod.MPRole);
                Instance.DisconnectFromServer();
                if (!IsClientConnected)
                {
                    GurkenplayerMod.MPRole = MultiplayerRole.None;
                    instance = null;
                }
                else
                    throw new Exception("ClientFailedDisposeException");
                
                Log.Message("Client disposed. Current MPRole: " + GurkenplayerMod.MPRole);
            }
            catch (Exception ex)
            {
                Log.Error("Client.Dispose() Exception. " + ex.Message);
            }
        }

        /// <summary>
        /// ProcessMessage runs in a separate thread and manages the received server messages.
        /// </summary>
        /// <param name="obj">object obj represents a NetClient object.</param>
        private void ProcessMessage(object obj)
        {
            try
            {
                Log.Warning("Entering ProcessMessage");
                NetClient client = (NetClient)obj;
                NetIncomingMessage msg;

                while (true)
                {
                    if (GurkenplayerMod.MPRole != MultiplayerRole.Client)
                        break;

                    while ((msg = client.ReadMessage()) != null)
                    {
                        switch (msg.MessageType)
                        {
                            //Zum debuggen
                            #region NetIncomingMessageType Debug
                            case NetIncomingMessageType.VerboseDebugMessage: //Debug
                            case NetIncomingMessageType.DebugMessage: //Debug
                            case NetIncomingMessageType.WarningMessage: //Debug
                            case NetIncomingMessageType.ErrorMessage: //Debug
                                Log.Warning("DebugMessage: " + msg.ReadString());
                                break;
                            #endregion

                            #region NetIncomingMessageType.StatusChanged
                            case NetIncomingMessageType.StatusChanged:
                                NetConnectionStatus state = (NetConnectionStatus)msg.ReadByte();
                                Log.Warning("ProcessMessage entry state: " + state);
                                if (state == NetConnectionStatus.Connected)
                                {
                                    IsClientConnected = true;
                                    Log.Message("You connected. Client IP: " + msg.SenderEndPoint);
                                }
                                else if (state == NetConnectionStatus.Disconnected || state == NetConnectionStatus.Disconnecting || state == NetConnectionStatus.None)
                                {
                                    IsClientConnected = false;
                                    Log.Message("You disconnected. Client IP: " + msg.SenderEndPoint);
                                    GurkenplayerMod.MPRole = MultiplayerRole.Resetting;
                                }
                                break;
                            #endregion

                            #region NetIncomingMessageType.Data
                            case NetIncomingMessageType.Data:
                                int type = msg.ReadInt32();
                                ProgressData(type, msg);
                                break;
                            #endregion

                            #region NetIncomingMessageType.ConnectionApproval
                            case NetIncomingMessageType.ConnectionApproval:
                                break;
                            #endregion

                            default:
                                Log.Warning("Client ProcessMessage: Unhandled type/message: " + msg.MessageType);
                                break;
                        }
                    }
                }
                Log.Warning("Leaving Client Message Progressing Loop");
                ////MessageLoopRequestStop = false;
            }
            catch (Exception ex)
            {
                Log.Error("Client ProcessMessage Exception: " + ex.ToString());
            }
        }
        
        /// <summary>
        /// Method to process the received information.
        /// </summary>
        /// <param name="type">Type of the message. Indicates what the message's contents are.</param>
        /// <param name="msg">The message to process.</param>
        private void ProgressData(int type, NetIncomingMessage msg)
        {
            switch (type)
            {
                case 0x2000: //Receiving money
                    Log.Message("Client received 0x2000");
                    EcoExtBase._CurrentMoneyAmount = msg.ReadInt64();
                    EcoExtBase._InternalMoneyAmount = msg.ReadInt64();
                    break;
                case 0x3000: //Receiving demand
                    Log.Message("Client received 0x3000");
                    DemandExtBase._CommercialDemand = msg.ReadInt32();
                    DemandExtBase._ResidentalDemand = msg.ReadInt32();
                    DemandExtBase._WorkplaceDemand = msg.ReadInt32();
                    break;
                case 0x4000:
                    Log.Message("Client received 0x4000");
                    AreaExtBase._XCoordinate= msg.ReadInt32();
                    AreaExtBase._ZCoordinate = msg.ReadInt32();
                    //INFO: The unlock process is activated once every 4 seconds simutaniously with the
                    //EcoExtBase.OnUpdateMoneyAmount(long internalMoneyAmount).
                    //Maybe I find a direct way to unlock a tile within AreaExtBase
                    break;
                default: //Unbehandelte ID
                    Log.Warning("Client ProgressData: Unhandled ID/type: " + type);
                    break;
            }
        }
        /// <summary>
        /// Send the EconomyInformation of the client to the server to synchronize.
        /// </summary>
        public void SendEconomyInformationUpdateToServer()
        {
            if (CanSendMessage)
            {
                NetOutgoingMessage msg = client.CreateMessage((int)0x2000);
                msg.Write(EconomyManager.instance.LastCashAmount);//EcoExtBase._CurrentMoneyAmount
                msg.Write(EconomyManager.instance.InternalCashAmount);//EcoExtBase._InternalMoneyAmount
                client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                client.FlushSendQueue();
            }
        }
        /// <summary>
        /// Sends the DemandInformation of the client to the server to synchronize.
        /// </summary>
        public void SendDemandInformationUpdateToServer()
        {
            if (CanSendMessage)
            {
                NetOutgoingMessage msg = client.CreateMessage((int)0x3000);
                msg.Write(DemandExtBase._CommercialDemand);
                msg.Write(DemandExtBase._ResidentalDemand);
                msg.Write(DemandExtBase._WorkplaceDemand);
                client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                client.FlushSendQueue();
            }
        }

        public void SendAreaInformationUpdateToServer(int x, int z)
        {
            if (CanSendMessage)
            {
                NetOutgoingMessage msg = client.CreateMessage((int)0x4000);
                msg.Write(x);
                msg.Write(z);
                client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                client.FlushSendQueue();
            }
        }
    }
}
