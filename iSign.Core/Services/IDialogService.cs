using iSign.Core.ViewModels;

namespace iSign.Core.Services
{
    public interface IDialogService
    {
        void ShowTextDialog (DialogViewModel context);
        void ShowImageDialog (DialogViewModel context);
    }
}
