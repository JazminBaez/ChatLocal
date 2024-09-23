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
        public Client(string ip, int port)
        {
            host = Dns.GetHostEntry(ip);
            ipAdd = host.AddressList[0];
            endPoint = new IPEndPoint(ipAdd, port);

            c_socket = new Socket(ipAdd.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task Start(string alias)
        {
            await c_socket.ConnectAsync(endPoint);

            User user = new User(alias);
            SendObject(user);

            // Iniciar la recepción de mensajes en segundo plano
            _ = Task.Run(() => serverConnection());

        }



        public async Task<Message> serverConnection()
        {
            try
            {
                MessageBox.Show("Debbug: INICIANDO ESCUCHA EN SERVER");
                int bytesReceived = 1;
                while (true)
                {
                    byte[] buffer = new byte[1024];

                    MessageBox.Show("Debbug: ESPERANDO RECEPCIÓN DE MENSAJE...");

                    bytesReceived = await c_socket.ReceiveAsync(buffer, SocketFlags.None);


                   

                    byte[] receivedData = new byte[bytesReceived];
                    Array.Copy(buffer, receivedData, bytesReceived);

                    Message receivedMessage;

                    try
                    {
                        MessageBox.Show("Debbug: INTENTANDO DESERIALIZAR MENSAJE...");

                        receivedMessage = (Message)JSONSerialization.Deserialize(receivedData, typeof(Message));
                        MessageBox.Show("Debbug: MENSAJE DESERIALIZADO CON ÉXITO: " + receivedMessage.userFrom + ": " + receivedMessage.msg);

                        return receivedMessage;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al deserializar el mensaje: " + ex.Message);
                        return null; // Retornar null si hay un error en la deserialización
                    }
                }
            }
            catch (SocketException s_e)
            {
                MessageBox.Show("Error de conexión: " + s_e.Message);
                return null;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error general: " + e.Message);
                return null;
            }
        }


        public async Task<byte[]> readBuffer(Socket client)
        {
            int bytesReceived;
            byte[] buffer = new byte[1024];

            bytesReceived = await client.ReceiveAsync(buffer, SocketFlags.None);

            if (bytesReceived == 0)
            {
                Console.WriteLine($"Cliente {client.RemoteEndPoint} desconectado.");
                return null;
            }

            byte[] receivedData = new byte[bytesReceived];
            Array.Copy(buffer, receivedData, bytesReceived);

            return receivedData;
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
            if (c_socket == null || !c_socket.Connected)
            {
                throw new InvalidOperationException("El socket no está conectado.");
            }

            User user = (User)toSend;
            c_socket.Send(JSONSerialization.Serialize(toSend));
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

        public async Task<Socket> GetSocket()
        {
            return c_socket;
        }

    }
}