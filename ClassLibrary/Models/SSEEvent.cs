using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DudesPlayer.Library.Models
{
    public class SSEEvent
    {
        public PacketType Event { get; set; }
        public object Data { get; set; }
        public string Id { get; set; }
        public int? Retry { get; set; }

        public SSEEvent(PacketType name, object data)
        {
            Event = name;
            Data = data;
        }
        public SSEEvent()
        {
        }
    }
}
