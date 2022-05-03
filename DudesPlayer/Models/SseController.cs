using ClassLibrary;
using ClassLibrary.Models;
using DudesPlayer.Classes;
using DudesPlayer.Classes.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DudesPlayer.Models
{
    class SseController
    {
        private RestClient _client;
        private RestRequest _request;
        Task task;
        bool isRunning = false;
        int currentTask = 0;
        public SseController()
        {
            _client = new RestClient($"{ClientData.BaseUrl}/sse/room/{ClientData.CurrentUser}");
            _request = new RestRequest(Method.GET);
            ClientCommandHandler.Disconnect += ClientCommandHandler_Disconnect;
            task = new Task(StartTask);
        }

        private void ClientCommandHandler_Disconnect(object sender, EventArgs e)
        {
            isRunning = false;
            currentTask = -999;

        }

        public void Run()
        {

            try
            {

                isRunning = true;
                task = new Task(StartTask);
                currentTask = task.Id;
                task.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void StartTask()
        {
            try
            {
                int id = currentTask;
                while (id == currentTask)
                {
                    try
                    {
                        var resp = _client.Execute(_request);

                        SSEEvent sse = (SSEEvent)resp.Content.ToObject(typeof(SSEEvent));
                        if (sse == null)
                        {
                            continue;
                        }
                        else
                        {
                        }


                        switch (sse.Event)
                        {
                            case PacketType.Ping:
                                Models.Client.VDebug.WriteLine("ping)");
                                break;
                            case PacketType.UpdateAll:
                                var obj = sse.Data.ToString().ToObject(typeof(RoomInfo));
                                if (obj != null)
                                {
                                    var newRoom = (RoomInfo)obj;

                                    if (newRoom.UsersCount() < ClientData.Room.UsersCount())
                                    {
                                        SoundHandler.Play(SoundType.logout);
                                    }

                                    ClientData.Room = newRoom;
                                    ClientCommandHandler.UpdateInvoke();
                                    Models.Client.VDebug.WriteLine("SSE ROOM DATA:");
                                    Models.Client.VDebug.WriteLine(ClientData.Room.ToString());
                                }
                                break;
                                //case PacketType.Play:
                                //    ClientCommandHandler.PlayInvoke();
                                //    break;
                                //case PacketType.Set:
                                //    ClientCommandHandler.SetInvoke(sse.Data.ToString().ToObject<URLModel>());
                                //    break;
                                //case PacketType.Pause:
                                //    ClientCommandHandler.PauseInvoke();
                                //    break;
                                //case PacketType.Time:
                                //    ClientCommandHandler.TimeInvoke(sse.Data.ToString());
                                //    break;
                                //case PacketType.Joke:
                                //    ClientCommandHandler.JokeInvoke(sse.Data.ToString());
                                //    break;
                                //case PacketType.Shake:
                                //    ClientCommandHandler.ShakeInvoke();
                                //    break;
                        }
                        Models.Client.VDebug.WriteLine("SSE End...");
                    }
                    catch (Exception ex)
                    {
                        _client = new RestClient($"{ClientData.BaseUrl}/sse/room/{ClientData.CurrentUser}");
                        _request = new RestRequest(Method.GET);
                        Models.Client.VDebug.WriteLine("SSE EXCEPTION:\n" + ex.Message);
                    }
                }

            }
            catch
            {
            }
        }
    }
}
