using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server_Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> logQueue = new List<string>();

        private List<Tuple<Message, Socket>> SendQueue = new List<Tuple<Message, Socket>>();

        private List<Tuple<Message, Socket>> RecieveQueue =  new List<Tuple<Message, Socket>>();

        private List<Socket> clients = new List<Socket>();

        string logPath = "log.txt";

        bool serverStarted = false;

        Socket listener;
        int port = 7995;
        byte[] byteData = new byte[1024];

        public MainWindow()
        {
            InitializeComponent();
            Thread logManagementThread = new Thread(new ThreadStart(LogManagement));
            logManagementThread.Start();
            Log("Started Log Manager");

        }

        private void btnStartServer_Click(object sender, RoutedEventArgs e)
        {
            if (!serverStarted)
            {
                Thread serverListenThread = new Thread(new ThreadStart(ServerListen));
                Thread serverReplyThread = new Thread(new ThreadStart(ServerReply));
                Thread serverLogicThread = new Thread(new ThreadStart(ServerLogic));

                serverStarted = true;

                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPEndPoint localEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
                listener = new Socket(localEP.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEP);


                Log("Starting Server");
                serverListenThread.Start();
                serverReplyThread.Start();
                serverLogicThread.Start();

               

            }
        }

      
       
        private void Log(string msg)
        {
            logQueue.Add(msg);
        }


        private void LogManager()
        {
            if(logQueue.Count > 0)
            {
                string message = "[" + DateTime.Now + "]: " + logQueue[0] + "\n";
                logQueue.RemoveAt(0);
                this.Dispatcher.Invoke(() => {
                    txtLog.Text += message;
                    txtLog.ScrollToEnd();

                });


                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine(message);
                }
            }
            
            

        }

        public void ReadCallback(IAsyncResult ar)
        {
            Log("Received");
            Message message;

            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = handler.EndReceive(ar);

            if(bytesRead > 0)
            {

            }

        }

        public  void AcceptCallback(IAsyncResult ar)
        {
            Socket s = (Socket)ar.AsyncState;
           
            if(s != null)
            {
                Socket handler = listener.EndAccept(ar);
                StateObject state = new StateObject();
                state.workSocket = handler;
                Log("Connection is not null");
                handler.BeginReceive(state.buffer, 0, state.buffer.Length, 0, new AsyncCallback(ReadCallback), state);
            }

        }

        //Thread Methods
        private void LogManagement()
        {
            
            if (File.Exists(logPath))
            {
                string newPath = DateTime.Now.ToString().Replace("/", "_").Replace(":", "_") + " log.txt";
                File.Move(logPath, newPath );
            }
            else
            {
                StreamWriter sw = File.CreateText(logPath);
                sw.Close();
            }

            while (true)
            {
                LogManager();
            }
           
        }

        private void ServerListen()
        {
            try
            {

                

               // Log("Server started on port 7995");
                //Log("The local endpoint is :" + ipEndpoint);
                Log("Waiting for a connection...");

                while (true)
                {
                    listener.Listen(100);
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);


                    /*
                    clients.Add(s);

                    Log("Connection accepted from " + s.RemoteEndPoint);

                    
                    byte[] b = new byte[100];
                    s.Receive(b);
                    Log("Received...");
                    Message message = Message.Deserialize(b);
                   
                    
                    RecieveQueue.Add(new Tuple<Message, Socket>(message,s));

                    s.BeginAccept(new AsyncCallback(AcceptCallback), s);
                    
                */
                }
                
                //listener.Stop();
                
            }
            catch (Exception e)
            {
                Log("Error...." + e.StackTrace);
            }
            
        }

        private void ServerReply()
        {
            while (true)
            {
                if(SendQueue.Count > 0)
                {
                    try
                    {
                        Message message = SendQueue[0].Item1;
                        Socket client = SendQueue[0].Item2;
                        
                        byte[] serializedMessage;
                        var formatter = new BinaryFormatter();
                        using (var stream = new MemoryStream())
                        {
                            formatter.Serialize(stream, message);
                            serializedMessage = message.Serialize();
                        }

                        client.Send(serializedMessage);

                        Log("Sent Acknowledgement");

                        SendQueue.RemoveAt(0);
                        client.Close();
                    }
                    catch(Exception e)
                    {
                        Log(e.StackTrace);
                    }


                }
            }
        }
    
        private void ServerLogic()
        {
            while (true)
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblClients.Content = "Clients: " + clients.Count;
                });


                if(RecieveQueue.Count > 0)
                {

                    try
                    {
                        string message = RecieveQueue[0].Item1.message;
                        Socket client = RecieveQueue[0].Item2;
                        
                        SendQueue.Add(new Tuple<Message, Socket>(new Message(MessageType.SERVER_REPLY, "Your message has been received"), client));
                        RecieveQueue.RemoveAt(0);
                    }
                    catch(Exception e)
                    {
                        Log(e.StackTrace);
                    }
                   
                }
            }
        }

    }
}
