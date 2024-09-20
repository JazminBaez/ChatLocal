using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using Serialization;
using System.Diagnostics;


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
            User clientData;

            while (true)
            {
                c_socket = s_socket.Accept();
                clientThread = new Thread(() => clientConnection(c_socket));
                clientThread.Start();
               
            }


    
        }

        public void clientConnection(Socket client)
        {
            byte[] buffer;
            string msg;
            User user;
            string alias;
            int bytesReceived;
             byte[] receivedData;

            c_socket.Send(stringToBytes("Welcome to the server"));

            try {
                while (true)
                {
                    buffer = new byte[1024];
                    bytesReceived = c_socket.Receive(buffer);

                    receivedData = getReceivedData(buffer, bytesReceived);

                    user = (User)JSONSerialization.Deserialize(receivedData, typeof(User));

                    alias = user.alias;


                    Console.WriteLine("Cliente conectado, " + alias);
                    Console.Out.Flush();
                }
            }catch(SocketException s_e)
            {
                Console.WriteLine("Client disconnected");
            }
            
        }

        public byte[] getReceivedData(byte[] buffer, int bytesReceived)
        {

            byte[] receivedData = new byte[bytesReceived];
            Array.Copy(buffer, receivedData, bytesReceived);
            return receivedData;
        }


        public String byteToString(byte[] bytes)
        {
            string msg;
            int endIndex;

            msg = Encoding.ASCII.GetString(bytes);
            endIndex = msg.IndexOf("\0");

            if (endIndex > 0)
            {
                msg = msg.Substring(0, endIndex);
            }

            return msg;
        }

        public byte[] stringToBytes(string msg)
        {
            return Encoding.ASCII.GetBytes(msg);
        }
    }
}
