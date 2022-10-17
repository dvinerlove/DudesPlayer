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
using Wpf.Ui.Mvvm.Contracts;

namespace DudesPlayer.Desktop.Views.Pages.PlayerPage
{
    /// <summary>
    /// Логика взаимодействия для PlayerPage.xaml
    /// </summary>
    public partial class PlayerPage : UserControl
    {
        public PlayerPage(IPageService pageService, IModalWindowService modalWindowService)
        {
            InitializeComponent();
        }
    }
}
