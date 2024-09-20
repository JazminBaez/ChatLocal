using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Sockets;

namespace Server
{
    public class Program
    {
         public void Main()
        {
            Server server = new Server("localHost", 4404);
            server.Start();
            Console.ReadKey();
        }
    }
}