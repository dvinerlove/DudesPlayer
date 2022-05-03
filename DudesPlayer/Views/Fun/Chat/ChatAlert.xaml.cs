﻿using DudesPlayer.Classes.Client;
using DudesPlayer.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Логика взаимодействия для ChatAlert.xaml
    /// </summary>
    public partial class ChatAlert : UserControl
    {
        public ChatAlert()
        {
            InitializeComponent();
            Task.Factory.StartNew(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    ClientCommandHandler.ChatMessageEvent += Chat;
                });
            });
        }

        private void Chat(string user, string message)
        {
            Task.Factory.StartNew(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    ChatItem chatItem = new ChatItem(user, message);
                    VDebug.WriteLine(Visibility);
                    ChatGrid.Children.Clear();
                    ChatGrid.Children.Add(chatItem);
                    chatItem.Hide();
                });
            });
        }
    }
}