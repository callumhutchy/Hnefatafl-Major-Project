using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System;
using System.Threading;
using TMPro;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{


    public TMP_Text connectionStatus;

    public static Guid myId;
    public static Guid gameId;

    void Awake()
    {
       

        Debug.Log("0");

        _thread = new Thread(StartClient);
        _thread.Start();
    }

    Thread _thread;

    private const int port = 7995;

    // ManualResetEvent instances signal completion.  
    private static ManualResetEvent connectDone =
        new ManualResetEvent(false);
    private static ManualResetEvent sendDone =
        new ManualResetEvent(false);
    private static ManualResetEvent receiveDone =
        new ManualResetEvent(false);

    // The response from the remote device.  
    private static String response = String.Empty;

    static Socket MainClient;

    private static void StartClient()
    {
        try
        {
            //Server IP, for testing this will be the local IP of the machine
            IPHostEntry ipHostInfo = Dns.GetHostEntry("127.0.0.1");
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);


            // Create a TCP/IP socket.  
             MainClient = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            // Connect to the remote endpoint.  
            MainClient.BeginConnect(remoteEP,
                new AsyncCallback(ConnectCallback),MainClient);
            connectDone.WaitOne();

            //Send a connection request to the server
            Send(MainClient, new Message(MessageType.CONNECT, "Hey I want to connect").Serialize());


            sendDone.WaitOne();

                    // Receive the response from the remote device.  
            AcceptCallback(MainClient);
            receiveDone.WaitOne();
            
            

            // Release the socket.  
            //client.Shutdown(SocketShutdown.Both);
            //client.Close();

        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.  
            //Socket client = (Socket)ar.AsyncState;

            // Complete the connection.  
            MainClient.EndConnect(ar);

            Debug.Log("Socket connected to " +
                MainClient.RemoteEndPoint.ToString());

            // Signal that the connection has been made.  
            connectDone.Set();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    static bool connectedToTheServer = false;
    static bool searchingForGame = false;
    static bool inGame = false;

    static bool leaveScene = false;



    private static Message Logic(Message message, Socket s){
        if(quit){
            Debug.Log("Time to quit");
            return  new Message(MessageType.DISCONNECT, "BYE", myId);   
        }

        switch(message.type){
            case MessageType.CONNECT:
                connectedToTheServer = true;
                myId = message.gameID;
                Debug.Log(myId);
                return new Message(MessageType.FIND_GAME, "Please get me a game", myId);
                break;
            case MessageType.DISCONNECT:
                Debug.Log("We can disconnect");
                connectedToTheServer = false;
                exit = true;
                leaveScene = true;

                break;


            default:

            break;
            
        }

        return new Message(MessageType.IGNORE, "Weve crashed", myId);

    }

    private static void AcceptCallback(Socket client)
    {
        try
        {
            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = client;

            // Begin receiving the data from the remote device.  
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private static void ReadCallback(IAsyncResult ar)
    {
        try
        {
            String content = String.Empty;
            // Retrieve the state object and the client socket   
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state.workSocket;

            // Read data from the remote device.  
            int bytesRead = client.EndReceive(ar);

            if (bytesRead > 0)
            {
                 state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read   
                // more data.  
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the   
                    // client. Display it on the console.  
                    //Log("Read "+ content.Length+" bytes from socket. \n Data : " +   content);

                    Message msg = Message.Deserialize(content);
                    Debug.Log(msg.type.ToString() + ": " + msg.message);

                    
                    // Echo the data back to the client.  
                    Message response = Logic(msg, client);


                    Send(client, response.Serialize());
                }
                else
                {
                    // Not all data received. Get more.  
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
            else
            {
                // All the data has arrived; put it in response.  
                if (state.sb.Length > 1)
                {
                    response = state.sb.ToString();
                }
                // Signal that all bytes have been received.  
                receiveDone.Set();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private static void Send(Socket client, String data)
    {
        // Convert the string data to byte data using ASCII encoding.  
        byte[] byteData = Encoding.ASCII.GetBytes(data);
    
        // Begin sending the data to the remote device.  
        client.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), client);
    }

    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.  
            Socket client = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.  
            int bytesSent = client.EndSend(ar);
            Debug.Log("Sent " + bytesSent + " bytes to server.");

            // Signal that all bytes have been sent.  
            sendDone.Set();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    static bool quit = false;

    static bool exit = false;

    bool allowQuitting = false;

    void OnApplicationQuit()
    {
       quit = true;
       StartCoroutine(SendMessage());

       if(!allowQuitting){
           Application.CancelQuit();
       }

        
    }

    IEnumerator SendMessage(){
        
        Send(MainClient, new Message(MessageType.DISCONNECT, "Please can i leave", myId).Serialize());
        yield return new WaitUntil(() => exit == true);
        allowQuitting = true;
        Application.Quit();

    }


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
   
    }
}

public class StateObject
{
    // Client socket.  
    public Socket workSocket = null;
    // Size of receive buffer.  
    public const int BufferSize = 256;
    // Receive buffer.  
    public byte[] buffer = new byte[BufferSize];
    // Received data string.  
    public StringBuilder sb = new StringBuilder();
}

