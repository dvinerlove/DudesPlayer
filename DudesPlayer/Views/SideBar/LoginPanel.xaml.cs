using ClassLibrary.Models;
using DudesPlayer.Classes;
using DudesPlayer.Classes.Client;
using DudesPlayer.Client;
using RandomFriendlyNameGenerator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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

namespace DudesPlayer.Views.SideBar
{
    /// <summary>
    /// Логика взаимодействия для LoginPanel.xaml
    /// </summary>
    public partial class LoginPanel : UserControl
    {
        public LoginPanel()
        {
            InitializeComponent();
            LoginStack.Height = 0;
            LoginHeader.Height = 0;
            SetRandomAvatar();
            SetRandomName();
            Host_Checked_1(null, new RoutedEventArgs());
            ClientCommandHandler.OpenRoomEvent += ClientCommandHandler_OpenRoomEvent;
        }

        private void ClientCommandHandler_OpenRoomEvent(object sender, EventArgs e)
        {
            JoinBtn.IsChecked = true;
            RoomName.Text = sender.ToString();
            Connect_Checked(sender, new RoutedEventArgs());
        }

        private void SetRandomName()
        {
            UsernameTB.Text = RandomFriendlyNameGenerator.NameGenerator.Identifiers.Get(50,
                IdentifierComponents.Adjective |
                IdentifierComponents.Noun |
                IdentifierComponents.Animal,
                separator: "_", lengthRestriction: 18).ToList()[4];
        }
        public async void Hide()
        {
            DoubleAnimation animShowHide = new DoubleAnimation();
            animShowHide.FillBehavior = FillBehavior.HoldEnd;
            animShowHide.DecelerationRatio = 0.8;
            animShowHide.AccelerationRatio = 0.2;
            animShowHide.Duration = new Duration(TimeSpan.FromSeconds(0.1));
            animShowHide.From = LoginHeader.ActualHeight;
            animShowHide.To = 0;
            LoginHeader.BeginAnimation(Window.HeightProperty, animShowHide);
            await Task.Delay(100);
            animShowHide.From = LoginStack.ActualHeight;
            animShowHide.To = 0;
            animShowHide.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            LoginStack.BeginAnimation(Window.HeightProperty, animShowHide);
        }

        public async void Show()
        {
            DoubleAnimation animShowHide = new DoubleAnimation();
            animShowHide.FillBehavior = FillBehavior.HoldEnd;
            animShowHide.DecelerationRatio = 0.8;
            animShowHide.AccelerationRatio = 0.2;
            animShowHide.Duration = new Duration(TimeSpan.FromSeconds(0.1));
            animShowHide.From = LoginHeader.ActualHeight;
            animShowHide.To = 48;
            LoginHeader.BeginAnimation(Window.HeightProperty, animShowHide);
            await Task.Delay(100);
            animShowHide.From = LoginStack.ActualHeight;
            animShowHide.To = 380;
            animShowHide.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            LoginStack.BeginAnimation(Window.HeightProperty, animShowHide);
        }

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void Connect_Checked(object sender, RoutedEventArgs e)
        {
            if (StartBtn != null)
            {
                RoomName.Visibility = Visibility.Visible;
                StartBtn.Content = "Join room";
                RoomName.IsEnabled = true;
            }
        }

        private void Host_Checked_1(object sender, RoutedEventArgs e)
        {
            if (StartBtn != null)
            {
                RoomName.Visibility = Visibility.Collapsed;
                StartBtn.Content = "Create room";
                RoomName.IsEnabled = false;
            }
        }
        Random Random = new Random();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetRandomAvatar();
            SetRandomName();
        }
        void SetRandomAvatar()
        {
            CurrentIconID = Random.Next(2, 120);
            Uri uri = new Uri("pack://application:,,,/Avatars/" + CurrentIconID + ".jpg");
            BitmapImage bi = new BitmapImage(uri);
            AvatarImage.ImageSource = bi;
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(UsernameTB.Text))
            {
                StartBtn.IsEnabled = false;
                ChoosingPanel.IsEnabled = false;
                SeverComboBox.IsEnabled = false;

                bool isHostChecked = Host.IsChecked.Value;

                UserModel token = new UserModel()
                {
                    RoomName = RoomName.Text,
                    Username = UsernameTB.Text,
                    AvatarId = CurrentIconID.ToString(),
                };

                ClientData.CurrentUser = token;
                bool isConnected = false;
                Task.Factory.StartNew(() =>
                {

                    if (isHostChecked == true)
                    {
                        if (ClientData.Client.CreateRoom())
                        {
                            isConnected = ClientData.Client.Login();
                        }
                    }
                    else
                    {
                        isConnected = ClientData.Client.Login();
                    }

                    Dispatcher.Invoke(() =>
                    {
                        StartBtn.IsEnabled = true;
                        ChoosingPanel.IsEnabled = true;
                        SeverComboBox.IsEnabled = true;
                        if (isConnected == true)
                        {
                            ClientCommandHandler.LoginInvoke();
                        }
                    });

                });
            }
        }

        public int CurrentIconID { get; private set; } = 5;

        private void UsernameTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            (App.Current.MainWindow as MainWindow).IsInputFocused = true;
        }

        private void UsernameTB_GotFocus(object sender, RoutedEventArgs e)
        {
            (App.Current.MainWindow as MainWindow).IsInputFocused = true;
        }
    }
}
