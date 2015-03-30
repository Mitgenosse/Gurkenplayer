using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Lidgren.Network;
using System.Threading;

namespace Gurkenplayer
{
    public class Server
    {
        //Fields
        private static Server instance; //Singleton instance object
        NetPeerConfiguration config; //Is used to create the server
        NetServer server;
        private string appIdentifier = "Gurkenplayer";
        private string serverPassword = "Password";
        private int serverPort = 4230;
        private int serverMaximumPlayerAmount = 2;
        private static bool isServerStarted = false;
        private string username = "Host";

        public List<User> userList;

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
        private bool CanSendMessage
        {
            get
            {
                if (server != null)
                {
                    if (server.ConnectionsCount > 0)
                        return true;
                }
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
                try
                {
                    if (instance == null)
                    {
                        instance = new Server();
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
        /// Private Server constructor used in the singleton pattern. It initialized the config.
        /// </summary>
        private Server() 
        {
            try
            {
                Log.Message("Server constructor.");
                config = new NetPeerConfiguration(appIdentifier);
                config.Port = serverPort;
                config.MaximumConnections = serverMaximumPlayerAmount;
                config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
                config.AutoFlushSendQueue = false;
                userList = new List<User>();
                userList.Add(new User("Host", mpRole: MultiplayerRole.Server));
                Log.Message("approval activated");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
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
            try
            {
                if (instance != null)
                {
                    if (!IsServerStarted)
                    {
                        //Field configuration
                        serverPort = port;
                        serverPassword = password;
                        serverMaximumPlayerAmount = maximumPlayerAmount;

                        //NetPeerConfiguration configuration
                        config.Port = serverPort;
                        config.MaximumConnections = serverMaximumPlayerAmount;

                        //Initializes the NetServer object with the config and start it
                        server = new NetServer(config);
                        server.Start();
                        IsServerStarted = !IsServerStarted;
                        GurkenplayerMod.MPRole = MultiplayerRole.Server;
                        Log.Message("Server started");

                        //Separate thread in which the received messages are handled
                        ParameterizedThreadStart pts = new ParameterizedThreadStart(this.ProcessMessage);
                        Thread thread = new Thread(pts);
                        thread.Start(server);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Server StartServer Error: " + ex.ToString());
            }
        }

        /// <summary>
        /// Stops the running server gracefully.
        /// </summary>
        public void StopServer()
        {
            if (server != null)
            { //If NetServer is initialized and started, shut it down
                if (IsServerStarted)
                {
                    server.Shutdown("Bye bye Server!");
                    IsServerStarted = !IsServerStarted;
                    userList.Clear();
                    Log.Message("Shutting down the server");
                }
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
                Log.Message("In ProcessMessage thread");
                server = (NetServer)obj;
                NetIncomingMessage msg;

                while (isServerStarted)
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
                                break;
                            #endregion

                            //StatusChanged
                            #region NetIncomingMessageType.StatusChanged
                            case NetIncomingMessageType.StatusChanged:
                                NetConnectionStatus state = (NetConnectionStatus)msg.ReadByte();
                                if (state == NetConnectionStatus.Connected)
                                {
                                    Log.Message("Client connected. Client IP: " + msg.SenderEndPoint);
                                }
                                else if (state == NetConnectionStatus.Disconnected || state == NetConnectionStatus.Disconnecting)
                                {
                                    Log.Message("Client disconnected. Client IP: " + msg.SenderEndPoint);
                                    User.RemoveFromList(userList, msg.SenderConnection);
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

                                if (userList.Count <= ServerMaximumPlayerAmount)
                                {
                                    Log.Warning("User (" + sentUsername + ") trying to connect. Sent password " + sentPassword);
                                    if (serverPassword == sentPassword)
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
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
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
                case 0x2000: //
                    Log.Message("Server received 0x2000");
                    EcoExtBase._CurrentMoneyAmount = msg.ReadInt64();
                    EcoExtBase._InternalMoneyAmount = msg.ReadInt64();
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
        }
    }
}
