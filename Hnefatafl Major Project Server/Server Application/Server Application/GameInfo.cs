using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Server_Application
{
    class GameInfo
    {
        public Socket Player1;
        public Socket Player2;
        public Guid gameID;

        public GameInfo(Socket p1, Socket p2, Guid id)
        {
            Player1 = p1;
            Player2 = p2;
            gameID = id;
        }

    }
}
