using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_Console_Mode
{
    [Serializable]
    public class Message
    {
        public string message;
        public MessageType type;
        public bool hasId = false;
        public bool clientMessage = false;
        public Guid userId = Guid.Empty;
        public Guid clientId;

        public Message(MessageType t, string msg)
        {
            message = msg;
            type = t;
        }

        public Message(MessageType t, string msg, Guid id)
        {
            message = msg;
            type = t;
            hasId = true;
            clientMessage = false;
            userId = id;
        }

        public Message(MessageType t, string msg, Guid uid, Guid cid)
        {
            message = msg;
            type = t;
            hasId = true;
            clientMessage = true;
            userId = uid;
            clientId = cid;
        }

        public String Serialize()
        {
            return type.ToString() + "|" + message + "|" + hasId.ToString() + "|" + clientMessage.ToString() + "|" + userId.ToString() + "|" + clientId.ToString() + "|<EOF>";
        }

        public static Message Deserialize(string input)
        {
            Console.WriteLine(input);
            string[] splitString = input.Split('|');
            MessageType type = (MessageType)Enum.Parse(typeof(MessageType), splitString[0]);
            string message = splitString[1];
            bool has = bool.Parse(splitString[2]);
            bool clientMsg = bool.Parse(splitString[3]);
            Guid uid;
            if (has)
            {
                if (clientMsg)
                {
                    uid = new Guid(splitString[4]);
                    Guid cid = new Guid(splitString[5]);
                    return new Message(type, message, uid, cid);
                }
                uid = new Guid(splitString[4]);
                return new Message(type, message, uid);
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
        FIND_GAME,
        GAME_SETUP,
        FIRST_TURN,
        WAITING_FOR_OUR_TURN,
        TAKING_OUR_TURN,
        NEXT_TURN,
        OPPONENT_DISCONNECT,
        SEND_AGAIN,
        QUIT

    }
}
