using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Serialization;
using Server;

namespace Client
{
    public class Client
    {
        IPHostEntry host;
        IPAddress ipAdd;
        IPEndPoint endPoint;

        Socket s_client; // Se usa s_client aquí
        private User user { get; set; }

        public Client(string ip, int port)
        {
            host = Dns.GetHostEntry(ip);
            ipAdd = host.AddressList[0];
            endPoint = new IPEndPoint(ipAdd, port);

            s_client = new Socket(ipAdd.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task Start()
        {
            await s_client.ConnectAsync(endPoint);
            Console.WriteLine("Conectado al servidor!");

            await HandleUserAlias();

            _ = Task.Run(() => serverConnection(s_client));

            await HandleUserMessages();
        }

        private async Task HandleUserAlias()
        {
            Console.Write("Ingrese alias: ");
            string alias = Console.ReadLine();
            user = new User(alias);
            SendObject(user);
        }

        private async Task HandleUserMessages()
        {
            while (true)
            {
                string msg = Console.ReadLine();
                SendMessage(msg, user.alias);
            }
        }

        public async Task serverConnection(Socket s_client)
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int bytesReceived = await s_client.ReceiveAsync(buffer, SocketFlags.None);

                    if (bytesReceived == 0)
                    {
                        Console.WriteLine("Servidor ha cerrado la conexión.");
                        break;
                    }

                    byte[] receivedData = new byte[bytesReceived];
                    Array.Copy(buffer, receivedData, bytesReceived);

                    Message receivedMessage;
                    try
                    {
                        receivedMessage = (Message)JSONSerialization.Deserialize(receivedData, typeof(Message));
                        Console.WriteLine(">>>" + receivedMessage.msg);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al deserializar el mensaje: " + ex.Message);
                        continue;
                    }
                }
            }
            catch (SocketException s_e)
            {
                Console.WriteLine("Error de conexión: " + s_e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error general: " + e.Message);
            }
        }

        public void SendMessage(string msg, string userFrom)
        {
            Message message = new Message(msg, userFrom);
            s_client.Send(JSONSerialization.Serialize(message));
        }

        public void SendObject(object toSend)
        {
            if (s_client == null || !s_client.Connected)
            {
                throw new InvalidOperationException("El socket no está conectado.");
            }

            s_client.Send(JSONSerialization.Serialize(toSend));
        }

        public String byteToString(byte[] bytes)
        {
            string msg = Encoding.ASCII.GetString(bytes);
            int endIndex = msg.IndexOf("\0");
            return endIndex > 0 ? msg.Substring(0, endIndex) : msg;
        }

        public byte[] stringToBytes(string msg)
        {
            return Encoding.ASCII.GetBytes(msg);
        }
    }
}

