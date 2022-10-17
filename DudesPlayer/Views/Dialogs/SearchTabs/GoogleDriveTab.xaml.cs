using DudesPlayer.Library.Models;
using DudesPlayer.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using DudesPlayer.Views.SideBar;

namespace DudesPlayer.Views.Dialogs.SearchTabs
{
    /// <summary>
    /// Логика взаимодействия для GoogleDriveTab.xaml
    /// </summary>
    public partial class GoogleDriveTab : UserControl
    {
        public GoogleDriveTab()
        {
            InitializeComponent();
        }

        public EventHandler PlaylistItem_Click { get; private set; }
        public string APIKey { get { return "AIzaSyA-PP5oDBcqbzrPtHP6_J_Ob6_yexXQ23c"; } }

        private void Close(object sender, RoutedEventArgs e)
        {

        }

        private void Search(object sender, RoutedEventArgs e)
        {
            SearchBtn.IsEnabled = false;

            SearchSP.Children.Clear();

            string id = TextBox1.Text;
            id = GetFileId();

            //finally
            {
                Task.Factory.StartNew(() =>
                {
                    if (string.IsNullOrEmpty(id) == false)
                    {
                        Google.Apis.Drive.v3.DriveService driveService = new Google.Apis.Drive.v3.DriveService(new Google.Apis.Services.BaseClientService.Initializer() { ApiKey = APIKey });
                        Google.Apis.Drive.v3.FilesResource.GetRequest getRequest = new Google.Apis.Drive.v3.FilesResource.GetRequest(driveService, id);

                        getRequest.Fields = "webViewLink,exportLinks,linkShareMetadata,md5Checksum,name,size,webContentLink,thumbnailLink";
                        try
                        {
                            getRequest.CreateRequest();

                            var ex = getRequest.Execute();
                            Dispatcher.Invoke(() =>
                            {
                                var bi = new BitmapImage();
                                bi.BeginInit();
                                bi.UriSource = new Uri($"{ex.ThumbnailLink}");
                                bi.EndInit();
                                PlaylistDialogItem playlistItem = new PlaylistDialogItem();
                                playlistItem.Preview.Source = bi;
                                playlistItem.Title.Text = ex.Name;
                                playlistItem.Author.Text = "";
                                playlistItem.Tag = id;
                                playlistItem.Click += PlaylistItem_Click1; ;

                                SearchSP.Children.Add(playlistItem);

                                SearchSP.Children.Add(new Separator());
                                SearchBtn.IsEnabled = true;

                            });
                        }
                        catch
                        {
                            Dispatcher.Invoke(() =>
                            {
                                SearchBtn.IsEnabled = true;
                            });
                        }
                    }
                    else
                    {
                        Dispatcher.Invoke(() =>
                        {
                            SearchBtn.IsEnabled = true;
                        });
                    }
                });
            }
        }

        private string GetFileId()
        {
            string text = TextBox1.Text;
            try
            {
                return text.Length == 33 ? text : text.Split("d/")[1].Split("/")[0];
            }
            catch
            {
                return "";
            }
        }

        private void PlaylistItem_Click1(object sender, EventArgs e)
        {

            {
                URLModel urlModel = new URLModel();
                urlModel.Url = $"https://www.googleapis.com/drive/v3/files/{(sender as PlaylistDialogItem).Tag}?alt=media&key={APIKey}";
                urlModel.YTSource = "";
                urlModel.URLType = URLType.Google;
                urlModel.Name = (sender as PlaylistDialogItem).Title.Text;
                ClientData.Client.AddUrl(urlModel);
                MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, null);
                TextBox1.Text = "";
            }
        }

        private void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            Debug.WriteLine(TextBox1.Text);
            Debug.WriteLine(sender);
        }
    }
}
