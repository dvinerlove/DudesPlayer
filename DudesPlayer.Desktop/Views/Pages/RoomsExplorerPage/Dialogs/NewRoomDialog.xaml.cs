using DudesPlayer.Desktop.Views.Controls;
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
    /// Логика взаимодействия для NewRoomDialog.xaml
    /// </summary>
    public partial class NewRoomDialog : UserControl, IModalWindow
    {
        private IModalWindowService _modalWindowService;

        public NewRoomDialog(IModalWindowService modalWindowService)
        {
            _modalWindowService = modalWindowService;
            InitializeComponent();
            Title = "Create Room";
            SaveButtonTitle = "Save";
        }

        public string Title { get; }

        public string SaveButtonTitle { get; }

        public void OnCloseButtonClicked()
        {
        }

        public void OnSaveButtonClicked()
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _modalWindowService.Close();
        }
    }
}
