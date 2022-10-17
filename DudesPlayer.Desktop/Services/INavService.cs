using DudesPlayer.Desktop.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Wpf.Ui.Mvvm.Contracts;

namespace DudesPlayer.Desktop.Services
{
    public interface INavService
    {
        Frame GetFrame();
        void SetFrame(Frame frame);
        void SetFullFrame(Frame frame);
        INavControl GetNavigationControl();
        void SetNavigationControl(INavControl navigation);
        void SetPageService(IPageService pageService);
        bool Navigate(Type pageType);
        bool ShowFullPage(Type pageType);
        event EventHandler Navigated;
        event EventHandler FullPageShowed;
    }
}
