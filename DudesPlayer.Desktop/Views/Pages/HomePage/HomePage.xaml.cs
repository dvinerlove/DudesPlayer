using DudesPlayer.Desktop.Services;
using DudesPlayer.Desktop.Views.Pages.FriendsPage;
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

namespace DudesPlayer.Desktop.Views.Pages.HomePage
{
    /// <summary>
    /// Логика взаимодействия для HomePage.xaml
    /// </summary>
    public partial class HomePage : UserControl
    {
        public HomePage()
        {
        }

        public void Initialize(IPageService pageService, INavService navigationService)
        {
            InitializeComponent();
            navigationService.SetNavigationControl(NavigationControl);
            navigationService.SetPageService(pageService);
            navigationService.SetFrame(RootFrame);

            navigationService.Navigate(typeof(FriendsPage.FriendsPage));
        }
    }
}
