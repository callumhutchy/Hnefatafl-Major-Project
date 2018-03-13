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
    class Message
    {
        public string message;
        public MessageType type;

        public Message()
        {

        }

        public Message(MessageType t, string msg)
        {
            message = msg;
            type = t;
        }

        public byte[] Serialize()
        {
            string output = "";
            output += type.ToString() + "/";
            output += message.ToString();

            ASCIIEncoding asen = new ASCIIEncoding();
            return asen.GetBytes(output);
        }

        public static Message Deserialize(byte[] serial)
        {
            string inbound = Encoding.ASCII.GetString(serial);
            
            string[] components = inbound.Split('/');
            MessageType type = (MessageType)Enum.Parse(typeof(MessageType), components[0]);
            string message = components[1];
            return new Message(type, message);
        }

    }
    enum MessageType
    {
        CONNECT,
        DISCONNECT,
        SERVER_REPLY
    }
}
