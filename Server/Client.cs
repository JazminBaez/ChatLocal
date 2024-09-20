using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Client
    {
        public string IP { get; set; }
        public int Puerto { get; set; }

        public Client(string ip, int puerto)
        {
            IP = ip;
            Puerto = puerto;
        }
    }
}
