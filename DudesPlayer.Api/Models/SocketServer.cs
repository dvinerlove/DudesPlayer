using Fleck;
using Websocket;
using Microsoft.AspNetCore.SignalR.Client;
using Websocket.Client;
using System.Diagnostics;
using DudesPlayer.Library;

namespace DudesPlayer.Api.Models
{
    public static class SocketServer
    {
        private static HubConnection hubConnection;

        public static bool IsConnected { get; private set; }

        public static void Start()
        {
            Debug.WriteLine(String.Empty, "Подключение...");
            hubConnection = new HubConnectionBuilder()
             .WithUrl($"{Settings.BaseUrl}/chat")
             .Build();
            hubConnection.Closed += async (error) =>
            {
                Debug.WriteLine(String.Empty, "Подключение закрыто...");
                await Task.Delay(5000);
                await Connect();
            };
            Connect();

        }
        public static async Task Connect()
        {
            if (IsConnected)
                return;
            try
            {
                await hubConnection.StartAsync();
                IsConnected = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка подключения: {ex.Message}");
            }
        }
        public static async void Send(string user, string room, string message)
        {
            try
            {
                await hubConnection.InvokeAsync("Send", user, room, message, false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка отправки: {ex.Message}");
                await hubConnection.StopAsync();
                IsConnected = false;
                Start();
            }
            finally
            {
            }
        }

    }
}
