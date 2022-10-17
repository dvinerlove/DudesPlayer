using DudesPlayer.Desktop.Views.Pages.RoomsExplorerPage;
using System;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Mvvm.Contracts;

namespace DudesPlayer.Desktop.Views.Controls
{
    public class ModalWindowService : IModalWindowService
    {
        private IModalWindowDialog _dialog;
        private IPageService _pageService;

        public void SetPageService(IPageService pageService)
        {
            _pageService = pageService;
        }

        public void ShowModal(Type pageType)
        {
            IModalWindow? page = _pageService.GetPage(pageType) as IModalWindow;
            if (page != null)
            {
                _dialog.SetModal(page);
            }
        }

        public void ShowModal(FrameworkElement element)
        {
            IModalWindow? page = (IModalWindow?)element;
            if (page != null)
            {
                _dialog.SetModal(page);
            }
        }

        public void Close()
        {
            _dialog.Close();
        }

        void IModalWindowService.SetModalWindow(IModalWindowDialog dialog)
        {
            _dialog = dialog;
        }
    }
}
