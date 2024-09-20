using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program()
    {
        static void Main(String[] args)
        {
            Client c = new Client("localHost", 4404);
            c.Start();
            string msg;
            string alias;
            User user;



            while (true)
            {
                Console.Write("Ingrese alias: ");
                alias = Console.ReadLine();
                user = new User(alias);
                c.SendObject(user);
            }

        }
    }
}