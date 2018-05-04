using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_Console_Mode
{   

    //The Message class that is used to help transfer information between programs
    [Serializable]
    public class Message
    {
        //Main contents of the message
        public string message;
        //
        public MessageType type;
        //Used to detect whether this is the first time a client is connecting
        public bool hasId = false;
        //Used to say if this is a client message to enable extra authentication
        public bool clientMessage = false;
        //A Guid for the userid
        public Guid userId = Guid.Empty;
        //A Guid for the clientid
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

        //The serialize method converts an instance of the class into a string representation.
        public String Serialize()
        {
            return type.ToString() + "|" + message + "|" + hasId.ToString() + "|" + clientMessage.ToString() + "|" + userId.ToString() + "|" + clientId.ToString() + "|<EOF>";
        }

        //This will convert a string representation into an object of the class
        public static Message Deserialize(string input)
        {
            Console.WriteLine(input);
            string[] splitString = input.Split('|');
            MessageType type = (MessageType)Enum.Parse(typeof(MessageType), splitString[0]);
            string message = splitString[1];
            bool has = bool.Parse(splitString[2]);
            bool clientMsg = bool.Parse(splitString[3]);
            Guid uid;
            
            //If the client has been provided a user id
            if (has)
            {
                //If the message is coming from the client to the server then we need to retrieve the client id as well
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

    //The enumerator used to switch between functions when handling a message
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
