using CG.Web.MegaApiClient;
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

namespace DudesPlayer.Views.Dialogs.SearchTabs
{
    /// <summary>
    /// Логика взаимодействия для MegaTab.xaml
    /// </summary>
    public partial class MegaTab : UserControl
    {
        public MegaTab()
        {
            InitializeComponent();
            MegaApiClient megaApiClient = new MegaApiClient();
            megaApiClient.Login();
            //var link = megaApiClient.GetDownloadLink(megaApiClient.GetNodeFromLink(new Uri("https://mega.nz/file/YtxhlTKD#gwRCcGb6wV9-THehnm3Fv89fy1CKNj99vuCOUga53no")));

            //MessageBox.Show(link.OriginalString);
            //MessageBox.Show(link.Query);

        }

    }
}
