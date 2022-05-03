using Fleck;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;

namespace AspWebServer.Controllers
{
    public static class WSServer
    {
        private static List<IWebSocketConnection> allsockets = new List<IWebSocketConnection>();
        public static WebSocketServer? server { get; set; }
        public static List<string> Start()
        {
            List<string> strs = new List<string>();
            strs.Add("is server null " + (WSServer.server == null).ToString());
            if (WSServer.server != null)
            {
                strs.Add(WSServer.server.Location.ToString());
            }
            else
            {
                try
                {
                    WSServer.server = new WebSocketServer("ws://0.0.0.0:41235");

                    File.AppendAllLines("ws.txt", new List<string>() { "create" }  );
                    File.AppendAllLines("ws.txt", new List<string>() { "ws://0.0.0.0:41235" }  );
                    File.AppendAllLines("ws.txt", new List<string>() { " " }  );
                    WSServer.server.ListenerSocket.NoDelay = true;
                    WSServer.server.RestartAfterListenError = true;
                    WSServer.server.Start(socket =>
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

    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public List<string> Get()
        {
            List<string> strs = new List<string>();
            try
            {
                var result = WSServer.Start();
                strs.AddRange(result);

            }
            catch (Exception ex)
            {
                strs.Add(ex.Message);

            }
            strs.Add("");
            strs.Add("");

            foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                strs.Add("Name: " + netInterface.Name);
                strs.Add("Description: " + netInterface.Description);
                strs.Add("Addresses: ");
                IPInterfaceProperties ipProps = netInterface.GetIPProperties();
                foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
                {
                    strs.Add(" " + addr.Address.ToString());
                }
                strs.Add("");
            }

            return strs;
        }
    }
}
