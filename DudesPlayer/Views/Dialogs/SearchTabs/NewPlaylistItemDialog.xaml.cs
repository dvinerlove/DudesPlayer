﻿using DudesPlayer.Library;
using DudesPlayer.Library.Models;
using DudesPlayer.Classes;
using DudesPlayer.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
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
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeSearchApi.Net.Models.Youtube;
using YoutubeSearchApi.Net.Services;

namespace DudesPlayer.Views
{
    /// <summary>
    /// Interaction logic for NewPlaylistItemDialog.xaml
    /// </summary>
    public partial class PlaylistDialog : UserControl
    {
        public PlaylistDialog()
        {
            InitializeComponent();
            TextBox1.Focus();
            Loaded += (s, ee) =>
            {
                Load();
            };
        }

        public void Load()
        {
            ServerStack.Children.Clear();
            Task.Factory.StartNew(() =>
            {
                var files = ClientData.Client.GetFiles();
                Dispatcher.Invoke(() =>
                {
                    if (files != null)
                        foreach (var item in files)
                        {
                            item.Url = ClientData.BaseUrl + item.Url.Replace(@"\", @"/");
                            var button = new Button()
                            {
                                Content = item.GetTitle(),
                                Tag = ClientData.BaseUrl + item.Url,
                            };
                            PlaylistDialogItem playlistDialogItem = new PlaylistDialogItem(item);
                            playlistDialogItem.Click += PlaylistDialogItem_Click;
                            ServerStack.Children.Add(playlistDialogItem);
                        }
                });
            });
        }

        private void PlaylistDialogItem_Click(object sender, EventArgs e)
        {
            PlaylistDialogItem playlistDialogItem = (PlaylistDialogItem)sender;
            ClientData.Client.AddUrl(playlistDialogItem.GetURL());
            Close();
        }

        private void Close()
        {
            Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TextBox1.Text.Trim()))
            {
                URLModel uRLModel = new URLModel();
                uRLModel.YTSource = "";
                uRLModel.Url = GetLink();
                ClientData.Client.AddUrl(uRLModel);
                MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, null);
            }

        }

        private string GetLink()
        {
            var str = TextBox1.Text.Trim();
            if (!string.IsNullOrEmpty(str))
            {
                if (!str.StartsWith("http"))
                {
                    str = "http://" + TextBox1.Text;
                }
                if (str.EndsWith("/"))
                {
                    str = str.Substring(0, str.Length - 1);
                }
            }
            return str;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TextBox1.Text.Trim()))
            {
                URLModel uRLModel = new URLModel();
                uRLModel.Url = GetLink();
                uRLModel.YTSource = "";

                ClientData.Client.AddUrl(uRLModel);
                MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, null);
                TextBox1.Text = "";
            }
        }
    }
}
