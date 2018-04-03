using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;

using System;
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

        private GameData (Guid p1, Guid p2, Guid g1)
        {
            player1 = p1;
            player2 = p2;
            gameId = g1;
        }

        public static GameData CreateGameData(Guid p1, Guid p2)
        {
            return new GameData(p1, p2, Guid.NewGuid());
        }

        private GameData(Guid g1, Guid p1, Guid p2, bool p1r, bool p2r, Team tm1, Team tm2, Guid first)
        {
            player1 = p1;
            player2 = p2;
            gameId = g1;
            p1Ready = p1r;
            p2Ready = p2r;
            piece1 = tm1;
            piece2 = tm2;
            firstPlayer = first;
        }
        public String Serialize()
        {
            return gameId.ToString() + ":" + player1.ToString() + ":" + player2.ToString() + ":" + p1Ready.ToString() + ":" + p2Ready.ToString() + ":" + piece1.ToString() + ":" +piece2.ToString() + ":" + firstPlayer.ToString();
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
            return new GameData(gID, p1, p2, p1R, p2R, tm1,tm2, first);
        }

        public  void SetPieces()
        {
            if(piece1 == Team.NONE && piece2 == Team.NONE)
            {
                System.Random rand = new System.Random();
                int index = rand.Next(0, 1);
                if(index == 0)
                {
                    piece1 = Team.BARBARIANS;
                    piece2 = Team.VIKINGS;
                }
                else
                {
                    piece1 = Team.VIKINGS;
                    piece2 = Team.BARBARIANS;
                }
            }
        }
        
        public void SetFirst()
        {
            if(firstPlayer == Guid.Empty)
            {
                System.Random rand = new System.Random();
                int index = rand.Next(0, 1);
                if(index == 0)
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

    enum Team
    {
        VIKINGS,
        BARBARIANS,
        NONE
    }
