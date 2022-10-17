using DudesPlayer.Desktop.Services;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DudesPlayer.Desktop.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : UserControl
    {
        public LoginPage(INavService navigationService)
        {
            InitializeComponent();
            Loaded += LoginPage_Loaded;
            _navigationService = navigationService;
            SizeChanged += LoginPage_SizeChanged;
            TB.Focus();
            TB.KeyDown += TB_KeyDown;
        }

        private void TB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key== Key.Return||e.Key==Key.Enter)
            {
                if (SubmitButton.IsEnabled==true)
                {
                    _navigationService.Navigate(typeof(FriendsPage.FriendsPage));
                }
            }
        }

        private void LoginPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AnimationUpdate();
        }

        private void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            AnimationUpdate();
        }

        private void AnimationUpdate()
        {
            SubmitButton.IsEnabled = false;
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = GretGrid.ActualHeight;
            animation.To = ActualHeight * 0.35;
            animation.DecelerationRatio = 0.9;
            animation.AccelerationRatio = 0.1;
            animation.BeginTime = TimeSpan.FromSeconds(0.3);
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            GretGrid.BeginAnimation(HeightProperty, animation);
        }

        List<string> Strings = new List<string>() { "Hola", "Hello", "Witam", "你好", "こんにちは" };
        private INavService _navigationService;

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            HelloMessageTextBlock.Text = "Hola, %USERNAME%";
            Random random = new Random();
            var gret = Strings[random.Next(0, Strings.Count)];
            var username = (sender as TextBox).Text;
            if (string.IsNullOrEmpty(username))
            {
                username = "%USERNAME%";
                SubmitButton.IsEnabled = false;
            }
            else
            {
                SubmitButton.IsEnabled = true;
            }
            HelloMessageTextBlock.Text = $"{gret}, {username}";
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.Navigate(typeof(FriendsPage.FriendsPage));
        }
    }
}
