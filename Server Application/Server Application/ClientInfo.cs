using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server_Application
{
    class ClientInfo
    {
        public Socket connection;
        public Guid userId;
        public bool gameMatch = false;

        public ClientInfo(Socket con, Guid id)
        {
            connection = con;
            userId = id;
        }
    }
}
