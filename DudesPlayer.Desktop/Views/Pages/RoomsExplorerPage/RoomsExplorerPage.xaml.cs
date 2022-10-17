using DudesPlayer.Desktop.Views.Controls;
using DudesPlayer.Desktop.Views.Pages.RoomsExplorerPage.Dialogs;
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
using Wpf.Ui.Mvvm.Contracts;

namespace DudesPlayer.Desktop.Views.Pages.RoomsExplorerPage
{
    /// <summary>
    /// Логика взаимодействия для RoomsExplorerPage.xaml
    /// </summary>
    public partial class RoomsExplorerPage : UserControl
    {
        private IPageService _pageService;
        private IModalWindowService _modalWindowService;

        public RoomsExplorerPage(IPageService pageService, IModalWindowService modalWindowService)
        {
            InitializeComponent();
            _pageService = pageService;
            _modalWindowService = modalWindowService;
        }

        private void NewRoomButton_Click(object sender, EventArgs e)
        {

            _modalWindowService.ShowModal(typeof(NewRoomDialog));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _modalWindowService.ShowModal(typeof(ConnectToRoomDialog));

        }

        private void RoomItem_Click(object sender, EventArgs e)
        {
            var dialog = _pageService.GetPage<ConnectToRoomDialog>();
            if (dialog != null)
            {
                dialog.RoomName.Text = "gaynigger";
                _modalWindowService.ShowModal(dialog);
            }

        }
    }
}
