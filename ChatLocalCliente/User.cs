using System;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;


namespace ChatLocalCliente
{
    [Serializable]
    public class User
    {
        public string alias { get; set; }

        [JsonConstructor]
        public User(string alias)
        {
            this.alias = alias;

        }

    }
}

