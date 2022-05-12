using ClassLibrary;
using ClassLibrary.Models;
using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DudesPlayer.Views.SideBar
{
    /// <summary>
    /// Логика взаимодействия для UserItem.xaml
    /// </summary>
    public partial class UserItem : UserControl
    {
        UserModel User { get; set; }
        public UserItem(UserModel user)
        {
            InitializeComponent();
            User = user;
            UserNameTB.Text = user.Username;
            Uri uri = new Uri("pack://application:,,,/Avatars/" + user.AvatarId + ".jpg");
            VDebug.WriteLine(uri.ToString());
            BitmapImage bi = null;
            try
            {
                bi = new BitmapImage(uri);

            }
            catch (Exception ex)
            {
                VDebug.WriteLine(ex.ToMessageString());
            }
            if (bi != null)
            {
                bi.Freeze();
                UserImage.ImageSource = bi;
            }
        }

        public UserItem()
        {
            InitializeComponent();
            UserNameTB.Text = "test";
            Uri uri = new Uri("pack://application:,,,/Avatars/5.jpg");
            VDebug.WriteLine(uri.ToString());
            BitmapImage bi = null;
            try
            {
                bi = new BitmapImage(uri);

            }
            catch (Exception ex)
            {
                VDebug.WriteLine(ex.ToMessageString());
            }
            if (bi != null)
            {
                bi.Freeze();
                UserImage.ImageSource = bi;
            }

        }
    }
}
