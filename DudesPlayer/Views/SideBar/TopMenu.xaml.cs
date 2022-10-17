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

namespace DudesPlayer.Views.SideBar
{
    /// <summary>
    /// Логика взаимодействия для TopMenu.xaml
    /// </summary>
    public partial class TopMenu : UserControl
    {
        public event EventHandler ChatClick;
        public event EventHandler PlaylistClick;
        public TopMenu()
        {
            InitializeComponent();
        }

        private void Chat_Click(object sender, RoutedEventArgs e)
        {
            ChatClick?.Invoke(sender, e);
        }
        private void Playlist_Click(object sender, RoutedEventArgs e)
        {
            PlaylistClick?.Invoke(sender, e);
        }
    }
}
