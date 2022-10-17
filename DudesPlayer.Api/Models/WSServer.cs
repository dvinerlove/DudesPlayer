using Fleck;

namespace DudesPlayer.Api.Models
{
    public static class WSServer
    {
        private static List<IWebSocketConnection> allsockets = new List<IWebSocketConnection>();
        public static WebSocketServer? server { get; set; }
        public static List<string> Start()
        {
            List<string> strs = new List<string>();
            strs.Add("is server null " + (server == null).ToString());
            if (server != null)
            {
                strs.Add(server.Location.ToString());
            }
            else
            {
                try
                {
                    server = new WebSocketServer("ws://0.0.0.0:41235");

                    File.AppendAllLines("ws.txt", new List<string>() { "create" });
                    File.AppendAllLines("ws.txt", new List<string>() { "ws://0.0.0.0:41235" });
                    File.AppendAllLines("ws.txt", new List<string>() { " " });
                    server.ListenerSocket.NoDelay = true;
                    server.RestartAfterListenError = true;
                    server.Start(socket =>
                    {
                        socket.OnOpen = () =>
                        {
                            File.AppendAllText("ws.txt", "on open");
                            File.AppendAllText("ws.txt", socket.ConnectionInfo.Path);
                            File.AppendAllText("ws.txt", " ");
                            allsockets.Add(socket);
                        };
                        socket.OnClose = () =>
                        {
                            allsockets.Remove(socket);
                            File.AppendAllText("ws.txt", "On Close");
                            File.AppendAllText("ws.txt", socket.ConnectionInfo.Path);
                            File.AppendAllText("ws.txt", " ");
                        };
                        socket.OnMessage = message =>
                        {

                            File.AppendAllText("ws.txt", "OnMessage");
                            File.AppendAllText("ws.txt", socket.ConnectionInfo.Path);
                            File.AppendAllText("ws.txt", message);
                            File.AppendAllText("ws.txt", " ");

                            OnMessageRecieved(message);
                        };
                    });
                }
                catch (Exception ex)
                {
                    strs.Add(ex.Message);
                }

            }
            strs.Add("");
            return strs;
        }
        private static void OnMessageRecieved(string msg)
        {
            string path = msg.Split("|").FirstOrDefault() ?? "".Replace("/", "") ?? "";

            string message = msg.Replace(path + "|", "");

            foreach (var item in allsockets.Where(x => x.ConnectionInfo.Path.Contains(path.Trim())).ToList())
            {
                if (item.IsAvailable)
                {
                    //item.Send(message);
                }
                else
                {
                    allsockets.Remove(item);
                }
            }
        }

        internal static void Send(string path, string message)
        {
            foreach (var item in allsockets.Where(x => x.ConnectionInfo.Path.Contains(path.Trim())).ToList())
            {
                if (item.IsAvailable)
                {
                    item.Send(message);
                }
                else
                {
                    allsockets.Remove(item);
                }
            }
        }
    }
}
