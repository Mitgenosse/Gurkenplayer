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
    public class Server
    {
        //Fields
        #region Fields (and objects <3)
        static Server instance; //Singleton instance object
        NetPeerConfiguration config; //Is used to create the server
        NetServer server;
        string appIdentifier = "Gurkenplayer";
        string serverPassword = "Password";
        int serverPort = 4230;
        int serverMaximumPlayerAmount = 2;
        static bool isServerStarted = false;
        string username = "Host";

        public List<User> userList;
        #endregion

        //Properties
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
        public static bool IsServerStarted
        {
            get { return Server.isServerStarted; }
            set { Server.isServerStarted = value; }
        }
        /// <summary>
        /// Returns true if the server is initialized (instance != null).
        /// </summary>
        public static bool IsServerInitialized
        {
            get
            {
                if (Instance != null)
                    return true;
                else
                    return false;
            }
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
        /// Returns true if the server is initialized and a client is connected to it.
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

        //Singleton pattern
        /// <summary>
        /// Singleton instance. There should be only one instance of the Server or Client class.
        /// With the help of the singleton parameter, we can access this class from everywhere.
        /// </summary>
        public static Server Instance
        {
            get
            {
                if (instance == null)
                    instance = new Server();

                return instance;
            }
        }

        //Constructor
        /// <summary>
        /// Private Server constructor used in the singleton pattern. It initialized the config.
        /// </summary>
        private Server()
        {
            GurkenplayerMod.MPRole = MultiplayerRole.Server;
            config = new NetPeerConfiguration(appIdentifier);
            config.Port = ServerPort;
            config.MaximumConnections = ServerMaximumPlayerAmount;
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.AutoFlushSendQueue = false;
            userList = new List<User>();
            userList.Add(new User("Host", mpRole: MultiplayerRole.Server));
        }
        /// <summary>
        /// Destructor logic.
        /// </summary>
        ~Server()
        {
        }

        //Methods
        /// <summary>
        /// Method to start the server with 3 optional parameters. Arguments which are left blank take
        /// the default value.
        /// </summary>
        /// <param name="port">Port of the server. Default: 4230</param>
        /// <param name="password">Password of the server. Default: none</param>
        /// <param name="maximumPlayerAmount">Amount of players. Default: 2</param>
        public void StartServer(int port = 4230, string password = "", int maximumPlayerAmount = 2)
        {
            Log.Message("Starting the server. IsServerStarted? j -> Instance.StopServer(). Status: " + IsServerStarted);
            if (IsServerStarted)
                Instance.StopServer();

            if (!IsServerStarted)
            {
                //Field configuration
                ServerPort = port;
                ServerPassword = password;
                ServerMaximumPlayerAmount = maximumPlayerAmount;

                //NetPeerConfiguration configuration
                config.Port = ServerPort;
                config.MaximumConnections = ServerMaximumPlayerAmount;

                //Initializes the NetServer object with the config and start it
                server = new NetServer(config);
                server.Start();
                IsServerStarted = !IsServerStarted;
                
                Log.Message("Server starting Thread.");

                //Separate thread in which the received messages are handled
                ParameterizedThreadStart pts = new ParameterizedThreadStart(this.ProcessMessage);
                Thread thread = new Thread(pts);
                thread.Start(server);
                Log.Message("Server started successfuly.");
            }
        }

        /// <summary>
        /// Stops the running server gracefully.
        /// </summary>
        public void StopServer()
        {
            if (IsServerStarted)
            {
                Log.Message("Shutting down the server");
                Log.Error("ConnectionsCount BeforeShutdown:::" + server.ConnectionsCount.ToString());

                server.Shutdown("Bye bye Server!");
                IsServerStarted = !IsServerStarted;
                userList.Clear();
                Log.Message("Server shut down");

                config = new NetPeerConfiguration(appIdentifier);
                config.Port = ServerPort;
                config.MaximumConnections = ServerMaximumPlayerAmount;
                config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
                config.AutoFlushSendQueue = false;
                Log.Error("ConnectionsCount AfterShutdown:::" + server.ConnectionsCount.ToString());

            }
        }

        /// <summary>
        /// Gets rid of the Server instance.
        /// </summary>
        public void Dispose()
        {
            Log.Message("Disposing server. Current MPRole: " + GurkenplayerMod.MPRole);
            Instance.StopServer();
            IsServerStarted = false;
            GurkenplayerMod.MPRole = MultiplayerRole.None;
            instance = null;
            Log.Message("Server disposed. Current MPRole: " + GurkenplayerMod.MPRole);
        }

        /// <summary>
        /// ProcessMessage runs in a separate thread and manages the incoming messages of the clients.
        /// </summary>
        /// <param name="obj">object obj represents a NetServer object.</param>
        private void ProcessMessage(object obj)
        {
            server = (NetServer)obj;
            NetIncomingMessage msg;

            while (IsServerStarted)
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
                                User.RemoveFromList(userList, msg.SenderConnection);
                                Log.Message("Client disconnected. Client IP: " + msg.SenderEndPoint);
                                Log.Error("ConnectionsCount new disconnect:::" + server.ConnectionsCount.ToString());
                            }
                            break;
                        #endregion

                        //If the message contains data
                        #region NetIncomingMessageType.Data
                        case NetIncomingMessageType.Data:
                            int type = msg.ReadInt32();
                            ProgressData(type, msg);
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
                                    userList.Add(new User(sentUsername, netConnection: msg.SenderConnection));
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
                            Log.Warning("Server_ProcessMessage: Unhandled type/message: " + msg.MessageType);
                            break;
                    }
                }
            }
        }
        
        /// <summary>
        /// Message to progress the received information.
        /// </summary>
        /// <param name="type">Type of the message. Indicates what the message's contents are.</param>
        /// <param name="msg">Received message.</param>
        private void ProgressData(int type, NetIncomingMessage msg)
        {
            switch (type)
            {
                case 0x2000: //Receiving money
                    Log.Message("Server received 0x2000");
                    EcoExtBase._CurrentMoneyAmount = msg.ReadInt64();
                    EcoExtBase._InternalMoneyAmount = msg.ReadInt64();
                    break;
                case 0x3000: //Receiving demand
                    Log.Message("Server received 0x3000");
                    DemandExtBase._CommercialDemand = msg.ReadInt32();
                    DemandExtBase._ResidentalDemand = msg.ReadInt32();
                    DemandExtBase._WorkplaceDemand = msg.ReadInt32();
                    break;
                case 0x4000:
                    Log.Message("Server received 0x4000");
                    AreaExtBase._XCoordinate = msg.ReadInt32();
                    AreaExtBase._ZCoordinate = msg.ReadInt32();
                    //INFO: The unlock process is activated once every 4 seconds simutaniously with the
                    //EcoExtBase.OnUpdateMoneyAmount(long internalMoneyAmount).
                    //Maybe I find a direct way to unlock a tile within AreaExtBase
                    break;
                default:
                    Log.Warning("Server_ProgressData: Unhandled type/message: " + msg.MessageType);
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
                NetOutgoingMessage msg = server.CreateMessage((int)0x2000);
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
                NetOutgoingMessage msg = server.CreateMessage((int)0x3000);
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
                NetOutgoingMessage msg = server.CreateMessage((int)0x4000);
                msg.Write(x);
                msg.Write(z);
                server.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
                server.FlushSendQueue();
            }
        }
    }
}
