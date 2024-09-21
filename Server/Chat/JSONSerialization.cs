using System;
using System.IO;
using System.Text.Json;

namespace Server.Chat
{
    class JSONSerialization
    {
        public static byte[] Serialize(object toSerialize)
        {
            string jsonString = JsonSerializer.Serialize(toSerialize);
            return System.Text.Encoding.UTF8.GetBytes(jsonString);
        }

        public static object Deserialize(byte[] toDeserialize, Type objectType)
        {
            string jsonString = System.Text.Encoding.UTF8.GetString(toDeserialize);
            return JsonSerializer.Deserialize(jsonString, objectType);
        }
    }
}

