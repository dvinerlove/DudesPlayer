using DudesPlayer.Library.Models;

namespace DudesPlayer.Library.Models
{
    public class EventInfo
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string DataJson { get; set; }
        public PacketType Type { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
