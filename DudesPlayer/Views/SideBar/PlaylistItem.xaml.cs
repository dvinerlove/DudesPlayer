using ClassLibrary.Models;
using DudesPlayer.Classes;
using DudesPlayer.Classes.Client;
using DudesPlayer.Models.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace DudesPlayer.Views.SideBar
{
    /// <summary>
    /// Логика взаимодействия для PlaylistItem.xaml
    /// </summary>
    public partial class PlaylistItem : UserControl
    {
        private URLModel url;

        public SideMenu Menu { get; internal set; }

        public PlaylistItem(URLModel item)
        {
            InitializeComponent();
            url = item;
            if (string.IsNullOrEmpty(url.Name))
            {
                Title.Text = item.Url.Split('\"').Last().Split('/').Last().Split('\\').Last();
            }
            else
            {
                Title.Text = url.Name;
            }
            this.ToolTip = Title.Text;
            var currentURL = ClientData.Room.Settings.CurrentURL;

            if (currentURL != null && url.Id == currentURL.Id/* && url.Name == currentURL.Name && url.Url == currentURL.Url*/)
            {
                PanelButton.Foreground = new SolidColorBrush(Colors.White);
                PanelButton.FontWeight = FontWeights.Black;
            }
            ClientCommandHandler.Set += ClientCommandHandler_Set;

            ContextMenu = new ContextMenu();
            MenuItem link = new MenuItem();
            link.Header = "Open source";
            link.Click += Link_Click;
            ContextMenu.Items.Add(link);
        }

        private void Link_Click(object sender, RoutedEventArgs e)
        {
            string url;
            if (string.IsNullOrEmpty(this.url.YTSource) == false)
            {
                url = $"{this.url.YTSource}";
            }
            else
            {
                url = this.url.Url;
            }
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                }
            }
        }

        private void ClientCommandHandler_Set(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {

                var currentURL = (URLModel)sender;
                if (url.Id == currentURL.Id && url.Url == currentURL.Url)
                {
                    PanelButton.Foreground = new SolidColorBrush(Colors.White);
                    PanelButton.FontWeight = FontWeights.Black;
                }
                else
                {
                    PanelButton.FontWeight = FontWeights.Normal;
                    PanelButton.Foreground = template.Foreground;
                }
            });

        }

        private void PanelButton_Click(object sender, RoutedEventArgs e)
        {
            Menu.TimeOut();
            Task.Factory.StartNew(() =>
            {
                ClientData.Client.SetCurrentVideo(url);
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                ClientData.Client.DeleteUrl(url);
            });

        }
    }
}
