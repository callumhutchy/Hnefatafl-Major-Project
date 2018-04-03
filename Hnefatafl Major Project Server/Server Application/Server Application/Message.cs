using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Server_Application
{
    [Serializable]
    public class Message
    {
        public string message;
        public MessageType type;
        public Guid gameID;

        public Message(MessageType t, string msg)
        {
            message = msg;
            type = t;
        }

        public Message(MessageType t, string msg, Guid id)
        {
            message = msg;
            type = t;
            gameID = id;
        }
       public Message()
        {


        }

        public String Serialize()
        {
            /*/
            string output = "";
            output += type.ToString() + "/";
            output += message.ToString();

            ASCIIEncoding asen = new ASCIIEncoding();
            return asen.GetBytes(output);
    */
            return type.ToString() + "|" + message + "|" + gameID + "|" + "<EOF>";



        }

        public static Message Deserialize(string input)
        {
            /*string inbound = Encoding.ASCII.GetString(serial);

            string[] components = inbound.Split('/');
            MessageType type = (MessageType)Enum.Parse(typeof(MessageType), components[0]);
            string message = components[1];
            return new Message(type, message);*/
            string[] splitString = input.Split('|');
            MessageType type = (MessageType)Enum.Parse(typeof(MessageType), splitString[0]);
            string message = splitString[1];
            Guid id;
            if (splitString[2] != null)
            {
                id = new Guid(splitString[2]);
                return new Message(type, message, id);
            }
            return new Message(type, message);


        }
    }
    public enum MessageType
    {
        CONNECT,
        DISCONNECT,
        SERVER_REPLY,
        WAITING_FOR_PLAYER,
        PLAYER_FOUND,
        FINISHED_TURN,
        YOUR_TURN,
        GAME_OVER,
        IGNORE,
        FIND_GAME

    }

}
