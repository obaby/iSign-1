using System;
using iSign.Core.Services;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;

namespace iSign.Core.ViewModels
{
    public class SigningDocViewModel : BaseViewModel
    {
        private IDialogService DialogService { get; }
        public SigningViewModel SigningViewModel { get; }
        public string Filename { get; set; }

        public SigningDocViewModel (IViewModelServices viewModelServices, IDialogService dialogService, SigningViewModel signingViewModel) : base (viewModelServices)
        {
            AddTextCommand = new MvxCommand (AddLabel);
            AddImageCommand = new MvxCommand (AddImage);
            DialogService = dialogService;
            SigningViewModel = signingViewModel;
            ChangeButtonEnablity (false);
        }

        public IMvxCommand AddTextCommand { get; }
        private void AddLabel ()
        {
            var context = Mvx.Resolve<DialogTextViewModel> ();
            ChangeButtonEnablity (false);

            context.OkAction = t => ChangeButtonEnablity (true);
            context.CancelAction = () => ChangeButtonEnablity (true);

            DialogService.ShowTextDialog (context);
        }

        public IMvxCommand AddImageCommand { get; }
        private void AddImage ()
        {
            var context = Mvx.Resolve<SigningViewModel> ();
            ChangeButtonEnablity (false);

            context.OkAction = t => ChangeButtonEnablity (true);
            context.CancelAction = () => ChangeButtonEnablity (true);

            DialogService.ShowImageDialog (context);
        }

        public void LoadFile (IFileStorage tool)
        {
            tool.DownloadFile (OnFileDownloaded);
        }

        public event EventHandler<string> FileDownloaded;
        private void OnFileDownloaded (string filename)
        {
            Filename = filename;
            FileDownloaded?.Invoke (this, filename);
            ChangeButtonEnablity (true);
        }

        public bool CanGeneratePdf { get; set; }
        public bool CanAddSignature { get; set; }
        public bool CanAddText { get; set; }

        private void ChangeButtonEnablity (bool enabled)
        {
            CanAddText = enabled;
            CanAddSignature = enabled;
            CanGeneratePdf = enabled;

            RaisePropertyChanged (() => CanAddText);
            RaisePropertyChanged (() => CanAddSignature);
            RaisePropertyChanged (() => CanGeneratePdf);
        }
    }
}
