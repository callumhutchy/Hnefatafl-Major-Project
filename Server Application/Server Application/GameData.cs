using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_Application
{
    class GameData
    {
        public Guid player1;
        public Guid player2;
        public Guid gameId;
        public bool p1Ready = false;
        public bool p2Ready = false;

        public GameData (Guid p1, Guid p2)
        {
            player1 = p1;
            player2 = p2;
            gameId = Guid.NewGuid();
        }

       

    }
}
