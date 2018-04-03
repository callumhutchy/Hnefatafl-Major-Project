using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System;
using System.Text;
public class NetManager : MonoBehaviour
{
    private Socket clientSocket;
    private byte[] buffer;

    private const int port = 7995;

    private Guid myId;
    private Guid gameId;


    void Awake()
    {

        StartCoroutine(StopSpam());

        try
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Connect to server
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            Debug.Log("Attempting to connect");
            //Begin connection
            clientSocket.BeginConnect(endPoint, ConnectCallback, null);

        }
        catch (SocketException ex)
        {
            Debug.LogError(ex);
        }


    }

    Thread receiveData;

    //Start the callback loop
    void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            buffer = new byte[clientSocket.ReceiveBufferSize];
            Debug.Log("Connecting");


            byte[] data = Encoding.ASCII.GetBytes(new Message(MessageType.CONNECT, "can we connect").Serialize());
            Debug.Log("Began sending " + data.Length + " bytes");
            clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), clientSocket);


            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    //This method will loop 
    void ReceiveCallback(IAsyncResult ar)
    {
        

            try
            {
                int received = clientSocket.EndReceive(ar);

                if (received == 0)
                {
                    return;
                }

                string reply = Encoding.ASCII.GetString(buffer);

                //Logic goes here for response

                Message msg = Message.Deserialize(reply);
                Debug.Log(msg.type + " : " + msg.message);

                if (!myId.Equals(msg.gameID))
                {
                    Debug.Log("Our id from the server doesn't match");
                }

                if (disconnect)
                {
					Debug.Log("Sending a leave message");
                    Send(new Message(MessageType.DISCONNECT, "Can we leave", myId).Serialize(), clientSocket);
                    disconnect = false;
                }

                switch (msg.type)
                {
                    case MessageType.CONNECT:

                        myId = msg.gameID;
                        Debug.Log("We've connected and here is our id : " + myId);
                        Send(new Message(MessageType.FIND_GAME, "Please find a game", myId).Serialize(), clientSocket);

                        break;
                    case MessageType.WAITING_FOR_PLAYER:
						Debug.Log("We are waiting for another player");
						Send(new Message(MessageType.WAITING_FOR_PLAYER, "Ok we will wait", myId).Serialize(), clientSocket);
						break;
					
					case MessageType.PLAYER_FOUND:
						Debug.Log(msg.message);
						Send(new Message(MessageType.GAME_SETUP, "We will head to game setup",myId).Serialize(), clientSocket);

						//Scene move to multiplayer game;


						break;
					
					
					
					
					
					
					
					case MessageType.DISCONNECT:
                        Debug.Log("We can disconnect");
						exit = true;
                        break;




                    default:
                        while (stopSpam)
                        {
                            if (!stopSpam)
                            {
                                break;
                            }
                        }
							Send(new Message(MessageType.IGNORE, "Waiting", myId).Serialize(), clientSocket);
						
                         break;
                }




                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        
    }
    static bool stopSpam = true;
    IEnumerator StopSpam()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            stopSpam = !stopSpam;
        }


    }

    bool allowQuitting = false;
    static bool exit = false;

    static bool disconnect = false;
    void OnApplicationQuit()
    {
        Debug.Log("Attempting to quit");
        //StartCoroutine(TimeToQuit());
        if (!allowQuitting)
        {
            //Application.CancelQuit();
        }

    }

    IEnumerator TimeToQuit()
    {
        disconnect = true;
		Debug.Log("Waiting for exit");
        yield return new WaitUntil(() => exit == true);
        Debug.Log("We can quit");
        allowQuitting = true;
        Application.Quit();
    }

    private static void Send(string text, Socket socket)
    {
        byte[] data = Encoding.ASCII.GetBytes(text);
        //Debug.Log("Began sending " + data.Length + " bytes");
        socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
    }

    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndSend(ar);
            //Debug.Log("Sent data");
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

}
