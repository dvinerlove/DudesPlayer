using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DudesPlayer.Views.SideBar.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для SettingsDialog.xaml
    /// </summary>
    public partial class SettingsDialog : UserControl
    {
        public SettingsDialog()
        {
            InitializeComponent();
            Update();
        }

        private void Update()
        {
            PathField.Text = Properties.Settings.Default.LibDirectory;
        }

        private void Save()
        {
            Properties.Settings.Default.LibDirectory = PathField.Text;
            Properties.Settings.Default.Save();
        }

        private readonly string zipWebLink = "";//= "http://192.168.196.110/vlc.zip";

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FindPath();
        }

        private void FindPath()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    PathField.Text = dialog.SelectedPath;
                }
            }
            Save();
        }

        //download lib
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PathField.Text))
            {
                FindPath();
            }

            string myTempFile = Path.Combine(Path.GetTempPath(), "vlc.zip");

            if (File.Exists(PathField.Text))
            {
                Save();
                return;
            }

            if (File.Exists(myTempFile))
            {
                File.Delete(myTempFile);
            }
            IsEnabled = false;

            HttpClient client = new HttpClient();

            var response = await client.GetAsync(zipWebLink);

            using (var fs = new FileStream(myTempFile, mode: FileMode.CreateNew))
            {
                await response.Content.CopyToAsync(fs);
            }

            System.IO.Compression.ZipFile.ExtractToDirectory(myTempFile, PathField.Text);

            File.Delete(myTempFile);

            IsEnabled = true;

            Save();

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            App.UpdatePlayer();
            Save();
            Close();
        }

        private void Close()
        {
            MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, null);
        }
    }
}
