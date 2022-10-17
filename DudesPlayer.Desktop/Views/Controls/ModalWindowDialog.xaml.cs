using DudesPlayer.Desktop.Views.Pages.RoomsExplorerPage;
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

namespace DudesPlayer.Desktop.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для Dialog.xaml
    /// </summary>
    public partial class ModalWindowDialog : UserControl, IModalWindowDialog
    {
        private IModalWindow? _modalWindow;

        public ModalWindowDialog()
        {
            InitializeComponent();
        }

        public bool IsOpen => Visibility == Visibility.Visible;

        public void Close()
        {
            Visibility = Visibility.Collapsed;
            RootCard.Content = null;
            _modalWindow = null;
        }

        public void SetModal(IModalWindow page)
        {
            Visibility = Visibility.Visible;
            _modalWindow = page;
            SaveButton.Content = page.SaveButtonTitle;
            RootCard.Content = page;
            Title.Text = page.Title;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _modalWindow?.OnCloseButtonClicked();
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _modalWindow?.OnSaveButtonClicked();
            Close();
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _modalWindow?.OnCloseButtonClicked();
            Close();
        }
    }
}
