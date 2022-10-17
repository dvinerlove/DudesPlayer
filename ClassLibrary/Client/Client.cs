using System;
using System.Diagnostics;
using System.Windows;
using DudesPlayer.Library;
using DudesPlayer.Library.Models;
using RestSharp;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

namespace DudesPlayer.Library.Client
{
    public enum ApiType
    {
        rooms,
        users,
        urls,
        commands
    }

    public class Client
    {
        private static UserModel currentUser = new UserModel();
        private string baseUrl;
        private RoomInfo? roomInfo;

        public Client()
        {

        }
        public event EventHandler ErrorEvent;
        public RoomInfo GetRoom()
        {
            return roomInfo ?? new RoomInfo();
        }
        public void SetRoom(RoomInfo room)
        {
            roomInfo = room;
        }
        public bool CreateRoom()
        {
            var result = Post(ApiType.rooms, "", "", Method.POST);
            //if (ClientData.Room != null && result)
            //{
            //    ClientData.CurrentUser.RoomName = ClientData.Room.Name;
            //}
            return result;
        }
        public void SetUri(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }
        public void SetUser(UserModel user)
        {
            currentUser = user ?? new UserModel();
        }


        public bool Login()
        {
            var result = Post(ApiType.users, "", /*ClientData.CurrentUser.ToString()*/currentUser.ToString(), Method.POST);
            //if (result)
            //{
            //    ClientData.SseController = new SseController();
            //    ClientData.SseController.Run();
            //    if (ClientData.SocketClient == null)
            //    {
            //        ClientData.SocketClient = new SocketClient();
            //    }
            //    else
            //    {
            //        ClientData.SocketClient.Stop();
            //        ClientData.SocketClient = new SocketClient();
            //    }
            //    ClientData.SocketClient.Start();
            //}
            return result;
        }

        //public void SendChatMessage(string messageText)
        //{
        //    if (ClientData.SocketClient != null)
        //    {
        //        SSEEvent sse = new SSEEvent() { Event = PacketType.Chat, Data = messageText };
        //        ClientData.SocketClient.Send(sse.ToJson());
        //    }
        //}

        public bool AddUrl(URLModel link)
        {
            return Post(ApiType.urls, $"{currentUser.RoomName}", link.ToJson(), Method.POST);
        }


        public bool Disconnect()
        {
            if (currentUser != null)
            {
                return Post(ApiType.users, null, $"{currentUser}", Method.DELETE);
            }
            else
            {
                return true;
            }
        }
        public bool DeleteUrl(URLModel link)
        {
            return Post(ApiType.urls, $"{currentUser.RoomName}", link.ToJson(), Method.DELETE);
        }

        public void ShakeScreen()
        {
            Post(ApiType.commands, $"{currentUser}/shake", "", Method.POST);
        }

        public bool SetCurrentVideo(URLModel link)
        {
            return Post(ApiType.commands, $"{currentUser}/set", link.ToJson(), Method.POST);
        }
        public bool CreateJoke(Joke joke)
        {
            var json = joke.setup + "?.?123**" + joke.punchline;
            return Post(ApiType.commands, $"{currentUser}/joke", json, Method.POST);
        }
        public bool Play()
        {
            return Post(ApiType.commands, $"{currentUser}/play", null, Method.POST);
        }
        public bool Pause()
        {
            return Post(ApiType.commands, $"{currentUser}/pause", null, Method.POST);
        }
        public bool SetTime(long value)
        {
            return Post(ApiType.commands, $"{currentUser}/time", value.ToString(), Method.POST);

        }
        private bool Post(ApiType apiType, string path, string body, Method method)
        {
            try
            {
                var client = new RestClient($"{baseUrl}/api/{apiType}/{path}");
                VDebug.WriteLine($"{baseUrl}/api/{apiType}/{path}");
                var request1 = new RestRequest(method);
                if (string.IsNullOrEmpty(body) == false)
                {
                    request1.AddJsonBody(body);
                }

                var resp = client.Execute(request1);

                VDebug.WriteLine(resp.ResponseStatus.ToString());
                if (string.IsNullOrEmpty(resp.Content) == false)
                    VDebug.WriteLine(resp.Content.ToString());

                if (resp.Content != null && string.IsNullOrEmpty(resp.Content) == false)
                {
                    VDebug.WriteLine("Responce:");

                    Responce responce = (Responce)resp.Content.ToObject(typeof(Responce));
                    if (responce != null)
                    {
                        VDebug.WriteLine("state: " + responce.State);
                        if (responce.Reason != null)
                        {
                            VDebug.WriteLine("reason: " + responce.Reason);
                            if (responce.State != ResponceStateCode.Success)
                                ClientCommandHandler.ServerErrorInvoke(responce.State + " " + responce.Reason);
                        }

                        var jsonRoom = responce.Reason ?? "";

                        roomInfo = jsonRoom!.ToObject<RoomInfo>();

                        if (roomInfo != null)
                        {
                            currentUser.RoomName = roomInfo.Name ?? "";
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        ClientCommandHandler.ServerErrorInvoke("Responce is empty");
                        return false;
                    }
                }
                else
                {
                    VDebug.WriteLine("Error:");
                    VDebug.WriteLine("StatusCode: " + resp.StatusCode);

                    if (resp.ErrorMessage != null)
                    {
                        VDebug.WriteLine("ErrorMessage: " + resp.ErrorMessage);
                        VDebug.WriteLine("ResponseStatus: " + resp.ResponseStatus);
                    }

                    ClientCommandHandler.ServerErrorInvoke(resp.StatusCode + "\n" + resp.ErrorMessage ?? "");
                    return false;
                }
            }
            catch (Exception ex)
            {
                VDebug.WriteLine(ex.ToMessageString());
                ErrorEvent?.Invoke(null, EventArgs.Empty);
                return false;
            }
        }

        public async Task<string> TrySaveSubtitle(string fileName, string subtitlesDirectoryPath)
        {
            var client = new RestClient($"{baseUrl}/subtitles/{fileName}txt");
            var request1 = new RestRequest(Method.GET);

            var resp = await client.ExecuteAsync(request1);
            if (Directory.Exists(subtitlesDirectoryPath) == false)
            {
                Directory.CreateDirectory(subtitlesDirectoryPath);
            }
            if (string.IsNullOrEmpty(resp.Content) == false)
            {
                try
                {
                    File.Create(@$"{subtitlesDirectoryPath}\{fileName}srt").Close();
                    var file = File.WriteAllTextAsync(@$"{subtitlesDirectoryPath}\{fileName}srt", resp.Content);
                }
                catch
                {

                }

            }
            return fileName;
        }
        public List<RoomInfo> GetRooms()
        {
            var client = new RestClient($"{baseUrl}/api/rooms");
            var request1 = new RestRequest(Method.GET);

            var resp = client.Execute(request1);
            if (string.IsNullOrEmpty(resp.Content) == false)
            {
                return resp.Content.ToObject<List<RoomInfo>>()??new List<RoomInfo>();
            }
            else
            {
                return new List<RoomInfo>();
            }
        }
        public List<URLModel> GetFiles()
        {
            var client = new RestClient($"{baseUrl}/api/files");
            var request1 = new RestRequest(Method.GET);

            var resp = client.Execute(request1);
            if (string.IsNullOrEmpty(resp.Content) == false)
            {
                return resp.Content.ToObject<List<URLModel>>()??new List<URLModel>();
            }
            else
            {
                return new List<URLModel>();
            }
        }
    }
}
