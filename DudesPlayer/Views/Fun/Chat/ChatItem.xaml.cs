using ClassLibrary.Models;
using DudesPlayer.Classes;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DudesPlayer.Views.Fun.Chat
{
    /// <summary>
    /// Логика взаимодействия для ChatItem.xaml
    /// </summary>
    public partial class ChatItem : UserControl
    {
        public ChatItem()
        {
            InitializeComponent();
        }

        public void Hide()
        {
            var animation = new DoubleAnimation
            {
                To = 0,
                BeginTime = TimeSpan.FromSeconds(2),
                Duration = TimeSpan.FromSeconds(1),
                FillBehavior = FillBehavior.Stop
            };

            animation.Completed += (s, a) => this.Opacity = 0;

            this.BeginAnimation(UIElement.OpacityProperty, animation);
        }
        public ChatItem(string username, string message)
        {
            InitializeComponent();

            Username.Text = username;
            Message.Text = message;
            Time.Text = DateTime.Now.ToShortTimeString();
            var user = ClientData.Room.GetUser(username);
            if (user != null)
            {
                Uri uri = new Uri("pack://application:,,,/Avatars/" + user.AvatarId + ".jpg");
                Models.Client.VDebug.WriteLine(uri.ToString());
                BitmapImage bi = null;
                try
                {
                    bi = new BitmapImage(uri);

                }
                catch (Exception ex)
                {
                    Models.Client.VDebug.WriteLine(ex.ToMessageString());
                }
                if (bi != null)
                {
                    bi.Freeze();
                    UserImage.ImageSource = bi;
                }
            }
        }
    }
}
