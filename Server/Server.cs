using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
namespace Server
{
    public class Server
    {
        IPHostEntry host;
        IPAddress ipAdd;
        IPEndPoint endPoint;

        Socket s_socket;
        Socket? c_socket;

        public Server(string ip, int port)
        {
            host = Dns.GetHostByName(ip);
            ipAdd = host.AddressList[0];
            endPoint = new IPEndPoint(ipAdd, port);

            s_socket = new Socket(ipAdd.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            s_socket.Bind(endPoint);
            s_socket.Listen(10);

        }

        public void Start()
        {
            
            Thread clientThread;
            IPEndPoint clientEndpoint;
            Client clientData;

            while (true)
            {
                c_socket = s_socket.Accept();
                //imprime datos del cliente conectado
               clientData = new Client(((IPEndPoint)c_socket.RemoteEndPoint).Address.ToString(), ((IPEndPoint)c_socket.RemoteEndPoint).Port);
                Console.WriteLine("Connected to client: " + clientData.IP + " - " + clientData.Puerto);
                clientThread = new Thread(() => clientConnection(c_socket));
                clientThread.Start();
            }
    
        }

        public void clientConnection(Socket client)
        {
            byte[]? buffer;
            string msg;
            int endIndex;
            while (true)
            {
                buffer = new byte[1024];
                c_socket.Receive(buffer);
                msg = Encoding.ASCII.GetString(buffer);
                endIndex = msg.IndexOf("\0");

                if (endIndex > 0)
                {
                    msg = msg.Substring(0, endIndex);
                }

                Console.WriteLine("User: " + msg);
            }
        }
    }
}
