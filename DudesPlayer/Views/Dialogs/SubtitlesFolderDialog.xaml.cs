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
    /// Логика взаимодействия для SubtitlesFolderDialog.xaml
    /// </summary>
    public partial class SubtitlesFolderDialog : UserControl
    {
        public SubtitlesFolderDialog()
        {
            InitializeComponent();
            Update();
        }
        private void Update()
        {
            PathField.Text = Properties.Settings.Default.SubsDirectory;
        }

        private void Save()
        {
            Properties.Settings.Default.SubsDirectory = PathField.Text;
            Properties.Settings.Default.Save();
        }

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
