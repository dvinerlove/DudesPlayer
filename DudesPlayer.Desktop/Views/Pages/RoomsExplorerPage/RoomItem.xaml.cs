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

namespace DudesPlayer.Desktop.Views.Pages.RoomsExplorerPage
{
    /// <summary>
    /// Логика взаимодействия для RoomItem.xaml
    /// </summary>
    public partial class RoomItem : UserControl
    {
        public RoomItem()
        {
            InitializeComponent();
        }
        public event EventHandler? Click;
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e);
        }
    }
}
