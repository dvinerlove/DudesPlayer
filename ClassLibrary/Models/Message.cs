using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    internal class Message
    {
        public PacketType Type { get; set; }
        public string DataJson { get;  set; }
        public UserModel User { get;  set; }
        public Message(string json)
        {
            Message message = (Message)json.ToObject(typeof(Message));
            User = message.User;
            Type = message.Type;
            DataJson = message.DataJson;
        }
        public Message()
        {
            
        }
        public Message(PacketType type, string dataJson, UserModel user)
        {
            Type = type;
            DataJson = dataJson;
            User = user;
        }

    }
}

