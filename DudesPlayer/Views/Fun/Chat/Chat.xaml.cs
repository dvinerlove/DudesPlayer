using DudesPlayer.Classes;
using DudesPlayer.Classes.Client;
using DudesPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DudesPlayer.Views.Fun.Chat
{
    /// <summary>
    /// Логика взаимодействия для Chat.xaml
    /// </summary>
    public partial class Chat : UserControl
    {
        public Chat()
        {
            InitializeComponent();

            ClientCommandHandler.ChatMessageEvent -= ClientCommandHandler_ChatMessageEvent;
            ClientCommandHandler.LoginResponceEvent -= ClientCommandHandler_LoginResponceEvent;
            ClientCommandHandler.Disconnect -= ClientCommandHandler_LoginResponceEvent;

            ClientCommandHandler.ChatMessageEvent += ClientCommandHandler_ChatMessageEvent;
            ClientCommandHandler.LoginResponceEvent += ClientCommandHandler_LoginResponceEvent;
            ClientCommandHandler.Disconnect += ClientCommandHandler_LoginResponceEvent;
        }

        private void ClientCommandHandler_LoginResponceEvent(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    ChatStack.Children.Clear();
                    Scroll.ScrollToBottom();
                });
            });
        }

        private void ClientCommandHandler_ChatMessageEvent(string user, string message)
        {
            if (ClientData.CurrentUser.Username != user && Visibility != Visibility.Visible)
            {
                Task.Factory.StartNew(() =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        SoundHandler.Play(SoundType.chat);
                    });
                });
            }

            Task.Factory.StartNew(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    ChatItem chatItem = new ChatItem(user, message);

                    ChatStack.Children.Add(chatItem);
                    Scroll.ScrollToBottom();
                });
            });
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        bool isReturnUp = true;
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (isReturnUp)
            {

                if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)||
                    e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift))
                {
                    switch (e.Key)
                    {
                        case Key.Return:
                            TextBox t = sender as TextBox;
                            t.Text += Environment.NewLine;
                            t.CaretIndex = t.Text.Length - 1;
                            isReturnUp = false;
                            break;
                    }
                }
                else
                {
                    if (e.Key == Key.Return)
                    {
                        Submit();
                        isReturnUp = false;
                        e.Handled = true;
                    }
                }

            }



        }

        private void Submit()
        {
            var text = MessageText.Text.Trim();
            MessageText.Text = "";
            if (string.IsNullOrEmpty(text) == false)
            {
                Task.Factory.StartNew(() =>
                {
                    ClientData.Client.SendChatMessage(text);
                    Dispatcher.Invoke(() =>
                    {
                    });
                });
            }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Return:
                    isReturnUp = true;
                    break;
            }
        }
    }
}
