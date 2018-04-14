using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_Console_Mode
{
    class BoardState
    {

        public List<Vector2> knightsPos;
        public List<Vector2> barbariansPos;

        public Vector2 kingPos;

        public BoardState(List<Vector2> knights, List<Vector2> barbarians, Vector2 king)
        {
            knightsPos = knights;
            barbariansPos = barbarians;
            kingPos = king;
        }

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
