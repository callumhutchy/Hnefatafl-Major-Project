using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System;

public class NetworkManager : MonoBehaviour
{

    // Use this for initialization

    void Awake()
    {
        try
        {
            TcpClient client = new TcpClient();
            Debug.Log("Connecting...");

            client.Connect("127.0.0.1", 15500);

            Debug.Log("Connected");

            Stream stm = client.GetStream();

			Message transmission = new Message(MessageType.CONNECT, "Hey Server");

            byte[] ba = transmission.Serialize();
            Debug.Log("Transmitting...");

            stm.Write(ba, 0, ba.Length);

            byte[] bb = new byte[100];
            stm.Read(bb, 0, 100);
			Message message = Message.Deserialize(bb);

            Debug.Log(message.type + " : " + message.message);

            client.Close();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }


    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
