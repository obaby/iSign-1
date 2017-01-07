using System;
using MvvmCross.Core.ViewModels;

namespace iSign.Core
{
    public class SigningDocViewModel : BaseViewModel
    {
        private IDialogService DialogService { get; }
        public SigningDocViewModel (INavigationService navigationService, IDialogService dialogService) : base(navigationService)
        {
            AddLabelCommand = new MvxCommand (AddLabel);
            DialogService = dialogService;
        }

        public IMvxCommand AddLabelCommand { get; }
        private void AddLabel ()
        {
            DialogService.ShowDialog (OnInputSet);
        }

        public event EventHandler<string> InputSet;
        private void OnInputSet (string input)
        {
            InputSet?.Invoke (this, input);
        }
    }
}
