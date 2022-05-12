using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClassLibrary;
using ClassLibrary.Models;

namespace ClassLibrary
{
    public static class ClientCommandHandler
    {
        public static event EventHandler? LogoutResponceEvent;
        public static event EventHandler? Update;
        public static event EventHandler? LoginResponceEvent;
        public static event EventHandler? OpenRoomEvent;
        public static event EventHandler? Play;
        public static event EventHandler? Pause;
        public static event EventHandler? Set;
        public static event EventHandler? Time;
        public static event EventHandler? Joke;
        public static event EventHandler? Shake;
        public static event EventHandler? Disconnect;
        public static event EventHandler? ServerError;

        public delegate void ChatHandler(string user, string message);
        public static event ChatHandler? ChatMessageEvent;

        public static void LoginInvoke()
        {
            LoginResponceEvent?.Invoke(null, EventArgs.Empty);
        }
        public static void ChatMessageInvoke(string user, string message)
        {
            ChatMessageEvent?.Invoke(user, message);
        }
        public static void UpdateInvoke()
        {
            Update?.Invoke(null, EventArgs.Empty);
        }
        public static void DesconnectInvoke()
        {
            LogoutResponceEvent?.Invoke(null, EventArgs.Empty);
        }

        public static void PlayInvoke()
        {
            Play?.Invoke(null, EventArgs.Empty);
        }

        public static void DisconnectInvoke(string v)
        {
            //ClientData.Client.Disconnect();
            Disconnect?.Invoke(null, EventArgs.Empty);
        }

        public static void SetInvoke(URLModel v)
        {
            
                Set?.Invoke(v, EventArgs.Empty);
        }

        public static void TimeInvoke(string v)
        {
            Time?.Invoke(v, EventArgs.Empty);
        }
        public static void JokeInvoke(string v)
        {
            Joke?.Invoke(v, EventArgs.Empty);
        }


        public static void PauseInvoke()
        {
            Pause?.Invoke(null, EventArgs.Empty);
        }

        public static void ServerErrorInvoke(string reason)
        {
            ServerError?.Invoke(reason, EventArgs.Empty);
        }

        public static void OpenRoomInvoke(string reason)
        {
            OpenRoomEvent?.Invoke(reason, EventArgs.Empty);
        }

        public static void ShakeInvoke()
        {
            Shake?.Invoke(null, EventArgs.Empty);
        }
    }
}
