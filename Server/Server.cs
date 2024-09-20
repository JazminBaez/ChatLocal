using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

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
            byte[] buffer;
            string msg;
            int endIndex;
            c_socket = s_socket.Accept();
            Console.WriteLine("Connected to client");

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
