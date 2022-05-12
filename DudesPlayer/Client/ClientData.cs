using ClassLibrary.Models;
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
        public static ClassLibrary.Client.Client Client { get; set; }
        public static UserModel CurrentUser { get; set; }
        public static SseController SseController { get; set; }
        public static SocketClient SocketClient { get; set; }
        public static string BaseUrl { get; set; } = "http://192.168.196.110:43489";
        public static bool IsDialogOpen { get; internal set; }
        public static string EventsUrl { get; internal set; }
    }
}
