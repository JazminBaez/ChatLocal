using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Serialization;


namespace ChatLocalCliente
{
    public class Client
    {
        IPHostEntry host;
        IPAddress ipAdd;
        IPEndPoint endPoint;

        Socket c_socket;
        User user;
        public Client(string ip, int port) {
            host = Dns.GetHostEntry(ip);
            ipAdd = host.AddressList[0];
            endPoint = new IPEndPoint(ipAdd, port);

            c_socket = new Socket(ipAdd.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task Start()
        {
            await c_socket.ConnectAsync(endPoint);

            Console.WriteLine("Conectado al servidor!");

            _ = Task.Run(() => serverConnection(c_socket));

        }

        public async Task serverConnection(Socket c_socket)
        {
            try
            {
                Console.WriteLine("Debbug: ESCUCHANDO SERVER");
                byte[] buffer = new byte[1024];

                while (true)
                {
                    int bytesReceived = await c_socket.ReceiveAsync(buffer, SocketFlags.None);

                    if (bytesReceived == 0)
                    {
                        Console.WriteLine("Desconectado del servidor.");
                        break;
                    }


                    Message receivedMessage = await this.Received();
                    Console.WriteLine("--" + receivedMessage.userFrom + ": " + receivedMessage.msg);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void SendMessage(string msg, string userFrom)
        {
            Message message = new Message(msg, userFrom);
            string jsonString = JsonSerializer.Serialize(message);
            c_socket.Send(JSONSerialization.Serialize(message));

        }

        public async Task<Message> Received()
        {
            byte[] buffer = new byte[1024];
            int bytesReceived;
            Message msg;

            c_socket.Receive(buffer);
            bytesReceived = await c_socket.ReceiveAsync(buffer, SocketFlags.None);

            byte[] receivedData = new byte[bytesReceived];
            Array.Copy(buffer, receivedData, bytesReceived);

           return msg = (Message)JSONSerialization.Deserialize(receivedData, typeof(Message));
            
            
        }

        public void SendObject(object toSend)
        {
            User user = (User)toSend;
            c_socket.Send(JSONSerialization.Serialize(toSend));
            string jsonString = JsonSerializer.Serialize(toSend);

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
