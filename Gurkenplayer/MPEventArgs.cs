using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gurkenplayer
{
    /// <summary>
    /// EventArgs providing information for the events inside the ProcessMessage method.
    /// </summary>
    public class ReceivedMessageEventArgs : EventArgs
    {
        // Fields
        NetIncomingMessage msg;

        // Properties
        public NetIncomingMessage Message
        {
            get { return msg; }
        }

        // Constructor
        public ReceivedMessageEventArgs(NetIncomingMessage msg)
        {
            this.msg = msg;
        }
    }

    /// <summary>
    /// EventArgs providing information for the events inside the ProcessMessage method.
    /// </summary>
    public class ReceivedUnhandledMessageEventArgs : EventArgs
    {
        // Fields
        NetIncomingMessage msg;
        string type;

        // Properties
        public NetIncomingMessage Message
        {
            get { return msg; }
        }

        public string Type
        {
            get { return type; }
        }

        // Constructor
        public ReceivedUnhandledMessageEventArgs(NetIncomingMessage msg, string type)
        {
            this.msg = msg;
            this.type = type;
        }
    }

    /// <summary>
    /// Provides additional information than ReceivedMessageEventArgs, like a username or password.
    /// </summary>
    public class ConnectionRequestEventArgs : EventArgs
    {
        // Fields
        NetIncomingMessage msg;
        string username;
        string password;
        string note;

        // Properties
        public NetIncomingMessage Message
        {
            get { return msg; }
        }
        public string Username
        {
            get { return username; }
        }
        public string Password
        {
            get { return password; }
        }
        /// <summary>
        /// Containing additional information about the connection request.
        /// </summary>
        public string Note
        {
            get { return note; }
            set { note = value; }
        }

        // Constructor
        public ConnectionRequestEventArgs(NetIncomingMessage msg, string note, string username, string password)
        {
            this.msg = msg;
            this.username = username;
            this.password = password;
            this.note = note;
        }
    }
}