using ClassLibrary.Models;
using ClassLibrary;

namespace DudesPlayer.Classes.Data
{
    internal class Message
    {
        public PacketType Type { get; set; }
        public string DataJson { get;  set; }
        public Message(string json)
        {
            Message message = (Message)json.ToObject(typeof(Message));
            Type = message.Type;
            DataJson = message.DataJson;
        }
        public Message()
        {
            
        }
        public Message(PacketType type, string dataJson)
        {
            Type = type;
            DataJson = dataJson;
        }

    }
}

