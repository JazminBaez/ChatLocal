using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Collections;


namespace Server.Chat
{
    public class Server
    {
        IPHostEntry host;
        IPAddress ipAdd;
        IPEndPoint endPoint;

        Socket s_socket;
        Thread clientThread;
        Hashtable users;

        public Server(string ip, int port)
        {
            try
            {
                host = Dns.GetHostByName(ip);
                ipAdd = host.AddressList[0];
                endPoint = new IPEndPoint(ipAdd, port);

                s_socket = new Socket(ipAdd.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                s_socket.Bind(endPoint);
                s_socket.Listen(10);

                clientThread = new Thread(this.Listen);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            

        }

        private void Listen()
        {

            Socket client;
            while (true)
            {
                client = this.s_socket.Accept();
                clientThread = new Thread(() => clientConnection(client));
                clientThread.Start();
            }

        }

        private void Send(Socket s, object toSend)
        {
            byte[] buffer = new byte[1024];
            byte[] objectSerialized = JSONSerialization.Serialize(toSend);
            Array.Copy(objectSerialized, buffer, objectSerialized.Length);
            s.Send(buffer);
        }

        private object Received(Socket s)
        {
            byte[] buffer = new byte[1024];
            s.Receive(buffer);
            return JSONSerialization.Deserialize(buffer, typeof(User));
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

            try
            {
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
            }
            catch (SocketException s_e)
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


        public string byteToString(byte[] bytes)
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
