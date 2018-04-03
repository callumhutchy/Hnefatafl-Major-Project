using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;


namespace Server_Application
{
    class SocketIdentifier
    {
        public Socket socket;
        public Guid userID;

        public SocketIdentifier(Socket client, Guid id)
        {
            socket = client;
            userID = id;
        }


    }
}
