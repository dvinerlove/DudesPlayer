using ControlzEx.Theming;
using DudesPlayer.Classes;
using DudesPlayer.Classes.Client;
using DudesPlayer.Models.Client;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WPFUI.Common;
//using System.Windows.Media;
//using System.Windows.Media.Animation;
namespace DudesPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isMenuShow = true;

        public MainWindow()
        {
            InitializeComponent();


            Loaded += (sender, args) =>
            {
                WPFUI.Appearance.Watcher.Watch(this);
                WPFUI.Appearance.Theme.Set(WPFUI.Appearance.ThemeType.Dark);
                WPFUI.Appearance.Background.ApplyDarkMode(this);
            };

            SetTheme();
            MenuToggle();

            PublicDalogHost.Loaded += PublicDalogHost_Loaded;
            PlayerWindow.MainWindow = this;
            StateChanged += MainWindow_StateChanged;
            Closed += MainWindow_Closed;
            Closing += MainWindow_Closing;


            if (string.IsNullOrEmpty(Properties.Settings.Default.LibDirectory))
            {
                Properties.Settings.Default.LibDirectory = "vlc.zip";
            }

            if (string.IsNullOrEmpty(Properties.Settings.Default.SubsDirectory))
            {
                Properties.Settings.Default.SubsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Normalize() + @"\DudesPlayer\downloads";
            }

            VDebug.DebugEvent += VisualDebug_DebugEvent;
            ClientCommandHandler.Disconnect += ClientCommandHandler_Disconnect;
            ClientCommandHandler.ServerError += ClientCommandHandler_ServerError;
            ClientCommandHandler.LogoutResponceEvent += ClientCommandHandler_Disconnect;
            ClientCommandHandler.LoginResponceEvent += ClientCommandHandler_LoginResponceEvent;

            PlayerWindow.SideMenu = sideMenu1;
        }

        private void ClientCommandHandler_LoginResponceEvent(object sender, EventArgs e)
        {
            PlayerWindow.Visibility = Visibility.Visible;
            browser.Visibility = Visibility.Collapsed;
        }

        private void ClientCommandHandler_Disconnect(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                //RootSnackbar.Title = ClientData.CurrentUser.Username;
                //RootSnackbar.Message = $"Disconnected from room ({ClientData.CurrentUser.RoomName})";
                RootSnackbar.Show(ClientData.CurrentUser.Username, $"Disconnected from room ({ClientData.CurrentUser.RoomName})");
                PlayerWindow.Visibility = Visibility.Collapsed;
                browser.Visibility = Visibility.Visible;
            });
        }

        private void ClientCommandHandler_ServerError(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                //RootSnackbar.Title = "Error";
                //RootSnackbar.Message = $"{sender.ToString()}";
                RootSnackbar.Show("Error", $"{sender.ToString()}");
                //PlayerWindow.Visibility = Visibility.Collapsed;
                //browser.Visibility = Visibility.Visible;
            });
        }

        private void VisualDebug_DebugEvent(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (string.IsNullOrEmpty(sender.ToString()))
                {
                    return;
                }
                if (isDebugMode)
                {
                    var textBlock = new TextBlock() { Margin = new Thickness(8, 4, 8, 4), Text = sender.ToString(), TextWrapping = TextWrapping.Wrap };
                    DebugStack.Children.Insert(0, textBlock);
                    DebugStack.Children.Insert(0, new Separator());
                    textBlock.Focus();
                }
            });
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ClientData.Client.Disconnect();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            ClientData.Client.Disconnect();
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {

            if (WindowState == WindowState.Maximized)
            {
                btnMaximize.Visibility = Visibility.Collapsed;
                btnRestore.Visibility = Visibility.Visible;
                ResizeMode = ResizeMode.NoResize;
            }
            else
            {
                btnMaximize.Visibility = Visibility.Visible;
                btnRestore.Visibility = Visibility.Collapsed;
                ResizeMode = ResizeMode.CanResize;
            }
        }

        private void PublicDalogHost_Loaded(object sender, RoutedEventArgs e)
        {
            PlayerWindow.Startup();
        }

        private static void SetTheme()
        {
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
            ThemeManager.Current.SyncTheme();
            ThemeManager.Current.ChangeTheme(Application.Current, "Dark.Yellow");
            var theme = ThemeManager.Current.DetectTheme(Application.Current);
            var inverseTheme = ThemeManager.Current.GetInverseTheme(theme);
            ThemeManager.Current.AddTheme(RuntimeThemeGenerator.Current.GenerateRuntimeTheme(inverseTheme.BaseColorScheme, Color.FromRgb(64, 64, 64)));
            ThemeManager.Current.ChangeTheme(Application.Current, ThemeManager.Current.AddTheme(RuntimeThemeGenerator.Current.GenerateRuntimeTheme(theme.BaseColorScheme, Color.FromRgb(64, 64, 64))));
            Application.Current?.MainWindow?.Activate();
            ModifyTheme(theme => theme.SetSecondaryColor(Color.FromRgb(00, 00, 00)));
            try
            {
                var paletteHelper = new PaletteHelper();
                ITheme theme1 = paletteHelper.GetTheme();

                theme1.SetBaseTheme(MaterialDesignThemes.Wpf.Theme.Dark);
                theme1.SetPrimaryColor(Colors.White);

                paletteHelper.SetTheme(theme1);
            }
            catch 
            {

            }
            
        }

        private static void ModifyTheme(Action<ITheme> modificationAction)
        {
            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            modificationAction?.Invoke(theme);
            paletteHelper.SetTheme(theme);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
        }


        private void MunuBtn_Click(object sender, RoutedEventArgs e)
        {
            MenuToggle();
        }

        public void MenuToggle()
        {
            if (isMenuShow)
            {
                ShowMenu();
            }
            else
            {
                HideMenu();
            }
            isMenuShow = !isMenuShow;
        }

        void ShowMenu()
        {
            var sb = new Storyboard();
            var ta = new ThicknessAnimation();
            ta.BeginTime = new TimeSpan(0);
            ta.SetValue(Storyboard.TargetNameProperty, "sideMenu1");
            Storyboard.SetTargetProperty(ta, new PropertyPath(MarginProperty));
            ta.DecelerationRatio = 0.8;
            ta.AccelerationRatio = 0.2;
            ta.Duration = new Duration(TimeSpan.FromSeconds(0.25));
            ta.From = sideMenu1.Margin;
            ta.To = new Thickness(0, 0, 0, 0);
            sb.Children.Add(ta);
            sb.Begin(this);
            MainAppBar.Height = new GridLength(32);
        }

        public void HideMenu()
        {
            var sb = new Storyboard();
            var ta = new ThicknessAnimation();
            ta.BeginTime = new TimeSpan(0);
            ta.SetValue(Storyboard.TargetNameProperty, "sideMenu1");
            Storyboard.SetTargetProperty(ta, new PropertyPath(MarginProperty));
            ta.DecelerationRatio = 0.8;
            ta.AccelerationRatio = 0.2;
            ta.Duration = new Duration(TimeSpan.FromSeconds(0.25));
            ta.From = sideMenu1.Margin;
            ta.To = new Thickness(0, 0, -300, 0);
            sb.Children.Add(ta);
            sb.Begin(this);
            MainAppBar.Height = new GridLength(0);
            isMenuShow = false;
        }

        private bool isEscKeyUp = true;
        private bool isFKeyUp = true;
        public bool IsInputFocused = false;
        private bool isDebugMode;
        private bool isSpaceKeyUp;

        private void MetroWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F)
            {
                isFKeyUp = true;
            }
            if (e.Key == Key.Escape)
            {
                isEscKeyUp = true;
            }
            if (e.Key == Key.Tab)
            {
            }
            if (e.Key == Key.Space)
            {
                isSpaceKeyUp = true;
            }
        }
        public void ToggleFullScreen()
        {
            if (WindowState != WindowState.Maximized)
            {
                //this.Visibility = Visibility.Collapsed;
                //WindowStyle = WindowStyle.None;
                this.ResizeMode = ResizeMode.NoResize;
                WindowState = WindowState.Maximized;
                //this.Visibility = Visibility.Visible;
            }
            else
            {
                //WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Normal;
                this.ResizeMode = ResizeMode.CanResize;

            }
        }
        private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsLoaded == false)
            {
                return;
            }
            if (e.Key == Key.F8)
            {
                if (DebugCard.Visibility == Visibility.Visible)
                {
                    DebugCard.Visibility = Visibility.Collapsed;
                    isDebugMode = true;
                }
                else
                {
                    DebugCard.Visibility = Visibility.Visible;
                }
                return;
            }



            if (PlayerWindow.PlaylistDialog.Visibility != Visibility.Visible &&
                PlayerWindow.ChatDialog.Visibility != Visibility.Visible &&
                sideMenu1.DialogPanel.Children.Count == 0)
            {
                if (e.Key == Key.F)
                {
                    if (isFKeyUp)
                    {
                        if (sideMenu1.loginPanel1.UsernameTB.IsFocused == false)
                        {
                            ToggleFullScreen();
                        }
                    }
                    isFKeyUp = false;
                    return;
                }

                if (e.Key == Key.Space)
                {
                    if (isSpaceKeyUp)
                    {
                        PlayerWindow.PlayToggle();
                    }
                    isSpaceKeyUp = false;
                    return;
                }

            }





            if (e.Key == Key.Tab)
            {
                e.Handled = true;
            }
            if (e.Key == Key.Escape)
            {
                if (isEscKeyUp)
                {
                    if (DialogHost.IsDialogOpen(null))
                    {
                        DialogHost.Close(null);
                    }
                    else
                    {
                        MenuToggle();
                    }
                }
                isEscKeyUp = false;
                return;
            }
            IsInputFocused = false;
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
        }

        private void MetroWindow_StateChanged(object sender, EventArgs e)
        {
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
