using DudesPlayer.Desktop.Views.Pages.RoomsExplorerPage;
using System.Windows;

namespace DudesPlayer.Desktop.Views.Controls
{
    public interface IModalWindowDialog
    {
        bool IsOpen { get; }
        void SetModal(IModalWindow page);
        void Close();
    }
}
