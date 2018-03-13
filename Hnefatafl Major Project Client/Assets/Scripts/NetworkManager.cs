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

    TcpClient client = new TcpClient();

    void Awake()
    {
        try
        {

            Debug.Log("Connecting...");

            client.Connect("127.0.0.1", 15500);

            Debug.Log("Connected");

            Stream stm = client.GetStream();

            Message transmission = new Message(MessageType.CONNECT, "Hey Server");

            byte[] ba = transmission.Serialize();
            Debug.Log("Transmitting...");

            stm.Write(ba, 0, ba.Length);



        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }


    }

    void OnApplicationQuit()
    {
        try
        {
			Debug.Log("Disconnecting");
            Stream stm = client.GetStream();
            Message transmission = new Message(MessageType.DISCONNECT, "Bye");
            byte[] b = transmission.Serialize();
            stm.Write(b, 0, b.Length);
            stm.Close();
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
        byte[] b = new byte[100];
        Stream stm = client.GetStream();
        int k = stm.Read(b, 0, 100);
        if (k > 0)
        {
            Message message = Message.Deserialize(b);

            Debug.Log(message.type + " : " + message.message);
        }


    }
}
