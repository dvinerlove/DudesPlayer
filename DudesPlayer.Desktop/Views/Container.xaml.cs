using DudesPlayer.Desktop.Services;
using DudesPlayer.Desktop.Views.Controls;
using DudesPlayer.Desktop.Views.Pages;
using DudesPlayer.Desktop.Views.Pages.FriendsPage;
using DudesPlayer.Desktop.Views.Pages.HomePage;
using HandyControl.Tools;
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
using System.Windows.Shapes;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace DudesPlayer.Desktop.Views
{

    public interface IWindow
    {
        void ShowWindow();

        void CloseWindow();
    }
    /// <summary>
    /// Логика взаимодействия для Container.xaml
    /// </summary>
    public partial class Container : IWindow
    {
        public Container(IPageService pageService, INavService navigationService, IModalWindowService modalWindowDialog)
        {
            InitializeComponent();

            modalWindowDialog.SetPageService(pageService);
            modalWindowDialog.SetModalWindow(RootDialog);

            navigationService.SetNavigationControl(NavigationControl);
            navigationService.SetPageService(pageService);
            navigationService.SetFrame(RootFrame);
            navigationService.SetFullFrame(FullFrame);
            navigationService.Navigate(typeof(FriendsPage));
            navigationService.ShowFullPage(typeof(LoginPage));
            navigationService.Navigated += (s, e) => { };
            navigationService.FullPageShowed += (s, e) => { };
            modalWindowDialog.Close();

            PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    modalWindowDialog.Close();
                }
            };
            ConfigHelper.Instance.SetWindowDefaultStyle();
            Theme.ApplyDarkThemeToWindow(this);
            Watcher.Watch(this);

        }


        #region IWindow methods

        public void ShowWindow()
            => Show();

        public void CloseWindow()
            => Close();

        #endregion IWindow methods

        private void UiWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
