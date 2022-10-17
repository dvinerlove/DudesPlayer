using System;
using System.Windows;
using Wpf.Ui.Mvvm.Contracts;

namespace DudesPlayer.Desktop.Views.Controls
{
    public interface IModalWindowService
    {
        void SetModalWindow(IModalWindowDialog dialog);
        void SetPageService(IPageService pageService);
        void ShowModal(Type pageType);
        void ShowModal(FrameworkElement element);
        void Close();
    }
}
