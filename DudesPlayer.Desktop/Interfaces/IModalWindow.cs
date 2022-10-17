namespace DudesPlayer.Desktop.Views.Pages.RoomsExplorerPage
{
    public interface IModalWindow
    {
        string Title { get; }
        string SaveButtonTitle { get; }
        void OnCloseButtonClicked();
        void OnSaveButtonClicked();
    }
}
