using ClassLibrary;
using ClassLibrary.Models;
using DudesPlayer.Classes;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Websocket.Client;

namespace DudesPlayer.Models
{
    public class SocketClient
    {
        HubConnection hubConnection;

        string user;

        string room;
        public SocketClient()
        {

        }
        public async void Stop()
        {
            if (hubConnection != null)
            {
                await hubConnection.StopAsync();
            }
            IsConnected = false;
            IsBusy = false;
        }
        public void Start()
        {
            user = ClientData.CurrentUser.Username;

            room = "/" + ClientData.CurrentUser.RoomName;

            if (IsConnected)
            {
                Stop();
            }

            hubConnection = new HubConnectionBuilder()
            .WithUrl("http://dudesplayer.somee.com/chat")
            .Build();
            hubConnection.Closed += async (error) =>
            {
                VDebug.WriteLine("Подключение закрыто...");
                await Task.Delay(5000);
                Connect();
            };

            Connect();

            SendMessage(user, room, "login", true);

            hubConnection.On<string, string>("Receive", (user, message) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    MessageReceived(user, message);
                });
            });

        }
        public void Send(string message)
        {
            SendMessage(user, room, message, false);
        }
        SSEEvent oldsse = new();
        SSEEvent newsse = new();

        public static bool IsConnected { get; private set; }
        public bool IsBusy { get; private set; }

        public async void Connect()
        {
            if (IsConnected)
                return;

            try
            {
                await hubConnection.StartAsync();
                VDebug.WriteLine("Вы вошли в чат...");

                SSEEvent sse = new SSEEvent() { Event = PacketType.LoginNew, Data = "hello" };
                ClientData.SocketClient.Send(sse.ToJson());

                IsConnected = true;
            }
            catch (Exception ex)
            {
                VDebug.WriteLine($"Ошибка подключения: {ex.Message}");
            }
        }

        async void SendMessage(string user, string room, string message, bool isLogin)
        {
            try
            {
                IsBusy = true;
                await hubConnection.InvokeAsync("Send", user, room, message, isLogin);
            }
            catch (Exception ex)
            {
                VDebug.WriteLine($"Ошибка отправки: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void MessageReceived(string user, string msg)
        {
            VDebug.WriteLine($"from user: {user}");
            VDebug.WriteLine($"Message received: {msg}");

            oldsse = newsse;
            newsse = msg.ToString().ToObject<SSEEvent>();

            if (newsse == null)
            {
                VDebug.WriteLine("!!!!!!! UNKNOWN SSE EROR !!!!!!!");
                return;
            }
            else
            {
                if (oldsse != null)
                {
                    if (newsse.ToJson() == oldsse.ToJson())
                    {
                        //return;
                    }
                }
            }

            VDebug.WriteLine("SSE EVENT CODE:\n" + ":" + newsse.Event.ToString() + ":");

            switch (newsse.Event)
            {
                case PacketType.Ping:
                    VDebug.WriteLine("ping)");
                    break;
                case PacketType.UpdateAll:
                    var obj = newsse.Data.ToString().ToObject(typeof(RoomInfo));
                    if (obj != null)
                    {
                        var newRoom = (RoomInfo)obj;
                        ClientData.Client.SetRoom(newRoom);
                        ClientCommandHandler.UpdateInvoke();
                    }
                    break;
                case PacketType.Play:
                    ClientCommandHandler.PlayInvoke();
                    break;
                case PacketType.Set:
                    ClientCommandHandler.SetInvoke(newsse.Data.ToString().ToObject<URLModel>());
                    break;
                case PacketType.Pause:
                    ClientCommandHandler.PauseInvoke();
                    break;
                case PacketType.Time:
                    ClientCommandHandler.TimeInvoke(newsse.Data.ToString());
                    break;
                case PacketType.Joke:
                    ClientCommandHandler.JokeInvoke(newsse.Data.ToString());
                    break;
                case PacketType.Shake:
                    ClientCommandHandler.ShakeInvoke();
                    break;
                case PacketType.Chat:
                    ClientCommandHandler.ChatMessageInvoke(user, newsse.Data.ToString());
                    break;
                case PacketType.LoginNew:
                    SoundHandler.Play(SoundType.login);
                    break;
                case PacketType.Logout:
                    SoundHandler.Play(SoundType.logout);
                    break;
            }
            VDebug.WriteLine("SSE End...");

        }
    }
}
