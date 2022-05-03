using ClassLibrary.Models;
using DudesPlayer.Classes.Client;
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

namespace DudesPlayer.Views.RoomsBrowser
{
    /// <summary>
    /// Логика взаимодействия для RoomItem.xaml
    /// </summary>
    public partial class RoomItem : UserControl
    {
        public event EventHandler Click;
        public RoomInfo RoomInfo;
        public RoomItem()
        {
            InitializeComponent();
        }
        public RoomItem(RoomInfo item)
        {
            InitializeComponent();
            RoomInfo = item;
            RoomName.Text = item.Name;
            Count.Text = item.UsersCount().ToString();
            if (item.Settings != null && item.Settings.CurrentURL != null)
            {
                Url.Text = item.Settings.CurrentURL.GetTitle();
            }
            else
            {
                Url.Text = "";
            }
        }

        private void CardAction_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(RoomInfo, e);
            ClientCommandHandler.OpenRoomInvoke(RoomInfo.Name);
        }
    }
}
