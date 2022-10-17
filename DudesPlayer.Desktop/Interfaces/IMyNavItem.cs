using System;
using System.ComponentModel;
using System.Windows;

namespace DudesPlayer.Desktop.Views.Pages.HomePage
{
    public interface IMyNavItem
    {
        object Content { get; }
        bool IsActive { get; set; }
        bool IsChecked { get; set; }
        bool Cache { get; set; }
        Uri PageSource { get; set; }
        Type PageType { get; set; }
        Uri AbsolutePageSource { get; }
        [Category("Behavior")]
        event RoutedEventHandler Click;
    }
}
