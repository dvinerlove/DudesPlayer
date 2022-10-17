using DudesPlayer.Library;
using DudesPlayer.Library.Models;
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
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.ClosedCaptions;

namespace DudesPlayer.Views.Player
{
    /// <summary>
    /// Логика взаимодействия для Subtitles.xaml
    /// </summary>
    public partial class Subtitles : UserControl
    {
        public Subtitles()
        {
            InitializeComponent();
        }
        List<ClosedCaptionTrack> subtitles = new List<ClosedCaptionTrack>();
        public bool IsSubVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                isVisible = value;

                if (!isVisible)
                {
                    Text.Visibility = Visibility.Collapsed;
                }
            }
        }
        private bool isVisible;
        internal async void Load(URLModel url)
        {
            try
            {
                subtitles.Clear();
                Text.Visibility = Visibility.Collapsed;
                var youtube = new YoutubeClient();
                string id = "";

                if (string.IsNullOrEmpty(url.YTSource))
                {
                    var result = await youtube.Search.GetVideosAsync(url.GetTitle()[..^4]);
                    if (result.FirstOrDefault() != null)
                    {
                        id = result.FirstOrDefault().Id;
                    }
                }
                else
                {
                    id = url.YTSource;
                }

                if (string.IsNullOrEmpty(id) == false)
                {
                    var trackManifest = await youtube.Videos.ClosedCaptions.GetManifestAsync(id);

                    var trackInfo = trackManifest.TryGetByLanguage("ru");
                    if (trackInfo != null)
                    {
                        subtitles.Add(await youtube.Videos.ClosedCaptions.GetAsync(trackInfo));
                    }

                    trackInfo = trackManifest.TryGetByLanguage("en");

                    if (trackInfo != null)
                    {
                        subtitles.Add(await youtube.Videos.ClosedCaptions.GetAsync(trackInfo));
                    }
                }
            }
            catch (Exception ex)
            {
                VDebug.WriteLine(ex.Message);
            }
        }
        public void SetTime(long time)
        {
            if (isVisible == false)
            {
                return;
            }

            if (subtitles.Count == 0)
            {
                return;
            }

            Text.Visibility = Visibility.Collapsed;

            ClosedCaption caption = subtitles.First().TryGetByTime(TimeSpan.FromMilliseconds(time+100));

            if (caption != null)
            {
                Text.Text = caption.Text;
                Text.Visibility = Visibility.Visible;
            }

        }
    }
}
