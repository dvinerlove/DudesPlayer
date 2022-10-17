using DudesPlayer.Library.Models;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DudesPlayer.Views.RoomsBrowser
{
    /// <summary>
    /// Логика взаимодействия для RoomsBrowser.xaml
    /// </summary>
    public partial class RoomsBrowser : UserControl
    {
        public RoomsBrowser()
        {
            InitializeComponent();
            Load();
        }
        void Load()
        {
            RoomStack.Children.Clear();
            Task.Factory.StartNew(() =>
            {
                List<RoomInfo> rooms = ClientData.Client.GetRooms()??new List<RoomInfo>();
                Dispatcher.Invoke(() =>
                {
                    foreach (var item in rooms)
                    {
                        var ri = new RoomItem(item);

                        RoomStack.Children.Add(ri);
                    }
                });
            });
        }
        private void RoomItem_Click(object sender, EventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Load();
        }
    }
}
