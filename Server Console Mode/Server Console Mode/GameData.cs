using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_Console_Mode
{
    [Serializable]
    class GameData
    {
        public Guid player1;
        public Guid player2;
        public Guid gameId;
        public bool p1Ready = false;
        public bool p2Ready = false;
        public Team piece1 = Team.NONE;
        public Team piece2 = Team.NONE;
        public Guid firstPlayer = Guid.Empty;
        public Guid turn = Guid.Empty;
        public string boardState;
        public bool disconnect = false;
        public Guid winner = Guid.Empty;

        private GameData(Guid p1, Guid p2, Guid g1)
        {
            player1 = p1;
            player2 = p2;
            gameId = g1;
        }

        public static GameData CreateGameData(Guid p1, Guid p2)
        {
            return new GameData(p1, p2, Guid.NewGuid());
        }

        private GameData(Guid g1, Guid p1, Guid p2, bool p1r, bool p2r, Team tm1, Team tm2, Guid first, Guid tur, string board, bool dc, Guid win)
        {
            player1 = p1;
            player2 = p2;
            gameId = g1;
            p1Ready = p1r;
            p2Ready = p2r;
            piece1 = tm1;
            piece2 = tm2;
            firstPlayer = first;
            turn = first;
            boardState = board;
            disconnect = dc;
            winner = win;

        }
        public String Serialize()
        {
            return gameId.ToString() + ":" + player1.ToString() + ":" + player2.ToString() + ":" + p1Ready.ToString() + ":" + p2Ready.ToString() + ":" + piece1.ToString() + ":" + piece2.ToString() + ":" + firstPlayer.ToString() + ":" + turn.ToString() + ":" + boardState + ":" + disconnect.ToString() + ":" + winner.ToString();
        }

        public static GameData Deserialise(string input)
        {
            string[] splitString = input.Split(':');
            Guid gID = new Guid(splitString[0]);
            Guid p1 = new Guid(splitString[1]);
            Guid p2 = new Guid(splitString[2]);
            bool p1R = bool.Parse(splitString[3]);
            bool p2R = bool.Parse(splitString[4]);
            Team tm1 = (Team)Enum.Parse(typeof(Team), splitString[5]);
            Team tm2 = (Team)Enum.Parse(typeof(Team), splitString[6]);
            Guid first = new Guid(splitString[7]);
            Guid trn = new Guid(splitString[8]);
            string board = splitString[9];
            bool dc = bool.Parse(splitString[10]);
            Guid win = new Guid(splitString[11]);
            return new GameData(gID, p1, p2, p1R, p2R, tm1, tm2, first, trn, board, dc, win);
        }

        public void SetPieces()
        {
            if (piece1 == Team.NONE && piece2 == Team.NONE)
            {
                Random rand = new Random();
                int index = rand.Next(0, 2);
                if (index == 0)
                {
                    piece1 = Team.BARBARIAN;
                    piece2 = Team.VIKING;
                }
                else
                {
                    piece1 = Team.VIKING;
                    piece2 = Team.BARBARIAN;
                }
            }
        }

        public void SetFirst()
        {
            if (firstPlayer == Guid.Empty)
            {
                Random rand = new Random();
                int index = rand.Next(0, 2);
                if (index == 0)
                {
                    firstPlayer = player1;
                }
                else
                {
                    firstPlayer = player2;
                }
            }
        }

    }

    public enum Team
    {
        VIKING,
        BARBARIAN,
        NONE
    }

}
