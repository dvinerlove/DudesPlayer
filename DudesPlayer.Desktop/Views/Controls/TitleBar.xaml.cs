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
using Wpf.Ui.Controls;

namespace DudesPlayer.Desktop.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для TitleBar.xaml
    /// </summary>
    public partial class TitleBar : UserControl
    {
        UiWindow window;
        public TitleBar()
        {
            InitializeComponent();
            Loaded += TitleBar_Loaded;
        }

        private void TitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            window = (App.Current.MainWindow as UiWindow)!;
            window.StateChanged += Window_StateChanged;
        }

        private void Window_StateChanged(object? sender, EventArgs e)
        {
            if (window.WindowState == WindowState.Maximized)
            {
                btnMaximize.Visibility = Visibility.Collapsed;
                btnRestore.Visibility = Visibility.Visible;
                window.ResizeMode = ResizeMode.NoResize;
            }
            else
            {
                btnMaximize.Visibility = Visibility.Visible;
                btnRestore.Visibility = Visibility.Collapsed;
                window.ResizeMode = ResizeMode.CanResize;
            }
        }
        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            ToggleFullScreen();
        }

        private void ToggleFullScreen()
        {
            if (window.WindowState != WindowState.Maximized)
            {
                //window.ResizeMode = ResizeMode.NoResize;
                window.WindowState = WindowState.Maximized;
            }
            else
            {
                window.WindowState = WindowState.Normal;
                //window.ResizeMode = ResizeMode.CanResize;
            }
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            window.WindowState = WindowState.Normal;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            window.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            window.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //window.DragMove();
        }

        private void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ToggleFullScreen();
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton ==  MouseButtonState.Pressed)
            {
                window.DragMove();
            }
        }
    }
}
