using DudesPlayer.Library.Models;
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

namespace DudesPlayer.Views.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для PlaylistDialogItem.xaml
    /// </summary>
    public partial class PlaylistDialogItem : UserControl
    {
        public event EventHandler Click;
        public PlaylistDialogItem()
        {
            InitializeComponent();
        }
        URLModel urlModel;
        public PlaylistDialogItem(URLModel url)
        {
            InitializeComponent();
            urlModel = url;
            urlModel.YTSource = "";
            Preview.Visibility = Visibility.Collapsed;
            Title.Text = url.GetTitle();
            Author.Text = url.Url;
        }
        public URLModel GetURL()
        {
            return urlModel;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e);
        }
    }
}
