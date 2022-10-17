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
using Wpf.Ui.Common;
using Wpf.Ui.Controls.Interfaces;

namespace DudesPlayer.Desktop.Views.Pages.HomePage
{


    /// <summary>
    /// Логика взаимодействия для NavigationItem.xaml
    /// </summary>
    public partial class NavigationItem : UserControl, IMyNavItem
    {
        public event EventHandler<bool>? IsCheckedChanged;
        public event RoutedEventHandler Click;


        public string PageTag { get; set; }
        public bool IsActive { get; set; }
        public bool Cache { get; set; }
        public Uri PageSource { get; set; }
        public Type PageType { get; set; }
        public Uri AbsolutePageSource { get; set; }

        public NavigationItem()
        {
            InitializeComponent();
            Loaded += NavigationItem_Loaded;
        }

        private void NavigationItem_Loaded(object sender, RoutedEventArgs e)
        {
            SelectedColorBrush = new SolidColorBrush(Colors.White);
        }

        private static void OnChangedCallBack(
                DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            NavigationItem c = sender as NavigationItem;
            if (c != null)
            {
                if (e.Property.Name == "IsChecked")
                {
                    c.IsCheckedChanged?.Invoke(c, c.IsChecked);
                    if (c.IsChecked)
                    {
                        c.DefaultBorderBrush = (SolidColorBrush)Application.Current.FindResource("AccentBrush");
                        c.SelectedColorBrush = (SolidColorBrush)Application.Current.FindResource("AccentBrush");
                        c.SelectedColorBrush = new SolidColorBrush(Colors.White);
                        c.TextBlock1.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        c.DefaultBorderBrush = new SolidColorBrush(Colors.Transparent);
                        c.SelectedColorBrush = new SolidColorBrush(Colors.White);
                        c.TextBlock1.Visibility = Visibility.Visible;
                    }
                }
                Console.WriteLine();
            }
        }




        public SolidColorBrush DefaultBorderBrush
        {
            get { return (SolidColorBrush)GetValue(DefaultBorderBrushProperty); }
            set { SetValue(DefaultBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultBorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultBorderBrushProperty =
            DependencyProperty.Register("DefaultBorderBrush", typeof(SolidColorBrush), typeof(NavigationItem), new PropertyMetadata(OnChangedCallBack));



        public SolidColorBrush SelectedColorBrush
        {
            get { return (SolidColorBrush)GetValue(SelectedColorBrushProperty); }
            set { SetValue(SelectedColorBrushProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorBrushProperty =
            DependencyProperty.Register("SelectedColorBrush", typeof(SolidColorBrush), typeof(NavigationItem), new PropertyMetadata(OnChangedCallBack));




        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(NavigationItem), new PropertyMetadata(OnChangedCallBack));



        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(NavigationItem), new PropertyMetadata(OnChangedCallBack));



        public SymbolRegular Symbol
        {
            get { return (SymbolRegular)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }


        // Using a DependencyProperty as the backing store for Symbol.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register("Symbol", typeof(SymbolRegular), typeof(NavigationItem), new PropertyMetadata(OnChangedCallBack));

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e);
        }
    }
}
