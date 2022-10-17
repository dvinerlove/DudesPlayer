using DudesPlayer.Library.Models;
using DudesPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DudesPlayer.Classes
{
    internal static class ClientData
    {
        public static DudesPlayer.Library.Client.Client Client { get; set; }
        public static UserModel CurrentUser { get; set; }
        public static SseController SseController { get; set; }
        public static SignalRClient SocketClient { get; set; }
        public static string BaseUrl { get; set; }
        public static bool IsDialogOpen { get; internal set; }
        public static string EventsUrl { get; internal set; }
    }
}
