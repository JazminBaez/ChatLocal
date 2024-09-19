using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Sockets;

namespace Server
{
    public class Server
    {
        IPHostEntry host;
        IPAddress ipAdd;
        IPEndPoint endPoint;

        Socket s_socket;
        public Server(string ip, int port)
        {
            host = Dns.GetHostByName(ip);
            ipAdd = host.AddressList[0];
            endPoint = new IPEndPoint(ipAdd, port);

            s_socket = new Socket(ipAdd.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            s_socket.Bind(endPoint);
            s_socket.Listen(10);

        }
    }
}
