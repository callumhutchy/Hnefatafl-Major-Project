using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server_Application
{
    //Every client that connects will have a ClientInfo object created for them, this holds onto their socket, several IDs and whether they are in a game.
    class ClientInfo
    {
        public Socket connection;

        //User id is given to the client from the server, and the client id comes over with the first connection from the client.
        public Guid userId;
        public Guid clientId;

        //This is always false, as a user can't start in a game
        public bool gameMatch = false;

        public ClientInfo(Socket con, Guid id, Guid cid)
        {
            connection = con;
            userId = id;
            clientId = cid;
        }
    }
}
