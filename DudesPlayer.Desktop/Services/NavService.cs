using DudesPlayer.Desktop.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace DudesPlayer.Desktop.Services
{
    public class NavService : INavService
    {

        private Frame _fullFrame;
        private Frame _frame;

        private INavControl navigationControl;

        private IPageService _pageService;

        public event EventHandler Navigated;
        public event EventHandler FullPageShowed;

        public Frame GetFrame()
        {
            return _frame;
        }

        public void SetFrame(Frame frame)
        {
            _frame = frame;
        }
        public void SetFullFrame(Frame frame)
        {
            _fullFrame = frame;
        }

        public INavControl GetNavigationControl()
        {
            return navigationControl;
        }

        public void SetNavigationControl(INavControl navigation)
        {
            navigationControl = navigation;
            navigationControl.ItemSelected += NavigationControl_ItemSelected;
        }

        private void NavigationControl_ItemSelected(object? sender, Type e)
        {
            Navigate(e);
        }

        public void SetPageService(IPageService pageService)
        {
            _pageService = pageService;
        }

        public bool Navigate(Type pageType)
        {
            if (pageType == null)
            {
                return false;
            }
            var page = _pageService.GetPage(pageType);
            if (page != null)
            {
                _frame.Content = page;
                _frame.Visibility = System.Windows.Visibility.Visible;
                _fullFrame.Visibility = System.Windows.Visibility.Collapsed;
                navigationControl.SelectItem(pageType);
                Navigated?.Invoke(this, EventArgs.Empty);
                return true;
            }
            return false;

        }

        public bool ShowFullPage(Type pageType)
        {
            if (pageType == null)
            {
                return false;
            }
            var page = _pageService.GetPage(pageType);
            if (page != null)
            {
                _fullFrame.Content = page;
                _frame.Visibility = System.Windows.Visibility.Collapsed;
                _fullFrame.Visibility = System.Windows.Visibility.Visible;
                FullPageShowed?.Invoke(this, EventArgs.Empty);
                return true;
            }
            return false;
        }
    }
}