using ClassLibrary;
using ClassLibrary.Models;
using DudesPlayer.Classes;
using DudesPlayer.Models;
using DudesPlayer.Views.Fun.Chat;
using DudesPlayer.Views.SideBar.Dialogs;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Vlc.DotNet.Wpf;
using YoutubeExplode;

namespace DudesPlayer.Views.Player
{
    /// <summary>
    /// Interaction logic for PlayerWindow.xaml
    /// </summary>
    public partial class PlayerWindow : UserControl
    {
        public VlcVideoSourceProvider sourceProvider;
        public VlcVideoSourceProvider audioProvider;
        private RoomSettings lastSettings;
        private URLModel currentURL;

        public PlayerWindow()
        {
            InitializeComponent();


            SubsTB.Text = Properties.Settings.Default.SubsDirectory;

            ClientCommandHandler.Update += (ss, ee) =>
            {
                Dispatcher.Invoke(() =>
                {
                    Update();
                });
            };
            ClientCommandHandler.Disconnect += ClientCommandHandler_Disconnect;
            ClientCommandHandler.Shake += ClientCommandHandler_Shake;
            ClientCommandHandler.Set += SetMedia;
            ClientCommandHandler.Play += Resume;
            ClientCommandHandler.Pause += Pause;
            ClientCommandHandler.Time += SetTime;
            ClientCommandHandler.Joke += Joke;
            Unloaded += MainWindow_Unloaded;
        }



        private void ClientCommandHandler_Shake(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    BeginStoryboard sb = this.FindResource("BotRotation") as BeginStoryboard;
                    sb.Storyboard.Begin();

                    ScaleTransform scaleTransform = new ScaleTransform();
                    VideoScaleGrid.RenderTransform = scaleTransform;
                    DoubleAnimation anim2 = new DoubleAnimation();
                    anim2.Duration = new Duration(TimeSpan.FromMilliseconds(70));
                    anim2.AutoReverse = true;
                    anim2.RepeatBehavior = new RepeatBehavior(7);
                    anim2.FillBehavior = FillBehavior.Stop;
                    anim2.From = 1;
                    anim2.To = 1.3;
                    VideoScaleGrid.RenderTransformOrigin = new Point(0.5, 0.5);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, anim2);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, anim2);

                    TranslateTransform trans = new TranslateTransform();
                    VideoShakeGrid.RenderTransform = trans;
                    anim2.From = -20;
                    anim2.To = 20;

                    trans.BeginAnimation(TranslateTransform.YProperty, anim2);
                });
            });
        }

        private void ClientCommandHandler_Disconnect(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                ThreadPool.QueueUserWorkItem(_ => this.sourceProvider.MediaPlayer.Stop());
                ThreadPool.QueueUserWorkItem(_ => this.audioProvider.MediaPlayer.Stop());
                VideoGrid.Visibility = Visibility.Collapsed;
            });
        }


        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void Update()
        {
            if (lastSettings == null || lastSettings.IsEqual(ClientData.Client.GetRoom().Settings) == false)
            {

                if (lastSettings == null || lastSettings.CurrentURL != ClientData.Client.GetRoom().Settings.CurrentURL)
                {
                    if (ClientData.Client.GetRoom().Settings != null)
                    {
                        currentURL = ClientData.Client.GetRoom().Settings.CurrentURL;
                        SetMedia(null, EventArgs.Empty);
                    }
                }
                if (lastSettings == null || lastSettings.Speed != ClientData.Client.GetRoom().Settings.Speed)
                {
                }
                if (lastSettings == null || lastSettings.State != ClientData.Client.GetRoom().Settings.State)
                {
                    //switch (ClientData.Client.GetRoom().Settings.State)
                    //{
                    //    case RoomState.Play:
                    //        Resume(null, EventArgs.Empty);
                    //        break;
                    //    case RoomState.Pause:
                    //        Pause(null, EventArgs.Empty);
                    //        break;
                    //    default:
                    //        break;
                    //}
                }
                if (lastSettings == null || lastSettings.CurrentTime != ClientData.Client.GetRoom().Settings.CurrentTime)
                {
                    //Time(ClientData.Client.GetRoom().Settings.CurrentTime!, EventArgs.Empty);
                }
                lastSettings = ClientData.Client.GetRoom().Settings;

            }
        }

        private void App_UpdatePlayerEvent(object sender, EventArgs e)
        {
            Startup();
        }

        public void Startup()
        {
            Task.Factory.StartNew(() =>
            {
                Dispatcher.Invoke(async () =>
                {
                    try
                    {
                        if (vlcLibDirectory == null)
                        {
                            vlcLibDirectory = new DirectoryInfo(Properties.Settings.Default.LibDirectory);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        await DialogHost.Show(new SettingsDialog());
                        Startup();
                        return;
                    }

                    try
                    {
                        if (sourceProvider == null)
                        {
                            this.sourceProvider = new VlcVideoSourceProvider(this.Dispatcher);
                        }
                        if (audioProvider == null)
                        {
                            this.audioProvider = new VlcVideoSourceProvider(this.Dispatcher);
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        await DialogHost.Show(new SettingsDialog());
                        Startup();
                        return;
                    }
                    try
                    {
                        this.sourceProvider.CreatePlayer(vlcLibDirectory);
                        this.audioProvider.CreatePlayer(vlcLibDirectory);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        await DialogHost.Show(new SettingsDialog());
                        Startup();
                        return;
                    }

                    sourceProvider.MediaPlayer.PositionChanged += MediaPlayer_PositionChanged;
                    sourceProvider.MediaPlayer.EndReached += ClientCommandHandler_Disconnect;
                    sourceProvider.MediaPlayer.MediaChanged += MediaPlayer_MediaChanged;
                });
            });
        }

        private void Joke(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var tmp = sender.ToString().Split("?.?123**");
                Joke joke = new Joke() { setup = tmp[0], punchline = tmp[1] };
                DemotivatorToggle(joke);
            });
        }

        private void SetTime(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    Time(long.Parse(sender as string), e);
                });
            });
        }

        private void Time(long? sender, EventArgs e)
        {
            if (isSliderDragStarted == false && sender != null)
            {
                VDebug.WriteLine(sender);
                if (sourceProvider.MediaPlayer.Time != (long)sender)
                {
                    sourceProvider.MediaPlayer.Time = (long)sender;
                    audioProvider.MediaPlayer.Time = (long)sender;
                    ThreadPool.QueueUserWorkItem(_ => { /*sourceProvider.MediaPlayer.Time = (long)sender;*/ });
                }
            }
        }

        private void Resume(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        if (currentURL != null && string.IsNullOrEmpty(currentURL.Url) == false)
                        {

                            if (currentURL.URLType != URLType.link)
                            {
                                audioProvider.MediaPlayer.Play();
                            }

                            sourceProvider.MediaPlayer.Play();


                            PlayBtn.Content = new PackIcon() { Kind = PackIconKind.Pause, Width = 40, Height = 40 };
                        }
                    }
                    catch
                    {

                    }
                });
            });
        }

        private void Pause(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Dispatcher.Invoke(() =>
                {
                    PlayBtn.Content = new PackIcon() { Kind = PackIconKind.Play, Width = 40, Height = 40 };
                    this.sourceProvider.MediaPlayer.Pause();
                    this.audioProvider.MediaPlayer.Pause();
                });
            });
        }

        public MainWindow MainWindow { get; internal set; }
        public SideMenu SideMenu { get; internal set; }

        private void SetMedia(object sender, EventArgs e)
        {
            if (sender != null)
            {
                currentURL = (URLModel)sender;

                if (currentURL == null)
                {
                    return;
                }
            }
            else
            {
                return;
            }

            try
            {
                new Uri(currentURL.Url);
            }
            catch
            {
                return;
            }

            Dispatcher.Invoke(() =>
            {
                SideMenu.TimeOut();
                QualityCB.Items.Clear();
                Title.Text = currentURL.GetTitle();
                Subtitles.Load(currentURL);

                switch (currentURL.URLType)
                {
                    case URLType.link:
                        if (QualityCB.Visibility != Visibility.Collapsed)
                        {
                            QualityCB.Visibility = Visibility.Collapsed;
                        }
                        SetVideo(currentURL.Url);
                        break;
                    case URLType.youtube:
                        QualityCB.Visibility = Visibility.Visible;
                        SetYouTubeVideo(currentURL.Url);
                        break;
                    default:
                        break;
                }
            });
        }

        private void SetYouTubeVideo(string url, string quality = "1080")
        {
            var youtube = new YoutubeClient();
            try
            {
                Task.Factory.StartNew(async () =>
                {
                    var manifest = await youtube.Videos.Streams.GetManifestAsync(url);

                    var streams = manifest.GetVideoOnlyStreams();

                    //UrlExtractor.GetVideo(currentURL.Url, quality);
                    //UrlExtractor.GetAudio(currentURL.Url);

                    Dispatcher.Invoke(() =>
                    {
                        QualityCB.SelectionChanged -= QualityCB_SelectionChanged;
                        QualityCB.Items.Clear();
                        foreach (var item in streams.Select(x => x.VideoQuality.Label).Distinct())
                        {
                            QualityCB.Items.Add(new ComboBoxItem() { Content = item + "      ", IsSelected = item.Contains(quality.Trim()) });
                        }
                        QualityCB.SelectionChanged += QualityCB_SelectionChanged;

                        var videoStream = streams.Where(x => x.VideoQuality.Label.Contains(quality.Trim())).FirstOrDefault();

                        if (videoStream == null)
                        {
                            videoStream = manifest.GetVideoOnlyStreams().FirstOrDefault();
                            foreach (var item in QualityCB.Items.OfType<ComboBoxItem>())
                            {
                                if (item.Content.ToString().Contains(videoStream.VideoQuality.Label))
                                {
                                    item.IsSelected = true;
                                }
                            }
                        }
                        else
                        {
                            VDebug.WriteLine(videoStream.Url);
                        }

                        var audioStreams = manifest.GetAudioOnlyStreams();

                        YoutubeExplode.Videos.Streams.AudioOnlyStreamInfo audioStream = audioStreams.FirstOrDefault();
                        foreach (var item in audioStreams)
                        {
                            if (item.Bitrate.MegaBitsPerSecond > audioStream.Bitrate.MegaBitsPerSecond)
                            {
                                audioStream = item;
                            }
                        }
                        SetVideo(videoStream.Url, audioStream.Url);
                    });
                });



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "youtube alert");
            }
        }

        private async void SetVideo(string url, string audioUrl = null)
        {

            List<string> options = new List<string>()
                    {
                        new string( "--network-caching=6000" ),
                        new string( ":network-caching=6000" ),
                        new string( "--aout=directsound" ),
                    };

            options.AddRange(await FindSubtitles());

            PlayBtn.Content = new PackIcon() { Kind = PackIconKind.Play, Width = 40, Height = 40 };

            ThreadPool.QueueUserWorkItem(async _ =>
            {
                sourceProvider.MediaPlayer.Pause();
                audioProvider.MediaPlayer.Pause();
                while (sourceProvider.MediaPlayer.IsPlaying() == true)
                {
                    await Task.Delay(100);
                }
                sourceProvider.MediaPlayer.SetMedia(new Uri(url), options.ToArray());

                if (string.IsNullOrEmpty(audioUrl) == false)
                {
                    audioProvider.MediaPlayer.SetMedia(new Uri(audioUrl), options.ToArray());
                }
            });

            try
            {
                try
                {
                    Video.SetBinding(System.Windows.Controls.Image.SourceProperty,
                    new Binding(nameof(VlcVideoSourceProvider.VideoSource)) { Source = sourceProvider });

                    BackgroundVideo.SetBinding(System.Windows.Controls.Image.SourceProperty,
                        new Binding(nameof(VlcVideoSourceProvider.VideoSource)) { Source = sourceProvider });

                    MaxTime.Text = sourceProvider.MediaPlayer.Length.ToCorrectTime();

                }
                finally
                {
                    VideoGrid.Visibility = Visibility.Visible;
                }

                return;
            }
            catch (Exception ex)
            {
                VDebug.WriteLine(ex.ToMessageString());
                return;
            }


        }

        private async Task<List<string>> FindSubtitles()
        {
            VDebug.WriteLine("Subs Directory");
            VDebug.WriteLine(Properties.Settings.Default.SubsDirectory);
            VDebug.WriteLine("Subs Url");
            VDebug.WriteLine(currentURL.Url.Substring(0, currentURL.Url.Length - 3) + "srt");

            List<string> strings = new List<string>();

            if (string.IsNullOrEmpty(Properties.Settings.Default.SubsDirectory))
            {
                return strings;
            }

            string subFile = Properties.Settings.Default.SubsDirectory + @"\" + Title.Text.Substring(0, Title.Text.Length - 3) + "srt";

            if (File.Exists(subFile) == false)
            {
                File.Delete(subFile);
            }

            await ClientData.Client.TrySaveSubtitle(Title.Text.Substring(0, Title.Text.Length - 3), Properties.Settings.Default.SubsDirectory);

            if (File.Exists(subFile))
            {
                strings.Add(new string("--sub-file=" + subFile));
                strings.Add(new string("sub-file=" + subFile));
            }
            return strings;
        }

        internal void PlayToggle()
        {
            var isPlaying = sourceProvider.MediaPlayer.IsPlaying();
            Task.Factory.StartNew(() =>
            {
                if (isPlaying)
                {
                    ClientData.Client.Pause();
                }
                else
                {
                    ClientData.Client.Play();
                }
            });
        }

        private async void Subs_Click(object sender, RoutedEventArgs e)
        {
            await DialogHost.Show(new SubtitlesFolderDialog());
            SubsTB.Text = Properties.Settings.Default.SubsDirectory;
        }

        private void MediaPlayer_MediaChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerMediaChangedEventArgs e)
        {
            CheckSettings();

            ThreadPool.QueueUserWorkItem(async _ =>
            {
                await Task.Delay(100);
                Resume(null, EventArgs.Empty);
            });
            //ThreadPool.QueueUserWorkItem(_ => this.audioProvider.MediaPlayer.Play());
        }

        private void CheckSettings()
        {
            Task.Factory.StartNew(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    SideMenu.IsEnabled = true;

                    var audios = sourceProvider.MediaPlayer.Audio.Tracks.All.ToList();
                    var subtitles = sourceProvider.MediaPlayer.SubTitles.All.ToList();
                    AudiosComboBox.Items.Clear();
                    SubtitlesComboBox.Items.Clear();
                    AudiosComboBox.SelectionChanged += AudiosComboBox_SelectionChanged;
                    SubtitlesComboBox.SelectionChanged += SubtitlesComboBox_SelectionChanged;
                    if (audios.Count > 0)
                    {
                        foreach (var audio in audios)
                        {
                            var aud = new ComboBoxItem() { Tag = audio, Content = audio.Name + "        " };

                            if (sourceProvider.MediaPlayer.Audio.Tracks.Current.ID == audio.ID)
                            {
                                aud.IsSelected = true;
                            }
                            AudiosComboBox.Items.Add(aud);
                        }
                    }
                    else
                    {
                        AudiosComboBox.Items.Clear();
                    }

                    if (subtitles.Count > 0)
                    {
                        foreach (var subtitle in subtitles)
                        {
                            var sub = new ComboBoxItem() { Tag = subtitle, Content = subtitle.Name + "        " };
                            if (sourceProvider.MediaPlayer.SubTitles.Current.ID == subtitle.ID)
                            {
                                sub.IsSelected = true;
                            }
                            SubtitlesComboBox.Items.Add(sub);
                        }
                    }
                    else
                    {
                        SubtitlesComboBox.Items.Clear();
                    }
                });
            });
        }

        private void SubtitlesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = (ComboBoxItem)SubtitlesComboBox.SelectedItem;
                if (item != null)
                {
                    sourceProvider.MediaPlayer.SubTitles.Current = (Vlc.DotNet.Core.TrackDescription)(item).Tag;
                }
            }
            catch
            {

            }
        }

        private void AudiosComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = (ComboBoxItem)AudiosComboBox.SelectedItem;
                if (item != null)
                {
                    sourceProvider.MediaPlayer.Audio.Tracks.Current = (Vlc.DotNet.Core.TrackDescription)(item).Tag;
                }
            }
            catch
            {

            }
        }

        private void MediaPlayer_PositionChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerPositionChangedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        if (sourceProvider.MediaPlayer.Time == 0 ||
                        audioProvider.MediaPlayer.Time == 0)
                        {
                            audioProvider.MediaPlayer.Time = sourceProvider.MediaPlayer.Time;
                        }
                        else
                        {
                            var abs = Math.Abs((sourceProvider.MediaPlayer.Time) - (audioProvider.MediaPlayer.Time));

                            if (abs > 300)
                            {
                                audioProvider.MediaPlayer.Time = sourceProvider.MediaPlayer.Time;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        VDebug.WriteLine(ex.ToMessageString());
                    }

                    Subtitles.SetTime(sourceProvider.MediaPlayer.Time);

                    if (!isMouseDown)
                        CurrentName.Text = sourceProvider.MediaPlayer.Time.ToCorrectTime();

                    MaxTime.Text = sourceProvider.MediaPlayer.Length.ToCorrectTime();

                    if (isDrahStarted == false)
                    {
                        Slider.Maximum = sourceProvider.MediaPlayer.Length;
                        Slider.Value = sourceProvider.MediaPlayer.Time;
                    }
                });
            });
        }


        private void MediaPlayer_Playing(object sender, Vlc.DotNet.Core.VlcMediaPlayerPlayingEventArgs e)
        {
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        DispatcherTimer _timer = new DispatcherTimer();
        DispatcherTimer _jokeTimer = new DispatcherTimer();
        Image jokeImage;
        internal void DemotivatorToggle(Joke joke)
        {
            var setup = joke.setup;
            var punchline = joke.punchline;
            DemotivatorGrid.Visibility = Visibility.Visible;
            _jokeTimer.Interval = TimeSpan.FromSeconds(10);
            _jokeTimer.Tick -= _jokeTimer_Tick;
            _jokeTimer.Tick += _jokeTimer_Tick;
            Cursor = Cursors.Arrow;
            _jokeTimer.Start();
            var prev = DemotivatorGrid.Content;
            if (prev == null)
            {
                jokeImage = new Image() { Margin = new Thickness(6), Stretch = Stretch.Fill };

                jokeImage.SetBinding(System.Windows.Controls.Image.SourceProperty,
                      new Binding(nameof(VlcVideoSourceProvider.VideoSource)) { Source = sourceProvider });

                DemotivatorGrid.Content = CreateMeme(jokeImage, setup, punchline);
            }
            else
            {
                DemotivatorGrid.Content = null;
                DemotivatorGrid.Content = CreateMeme((UIElement)prev, setup, punchline);
            }
        }

        Viewbox CreateMeme(UIElement uIElement, string setup, string punchline)
        {
            Viewbox viewbox = new Viewbox() { StretchDirection = StretchDirection.Both, Stretch = Stretch.Fill, Margin = new Thickness(0) };

            StackPanel stackPanel1 = new StackPanel() { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(160, 80, 160, 80) };
            viewbox.Child = stackPanel1;
            Border border = new Border() { Width = 750, Height = 600, BorderBrush = new SolidColorBrush(Colors.White), BorderThickness = new Thickness(2) };
            StackPanel stackPanel2 = new StackPanel() { VerticalAlignment = VerticalAlignment.Center };
            stackPanel1.Children.Add(border);
            stackPanel1.Children.Add(stackPanel2);
            border.Child = uIElement;

            TextBlock textBlock1 = new TextBlock() { Text = setup, HorizontalAlignment = HorizontalAlignment.Center, FontSize = 36, Margin = new Thickness(12) };
            textBlock1.SetValue(TextBlock.FontWeightProperty, FontWeights.DemiBold);
            TextBlock textBlock2 = new TextBlock() { Text = punchline, HorizontalAlignment = HorizontalAlignment.Center, FontSize = 20 };

            stackPanel2.Children.Add(textBlock1);
            stackPanel2.Children.Add(textBlock2);
            return viewbox;
        }

        private void _jokeTimer_Tick(object sender, EventArgs e)
        {
            DemotivatorGrid.Visibility = Visibility.Collapsed;
            DemotivatorGrid.Content = null;
            if (jokeImage != null)
                jokeImage.Source = null;
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            ControlPanelGrid.Visibility = Visibility.Visible;
            _timer.Interval = TimeSpan.FromSeconds(2);
            _timer.Tick -= _timer_Tick;
            _timer.Tick += _timer_Tick;
            Cursor = Cursors.Arrow;
            _timer.Start();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            Cursor = Cursors.None;
            ControlPanelGrid.Visibility = Visibility.Collapsed;
            _timer.Stop();
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            ControlPanelGrid.Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (App.Current.MainWindow as MainWindow).ToggleFullScreen();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PlayToggle();
        }

        private void Slider_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
        }

        private bool isSliderDragStarted = false;
        private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            isSliderDragStarted = true;
        }

        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            var value = (long)Slider.Value;
            Task.Factory.StartNew(() =>
            {
                ClientData.Client.SetTime(value);
            });
            isSliderDragStarted = false;
        }

        bool isDraged = false;
        bool isMouseDown = false;

        private void Slider_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            isDraged = true;
        }

        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CurrentName.Text = ((long)Slider.Value).ToCorrectTime();

            if (isSliderDragStarted == false && isDraged == true ||
                isSliderDragStarted == false && isMouseDown == true)
            {
                isDraged = false;
            }
        }

        private void Slider_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = true;
        }

        private void Slider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = false;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MainWindow.MenuToggle();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Application.Current.MainWindow.DragMove();
            }
            catch
            {

            }
        }

        private void VolumeSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (IsLoaded)
                Task.Factory.StartNew(() =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        int vol = (int)VolumeSlider.Value;

                        if (sourceProvider.MediaPlayer.Audio.Volume != vol)
                        {
                            sourceProvider.MediaPlayer.Audio.Volume = vol;
                        }
                        if (audioProvider.MediaPlayer.Audio.Volume != vol)
                        {
                            audioProvider.MediaPlayer.Audio.Volume = vol;
                        }
                    });
                });
        }

        private void Slider_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void VolumeSlider_DragCompleted_1(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            isDrahStarted = false;
            if (Math.Abs(e.HorizontalChange) > 2)
            {
                var value = (long)Slider.Value;
                Task.Factory.StartNew(() =>
                {
                    ClientData.Client.SetTime(value);
                });
            }
        }

        private void Slider_ValueChanged_2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }
        bool isDrahStarted = false;
        private DirectoryInfo vlcLibDirectory;

        private void Slider_DragStarted_1(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            isDrahStarted = true;
        }

        private void Slider_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
        }

        private void Slider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var value = (long)Slider.Value;
            Task.Factory.StartNew(() =>
            {
                ClientData.Client.SetTime(value);
            });

        }

        private void Slider_DragDelta_1(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.HideMenu();
            MainWindow.MenuToggle();
        }

        private void Popup_Opened(object sender, EventArgs e)
        {
            CheckSettings();
        }

        private void Badge_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PlaylistDialog.Visibility == Visibility.Collapsed)
                PlaylistToggle();
        }

        private void ChatToggle()
        {
            if (ChatDialog.Visibility == Visibility.Visible)
            {
                ChatDialog.Visibility = Visibility.Collapsed;
            }
            else
            {
                ChatDialog.Visibility = Visibility.Visible;
            }

            ControlPanelGrid_IsHitTestVisibleChanged(null, new DependencyPropertyChangedEventArgs());
        }

        public void PlaylistToggle()
        {
            if (PlaylistDialog.Visibility == Visibility.Visible)
            {
                PlaylistDialog.Visibility = Visibility.Collapsed;
            }
            else
            {
                PlaylistDialog.Load();
                PlaylistDialog.Visibility = Visibility.Visible;
            }

            ControlPanelGrid_IsHitTestVisibleChanged(null, new DependencyPropertyChangedEventArgs());
        }

        private void ControlPanelGrid_IsHitTestVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ControlPanelGrid.Visibility == Visibility.Visible)
            {
                PlaylistBadge.Visibility = Visibility.Visible;
            }
            else
            {
                if (PlaylistDialog.Visibility == Visibility)
                {
                    PlaylistBadge.Visibility = Visibility.Visible;
                }
                else
                {
                    PlaylistBadge.Visibility = Visibility.Collapsed;
                }
            }

            if (ControlPanelGrid.Visibility == Visibility.Visible)
            {
                ChatBadge.Visibility = Visibility.Visible;
            }
            else
            {
                if (ChatDialog.Visibility == Visibility)
                {
                    ChatBadge.Visibility = Visibility.Visible;
                }
                else
                {
                    ChatBadge.Visibility = Visibility.Collapsed;
                }
            }


            if (ChatDialog.Visibility != Visibility.Visible)
            {
                ChatBadge.VerticalAlignment = VerticalAlignment.Top;
            }
            else
            {
                ChatBadge.VerticalAlignment = VerticalAlignment.Stretch;
            }
        }

        private void ControlPanelGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void VB_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PlaylistDialog.Visibility = Visibility.Collapsed;
            ChatDialog.Visibility = Visibility.Collapsed;
            if (ChatDialog.Visibility != Visibility.Visible)
            {
                ChatBadge.VerticalAlignment = VerticalAlignment.Top;
            }
            else
            {
                ChatBadge.VerticalAlignment = VerticalAlignment.Stretch;
            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Subtitles.IsSubVisible = SubtitlesToggle.IsChecked.Value;
            });
        }

        private void QualityCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (currentURL != null && currentURL.URLType == URLType.youtube)
                    {
                        ComboBoxItem boxItem = (ComboBoxItem)QualityCB.SelectedItem;
                        if (boxItem != null)
                            SetYouTubeVideo(currentURL.Url, boxItem.Content.ToString());
                    }
                });
            });
        }

        private void ChatBadge_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ChatDialog.Visibility == Visibility.Collapsed)
                ChatToggle();
        }

        private void ChatDialog_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ChatDialog.Visibility == Visibility.Visible)
            {
                ChatAlertGrid.Visibility = Visibility.Hidden;
            }
            else
            {
                ChatAlertGrid.Visibility = Visibility.Visible;
            }
        }
    }

    public static class StringExtension
    {
        public static string ToCorrectTime(this long ms)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(ms);
            return string.Format("{0:D2}:{1:D2}:{2:D2}",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds);
        }
    }
}
