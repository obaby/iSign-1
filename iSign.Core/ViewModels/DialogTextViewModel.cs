using iSign.Core.Services;

namespace iSign.Core.ViewModels
{
    public class DialogTextViewModel : DialogViewModel
    {
        public DialogTextViewModel (IViewModelServices viewModelService) : base (viewModelService)
        {
        }

        public override void Reload ()
        {
            OkTxt = "Edit";
            RaisePropertyChanged (() => OkTxt);
        }
    }
}
