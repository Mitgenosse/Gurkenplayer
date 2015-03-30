using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurkenplayer
{
    public class User
    {
        //Field
        string username;
        NetConnection netConnection;
        MultiplayerRole mpRole;

        //Properties
        public string Username
        {
            get { return username; }
            set { username = value; }
        }
        public NetConnection NetConnection
        {
            get { return netConnection; }
            set { netConnection = value; }
        }
        public MultiplayerRole MPRole
        {
            get { return mpRole; }
            set { mpRole = value; }
        }

        //Constructor
        public User(string username, NetConnection netConnection = null, MultiplayerRole mpRole = MultiplayerRole.Client)
        {
            this.username = username;
            this.netConnection = NetConnection;
            this.mpRole = mpRole;
        }

        //Static methods
        public static void RemoveFromList(List<User> userList, NetConnection netConnection)
        {
            foreach (User user in userList)
            {
                if (user.NetConnection == netConnection)
                {
                    userList.Remove(user);
                    break;
                }
            }
        }
    }
}
