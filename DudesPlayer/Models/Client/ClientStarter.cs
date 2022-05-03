using System;
using System.Diagnostics;
using System.Windows;
using ClassLibrary;
using ClassLibrary.Models;
using RestSharp;
using DudesPlayer.Classes;
using DudesPlayer.Classes.Client;
using DudesPlayer.Models;
using Newtonsoft.Json;
using System.IO;
using DudesPlayer.Models.Client;
using System.Collections.Generic;

namespace DudesPlayer.Client
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
        public event EventHandler ErrorEvent;

        public bool CreateRoom()
        {
            var result = Post(ApiType.rooms, "", "", Method.POST);
            if (ClientData.Room != null && result)
            {
                ClientData.CurrentUser.RoomName = ClientData.Room.Name;
            }
            return result;
        }

        public bool Login()
        {
            var result = Post(ApiType.users, "", ClientData.CurrentUser.ToString(), Method.POST);
            if (result)
            {
                ClientData.SseController = new Models.SseController();
                ClientData.SseController.Run();
                if (ClientData.SocketClient==null)
                {
                    ClientData.SocketClient = new SocketClient();
                }
                else
                {
                    ClientData.SocketClient.Stop();
                    ClientData.SocketClient = new SocketClient();
                }
                ClientData.SocketClient.Start();
            }
            return result;
        }

        internal void SendChatMessage(string messageText)
        {
            if (ClientData.SocketClient != null)
            {
                SSEEvent sse = new SSEEvent() { Event = PacketType.Chat, Data = messageText };
                ClientData.SocketClient.Send(sse.ToJson());
            }
        }

        internal bool AddUrl(URLModel link)
        {
            return Post(ApiType.urls, $"{ClientData.CurrentUser.RoomName}", link.ToJson(), Method.POST);
        }


        internal bool Disconnect()
        {
            if (ClientData.CurrentUser != null)
            {
                return Post(ApiType.users, null, $"{ClientData.CurrentUser}", Method.DELETE);
            }
            else
            {
                return true;
            }
        }
        internal bool DeleteUrl(URLModel link)
        {
            return Post(ApiType.urls, $"{ClientData.CurrentUser.RoomName}", link.ToJson(), Method.DELETE);
        }

        internal void ShakeScreen()
        {
            Post(ApiType.commands, $"{ClientData.CurrentUser}/shake", "", Method.POST);
        }

        internal bool SetCurrentVideo(URLModel link)
        {
            return Post(ApiType.commands, $"{ClientData.CurrentUser}/set", link.ToJson(), Method.POST);
        }
        internal bool CreateJoke(Joke joke)
        {
            var json = joke.setup + "?.?123**" + joke.punchline;
            return Post(ApiType.commands, $"{ClientData.CurrentUser}/joke", json, Method.POST);
        }
        internal bool Play()
        {
            return Post(ApiType.commands, $"{ClientData.CurrentUser}/play", null, Method.POST);
        }
        internal bool Pause()
        {
            return Post(ApiType.commands, $"{ClientData.CurrentUser}/pause", null, Method.POST);
        }
        internal bool SetTime(long value)
        {
            return Post(ApiType.commands, $"{ClientData.CurrentUser}/time", value.ToString(), Method.POST);

        }
        private bool Post(ApiType apiType, string path, string body, Method method)
        {
            try
            {
                var client = new RestClient($"{ClientData.BaseUrl}/api/{apiType}/{path}");
                VDebug.WriteLine($"{ClientData.BaseUrl}/api/{apiType}/{path}");
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
                        var jsonRoom = responce.Reason;
                        RoomInfo roomInfo = jsonRoom.ToObject<RoomInfo>();
                        if (roomInfo != null)
                        {
                            ClientData.Room = roomInfo;
                            return true;
                        }
                        else
                        {
                            //ClientCommandHandler.ServerErrorInvoke($"{apiType} \n{method} \n{path} \n{body} \nroom is null".Trim());
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
                MessageBox.Show(ex.Message);
                ErrorEvent?.Invoke(null, EventArgs.Empty);
                return false;
            }
        }

        internal void TrySaveSubtitle(string fileName)
        {
            var client = new RestClient($"{ClientData.BaseUrl}/subtitles/{fileName}txt");
            var request1 = new RestRequest(Method.GET);

            var resp = client.Execute(request1);
            if (Directory.Exists(Properties.Settings.Default.SubsDirectory) == false)
            {
                Directory.CreateDirectory(Properties.Settings.Default.SubsDirectory);
            }
            if (string.IsNullOrEmpty(resp.Content) == false)
            {
                var file = File.CreateText(@$"{Properties.Settings.Default.SubsDirectory}\{fileName}srt");
                file.Write(resp.Content);
                file.Close();
            }
        }
        internal List<RoomInfo> GetRooms()
        {
            var client = new RestClient($"{ClientData.BaseUrl}/api/rooms");
            var request1 = new RestRequest(Method.GET);

            var resp = client.Execute(request1);
            if (string.IsNullOrEmpty(resp.Content) == false)
            {
                return resp.Content.ToObject<List<RoomInfo>>();
            }
            else
            {
                return new List<RoomInfo>();
            }
        }
        internal List<URLModel> GetFiles()
        {
            var client = new RestClient($"{ClientData.BaseUrl}/api/files");
            var request1 = new RestRequest(Method.GET);

            var resp = client.Execute(request1);
            if (string.IsNullOrEmpty(resp.Content) == false)
            {
                return resp.Content.ToObject<List<URLModel>>();
            }
            else
            {
                return new List<URLModel>();
            }
        }
    }
}
