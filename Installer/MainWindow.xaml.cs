using Microsoft.Win32;
using System.Windows;
using ClassLibrary.Models;
using System.Linq;
using System;
using System.Diagnostics;
using YoutubeExplode;
using System.Windows.Controls;
using System.Threading.Tasks;
using YoutubeExplode.Videos.ClosedCaptions;
using System.Collections.Generic;

namespace Installer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClosedCaptionTrack track;

        public MainWindow()
        {
            InitializeComponent();


        }

        private async Task<List<ClosedCaptionTrack>> GetSubtitles(string id)
        {
            List<ClosedCaptionTrack> subtitles = new List<ClosedCaptionTrack>();  
            var youtube = new YoutubeClient();

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
            return subtitles;
        }
    }
}
/*
 long seconds = 0;
                while (true)
                {
                    Stack.Children.Clear();

                    ClosedCaption? caption = track.TryGetByTime(TimeSpan.FromSeconds(seconds));
                    if (caption != null)
                    {
                        TextBlock tb = new TextBlock();
                        tb.Text = caption.Text + " " + caption.Duration + " " + caption.Offset;
                        Stack.Children.Add(tb);
                    }
                    await Task.Delay(1000);
                    seconds++;
                }*/