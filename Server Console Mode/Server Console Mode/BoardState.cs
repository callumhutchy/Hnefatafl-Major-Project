using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_Console_Mode
{
    //This class provides a way of converting the game board into a string, and can be used to turn a string back into a board
    //The functions of this class are not used in the server, it is only used to be able to reference board states
    class BoardState
    {
        //Lists if Vector 2s to store the positions of the pieces
        public List<Vector2> knightsPos;
        public List<Vector2> barbariansPos;
        //The King is referenced separately
        public Vector2 kingPos;

        public BoardState(List<Vector2> knights, List<Vector2> barbarians, Vector2 king)
        {
            knightsPos = knights;
            barbariansPos = barbarians;
            kingPos = king;
        }


        //Converts a board state into a string representation
        public string Serialize()
        {
            string knights = "";
            string barbarians = "";
            string king = "";
            foreach (Vector2 k in knightsPos)
            {
                knights += k.x + "/" + k.y + "@";
            }
            knights = knights.Remove(knights.Length - 1);
            foreach (Vector2 b in barbariansPos)
            {
                barbarians += b.x + "/" + b.y + "@";
            }
            barbarians = barbarians.Remove(barbarians.Length - 1);
            king += kingPos.x + "/" + kingPos.y;

            return knights + ":" + barbarians + ":" + king;
        }


        //Converts a string representation back into an instance of a BoardState
        public static BoardState Deserialise(string input)
        {
            string[] splitString = input.Split(':');
            string knightList = splitString[0];
            string barbarianList = splitString[1];
            string king = splitString[2];

            string[] knights = knightList.Split('@');
            string[] barbarians = barbarianList.Split('@');
            string[] kingPos = king.Split('/');

            List<Vector2> kPos = new List<Vector2>();
            List<Vector2> bPos = new List<Vector2>();

            foreach (string k in knights)
            {
                string[] pos = k.Split('/');
                kPos.Add(new Vector2(float.Parse(pos[0]), float.Parse(pos[1])));
            }

            foreach (string b in barbarians)
            {
                string[] pos = b.Split('/');
                bPos.Add(new Vector2(float.Parse(pos[0]), float.Parse(pos[1])));
            }

            return new BoardState(kPos, bPos, new Vector2(float.Parse(kingPos[0]), float.Parse(kingPos[1])));


        }


    }
}
