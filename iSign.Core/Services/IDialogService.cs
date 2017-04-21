using iSign.Core.ViewModels;

namespace iSign.Core.Services
{
    public interface IDialogService
    {
        void ShowTextDialog ();
        void ShowImageDialog (IReloadableViewModel context);
        void HideDialog ();
    }
}
