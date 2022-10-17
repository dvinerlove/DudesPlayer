using DudesPlayer.Library;
using DudesPlayer.Library.Models;
using DudesPlayer.Classes;
using DudesPlayer.Views.SideBar;
using DudesPlayer.Views.SideBar.Dialogs;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Vlc.DotNet.Wpf;

namespace DudesPlayer.Views
{
    /// <summary>
    /// Логика взаимодействия для SideMenu.xaml
    /// </summary>
    public partial class SideMenu : UserControl
    {
        public SideMenu()
        {
            InitializeComponent();
            loginPanel1.Show();
            ClientCommandHandler.Update += (ss, ee) => { Update(); };
            ClientCommandHandler.LoginResponceEvent += (ss, ee) => { OpenUserPanel(); };
            ClientCommandHandler.Disconnect += (ss, ee) =>
            {
                Dispatcher.Invoke(() =>
                {
                    CloseUserPanel();
                });
            };
        }

        private void Update()
        {
            Dispatcher.Invoke(() =>
            {
                var room = ClientData.Client.GetRoom();
                if (room == null || room.Name == null || room.GetUsers() == null || room.Settings == null || room.UsersCount() == 0)
                {
                    return;
                }
                UsersPanel.Children.Clear();
                List<UserModel> users = room.GetUsers();
                VDebug.WriteLine("room: " + room.ToJson());
                HeaderText.Text = $"ROOM {room.Name}";
                VDebug.WriteLine("users lenght: " + users.Count());
                foreach (UserModel item in users)
                {
                    UsersPanel.Children.Add(new UserItem(item));
                }

                PlaylistStack.Children.Clear();
                var urls = room.GetURls();
                VDebug.WriteLine("urls lenght: " + urls.Count());
                foreach (var item in urls)
                {
                    PlaylistStack.Children.Add(new PlaylistItem(item) { Menu = this });
                }
            });

        }

        private void OpenUserPanel()
        {
            Update();
            DoubleAnimation animation = new DoubleAnimation();
            animation.FillBehavior = FillBehavior.HoldEnd;
            animation.DecelerationRatio = 0.8;
            animation.AccelerationRatio = 0.2;
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.1));
            animation.From = UserPanel.ActualWidth;
            animation.To = 300;
            UserPanel.BeginAnimation(Window.WidthProperty, animation);
        }

        private void CloseUserPanel()
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.FillBehavior = FillBehavior.HoldEnd;
            animation.DecelerationRatio = 0.8;
            animation.AccelerationRatio = 0.2;
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.1));
            animation.From = UserPanel.ActualWidth;
            animation.To = 0;
            UserPanel.BeginAnimation(Window.WidthProperty, animation);
        }

        private void ClientCommandHandler_PlaylistUpdate(object sender, EventArgs e)
        {
            PlaylistStack.Children.Clear();
            foreach (var item in (sender as List<URLModel>))
            {
                PlaylistStack.Children.Add(new PlaylistItem(item) { Menu = this });
            }
        }

        private void ClientCommandHandler_UsersUpdate(object sender, EventArgs e)
        {

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }


        private void loginPanel1_Submit(object sender, EventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).PlayerWindow.PlaylistToggle();
        }

        private async void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            await DialogHost.Show(new SettingsDialog());
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {

            SSEEvent sse = new SSEEvent() { Event = PacketType.Logout, Data = "bye" };
            ClientData.SocketClient.Send(sse.ToJson());
            ClientCommandHandler.DisconnectInvoke("");
            ClientData.Client.Disconnect();
        }

        private void Demotivator_Click(object sender, RoutedEventArgs e)
        {
            if (DialogPanel.Children.Count > 0)
            {
                DialogPanel.Children.Clear();

            }
            else
            {
                DialogPanel.Children.Add(new DudesPlayer.Views.SideBar.Dialogs.JokeDialog());
            }
        }

        private void OnlineHeader_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(ClientData.CurrentUser.RoomName);
            //(Application.Current.MainWindow as MainWindow).RootSnackbar.Title = HeaderText.Text;
            //(Application.Current.MainWindow as MainWindow).RootSnackbar.Message = "Copied to clipboard";
            (Application.Current.MainWindow as MainWindow).RootSnackbar.Show(HeaderText.Text, "Copied to clipboard");
        }

        private void Topmost_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).Topmost = !(Application.Current.MainWindow as MainWindow).Topmost;
            if ((Application.Current.MainWindow as MainWindow).Topmost)
            {
                TopMostIcon.Kind = PackIconKind.PinOff;
            }
            else
            {
                TopMostIcon.Kind = PackIconKind.Pin;
            }
        }
        private void Background_Click(object sender, RoutedEventArgs e)
        {
            if ((Application.Current.MainWindow as MainWindow).PlayerWindow.BackgroundVideo.Opacity != 0)
            {
                (Application.Current.MainWindow as MainWindow).PlayerWindow.BackgroundVideo.Opacity = 0;
                (Application.Current.MainWindow as MainWindow).PlayerWindow.DarkBack.Opacity = 1;

                (Application.Current.MainWindow as MainWindow).PlayerWindow.BackgroundVideo.Source = null;

                BackgroundIcon.Kind = PackIconKind.CardOff;
            }
            else
            {

                (Application.Current.MainWindow as MainWindow).PlayerWindow.BackgroundVideo.Opacity = 1;
                (Application.Current.MainWindow as MainWindow).PlayerWindow.DarkBack.Opacity = 0;

                (Application.Current.MainWindow as MainWindow).PlayerWindow.BackgroundVideo.SetBinding(System.Windows.Controls.Image.SourceProperty,
                    new Binding(nameof(VlcVideoSourceProvider.VideoSource)) { Source = (Application.Current.MainWindow as MainWindow).PlayerWindow.sourceProvider });

                BackgroundIcon.Kind = PackIconKind.Card;

            }
        }
        DispatcherTimer timer;
        internal void TimeOut()
        {
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
            Task.Factory.StartNew(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    IsEnabled = false;
                });
            });
        }

        private void Timer_Tick(object sender, EventArgs e)
        {

            Task.Factory.StartNew(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    IsEnabled = true;
                });
            });
            timer.Stop();
        }
    }
}