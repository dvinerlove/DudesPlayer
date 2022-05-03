using DudesPlayer.Classes;
using DudesPlayer.Classes.Client;
using DudesPlayer.Models;
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

namespace DudesPlayer.Views.SideBar.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для JokeDialog.xaml
    /// </summary>
    public partial class JokeDialog : UserControl
    {
        public JokeDialog()
        {
            InitializeComponent();
            //Generate();
        }
        Joke joke;
        private void Generate()
        {
            joke = RandomJoke.Generate();
            Punchline.Text = joke.punchline;
            Setup.Text = joke.setup;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            joke = new Joke();
            joke.punchline = Punchline.Text;
            joke.setup = Setup.Text;
            Task.Factory.StartNew(() =>
            {
                ClientData.Client.CreateJoke(joke);
                Dispatcher.Invoke(() =>
                {
                });
            });
        }
       

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Generate();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                ClientData.Client.ShakeScreen();
               
            });
        }
    }
}
