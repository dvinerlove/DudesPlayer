using DudesPlayer.Desktop.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using Wpf.Ui.Controls.Interfaces;

namespace DudesPlayer.Desktop.Views.Pages.HomePage
{
    /// <summary>
    /// Логика взаимодействия для NavigationControl.xaml
    /// </summary>
    public partial class NavigationControl : UserControl, INavControl
    {
        public NavigationControl()
        {
            InitializeComponent();/*
                                    <local:NavigationItem PageType="Frie"
                HorizontalAlignment="Left"
                Symbol="ChatMultiple24"
                Text="Friends" />
            <local:NavigationItem
                HorizontalAlignment="Left"
                Symbol="Settings24"
                Text="Settings" />*/

            ItemsControl.Items.Add(new NavigationItem()
            {
                PageType = typeof(PlayerPage.PlayerPage),
                Text = "Player",
                Symbol = Wpf.Ui.Common.SymbolRegular.Play24
            });
            ItemsControl.Items.Add(new NavigationItem()
            {
                PageType = typeof(RoomsExplorerPage.RoomsExplorerPage),
                Text = "Rooms",
                Symbol = Wpf.Ui.Common.SymbolRegular.Board24
            });
            ItemsControl.Items.Add(new NavigationItem()
            {
                PageType = typeof(FriendsPage.FriendsPage),
                Text = "Friends",
                Symbol = Wpf.Ui.Common.SymbolRegular.ChatMultiple24
            });
            ItemsControl.Items.Add(new NavigationItem()
            {
                Text = "Settings",
                Symbol = Wpf.Ui.Common.SymbolRegular.Settings24
            });
            Loaded += NavigationControl_Loaded;
        }

        public event EventHandler<Type> ItemSelected;

        private void NavigationControl_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in ItemsControl.Items.OfType<IMyNavItem>())
            {
                item.Click -= Item_Click;
                item.Click += Item_Click;
            }
        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            IMyNavItem navigationItem = (IMyNavItem)sender;

            ItemSelected?.Invoke(this, navigationItem.PageType);
        }

        public void SelectItem(Type pageType)
        {
            IMyNavItem navigationItem = ItemsControl.Items.OfType<IMyNavItem>().Where(x => x.PageType == pageType).FirstOrDefault();
            foreach (var item in ItemsControl.Items.OfType<IMyNavItem>())
            {
                item.IsChecked = false;
            }
            navigationItem!.IsChecked = true;
        }
    }
}
