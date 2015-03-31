using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Lidgren.Network;
using System.Threading;

namespace Gurkenplayer
{
    public class Client
    {
        //Fields
        #region Fields
        private static Client instance;
        private NetPeerConfiguration config;
        private NetClient client;
        private string appIdentifier = "Gurkenplayer";
        private string serverIP = "localhost";
        private int serverPort = 4420;
        private string serverPassword = "Password";
        private static bool isClientInitialized = false;
        private static bool isClientConnected = false;
        private string username = "usr";
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
            get { return Client.isClientConnected; }
            set { Client.isClientConnected = value; }
        }
        /// <summary>
        /// Returns true if the client is initialized.
        /// </summary>
        public static bool IsClientInitialized
        {
            get { return Client.isClientInitialized; }
            set { Client.isClientInitialized = value; }
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
        private bool CanSendMessage
        {
            get
            {
                if (client != null)
                {
                    if (IsClientConnected)
                        return true;
                }
                return false;
            }
        }


        //Singleton pattern
        /// <summary>
        /// Singleton statement of Client.
        /// </summary>
        public static Client Instance
        {
            get
            {
                try
                {
                    if (instance == null)
                    {
                        instance = new Client();
                    }
                    return instance;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
                return instance;
            }
        }

        //Constructor
        /// <summary>
        /// Private Client constructor for the singleton pattern.
        /// </summary>
        private Client()
        {
            try
            {
                Log.Message("Client Constructor");
                config = new NetPeerConfiguration(appIdentifier);
                config.AutoFlushSendQueue = false; //client.SendMessage(message, NetDeliveryMethod); is needed for sending
                client = new NetClient(config);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
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
            try
            {
                if (client != null)
                {
                    Log.Message("Client ConnectToServer: Connecting");

                    //Manipulating fields
                    serverIP = ip;
                    serverPort = port;
                    serverPassword = password;

                    //Write approval message with password
                    Log.Warning("Client creating message.");
                    NetOutgoingMessage approvalMessage = client.CreateMessage();  //Approval message with password
                    approvalMessage.Write(serverPassword);
                    approvalMessage.Write(username);

                    client.Start();
                    Log.Warning("Client started.");
                    client.Connect(serverIP, serverPort, approvalMessage);
                    isClientConnected = true;
                    GurkenplayerMod.MPRole = MultiplayerRole.Client;
                    Log.Message("Client ConnectToServer: " + serverIP + " " + serverPort);

                    //Separate thread in which the received messages are handled
                    ParameterizedThreadStart pts = new ParameterizedThreadStart(this.ProcessMessage);
                    Thread thread = new Thread(pts);
                    thread.Start(client);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
        /// <summary>
        /// Disconnects the client from the server
        /// </summary>
        public void DisconnectFromServer()
        {
            if (client != null)
            {
                if (isClientConnected)
                {
                    Log.Message("!!!Disconnecting");
                    client.Disconnect("Bye Bye Client.");
                    isClientConnected = !isClientConnected;
                    Log.Message("!!!Disconected");
                }

            }
        }

        /// <summary>
        /// ProcessMessage runs in a separate thread and manages the received server messages.
        /// </summary>
        /// <param name="obj">object obj represents a NetClient object.</param>
        private void ProcessMessage(object obj)
        {
            NetClient client = (NetClient)obj;
            NetIncomingMessage msg;

            while (isClientConnected)
            {
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
                            //Log.Message("DebugMessage: " + msg.ReadString());
                            break;
                        #endregion

                        #region NetIncomingMessageType.StatusChanged
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus state = (NetConnectionStatus)msg.ReadByte();
                            if (state == NetConnectionStatus.Connected)
                            {
                                Log.Message("You connected. Client IP: " + msg.SenderEndPoint);
                            }
                            else if (state == NetConnectionStatus.Disconnected || state == NetConnectionStatus.Disconnecting)
                            {
                                Log.Message("You disconnected. Client IP: " + msg.SenderEndPoint);
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
                default: //Unbehandelte ID
                    Log.Warning("Client ProgressData: Unhandled ID/type: " + type);
                    break;
            }
        }
        /// <summary>
        /// Send the EconomyInformation of the client to the server to synchronize.
        /// </summary>
        public void SendEconomyInformationToServer()
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
    }
}
