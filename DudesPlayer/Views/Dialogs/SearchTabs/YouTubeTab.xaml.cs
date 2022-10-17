using DudesPlayer.Library;
using DudesPlayer.Library.Models;
using DudesPlayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using YoutubeExplode;
using YoutubeSearchApi.Net.Models.Youtube;
using YoutubeSearchApi.Net.Services;

namespace DudesPlayer.Views.Dialogs.SearchTabs
{
    /// <summary>
    /// Логика взаимодействия для YouTubeTab.xaml
    /// </summary>
    public partial class YouTubeTab : UserControl
    {
        public YouTubeTab()
        {
            InitializeComponent();
        }
        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            SearchBtn.IsEnabled = false;
            if (string.IsNullOrEmpty(SearchTB.Text) == false)
            {

                using (var httpClient = new HttpClient())
                {
                    YoutubeSearchClient client = new YoutubeSearchClient(httpClient);

                    var responseObject = await client.SearchAsync(SearchTB.Text);

                    foreach (YoutubeVideo video in responseObject.Results)
                    {
                        Console.WriteLine(video.ToString());
                        Console.WriteLine("");
                    }

                    SearchSP.Children.Clear();
                    foreach (var item in responseObject.Results)
                    {
                        var bi = new BitmapImage();
                        bi.BeginInit();
                        bi.UriSource = new Uri($"https://img.youtube.com/vi/{item.Id}/0.jpg");
                        bi.EndInit();
                        PlaylistDialogItem playlistItem = new PlaylistDialogItem();
                        playlistItem.Preview.Source = bi;
                        playlistItem.Title.Text = item.Title;
                        playlistItem.Author.Text = item.Author;
                        playlistItem.Tag = item.Id;
                        playlistItem.Click += PlaylistItem_Click;

                        SearchSP.Children.Add(playlistItem);

                        SearchSP.Children.Add(new Separator());
                    }
                    SearchBtn.IsEnabled = true;
                }
            }
        }

        private async void PlaylistItem_Click(object sender, EventArgs e)
        {
            try
            {
                SearchBtn.IsEnabled = false;
                var youtube = new YoutubeClient();

                var video = await youtube.Videos.GetAsync((sender as PlaylistDialogItem).Tag.ToString());

                URLModel uRLModel = new URLModel();
                uRLModel.Url = video.Url;
                uRLModel.YTSource = "";
                uRLModel.URLType = URLType.Youtube;
                uRLModel.Name = video.Title;
                ClientData.Client.AddUrl(uRLModel);

                MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, null);
                SearchBtn.IsEnabled = true;

            }
            catch (Exception ex)
            {
                VDebug.WriteLine(ex.Message);
            }
        }
    }
}
