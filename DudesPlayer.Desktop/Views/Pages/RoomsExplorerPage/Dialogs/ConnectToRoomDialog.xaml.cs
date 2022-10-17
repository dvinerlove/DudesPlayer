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

namespace DudesPlayer.Desktop.Views.Pages.RoomsExplorerPage.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для ConnectToRoomDialog.xaml
    /// </summary>
    public partial class ConnectToRoomDialog : UserControl, IModalWindow
    {
        public ConnectToRoomDialog()
        {
            InitializeComponent();

            Title = "Connect to room";
            SaveButtonTitle = "Connect";
        }

        public string Title { get; }

        public string SaveButtonTitle { get; }

        public void OnCloseButtonClicked()
        {
        }

        public void OnSaveButtonClicked()
        {
        }
    }
}
