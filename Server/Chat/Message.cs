using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Server.Chat
{
    [Serializable]
    public class Message
    {
        public string message { get; set; }
        public string userFrom { get; set; }

        [JsonConstructor]
        public Message(string message, string userFrom)
        {
            this.message = message;
            this.userFrom = userFrom;
        }
    }
}
