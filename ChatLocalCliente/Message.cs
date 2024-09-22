using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChatLocalCliente
{
    [Serializable]
    public class Message
    {

        public string msg { get; set; }
        public string userFrom { get; set; }

        [JsonConstructor]
        public Message(string msg, string userFrom)
        {
            this.msg = msg;
            this.userFrom = userFrom;
        }
    }
}
