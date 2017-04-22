using System;
using iSign.Core.Services;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;

namespace iSign.Core.ViewModels
{
    public class SigningDocViewModel : BaseViewModel
    {
        private IDialogService DialogService { get; }
        private IPdfGeneratorService PdfGeneratorService { get; }
        public SigningViewModel SigningViewModel { get; }
        public string Filename { get; set; }

        public SigningDocViewModel (IViewModelServices viewModelServices, IDialogService dialogService, SigningViewModel signingViewModel, IPdfGeneratorService pdfGeneratorService) : base (viewModelServices)
        {
            DialogService = dialogService;
            SigningViewModel = signingViewModel;
            PdfGeneratorService = pdfGeneratorService;

            AddTextCommand = new MvxCommand (AddLabel);
            AddImageCommand = new MvxCommand (AddImage);
            RotateCommand = new MvxCommand (RotateImage);
            GeneratePdfCommand = new MvxCommand (GeneratePdf);
            LoadFileCommand = new MvxCommand (LoadFile);

            ChangeButtonEnablity (false);
            CanLoadFile = true;
        }

        public IMvxCommand AddImageCommand { get; }
        public IMvxCommand AddTextCommand { get; }
        public IMvxCommand GeneratePdfCommand { get; }
        public IMvxCommand RotateCommand { get; }
        public IMvxCommand LoadFileCommand { get; }

        private void AddLabel ()
        {
            var context = Mvx.Resolve<DialogTextViewModel> ();
            ChangeButtonEnablity (false);

            context.OkAction = t => ChangeButtonEnablity (true);
            context.CancelAction = () => ChangeButtonEnablity (true);

            DialogService.ShowTextDialog (context);
        }

        private void AddImage ()
        {
            var context = Mvx.Resolve<SigningViewModel> ();
            ChangeButtonEnablity (false);

            context.OkAction = t => ChangeButtonEnablity (true);
            context.CancelAction = () => ChangeButtonEnablity (true);

            DialogService.ShowImageDialog (context);
        }

        public void GeneratePdf ()
        {
            PdfGeneratorService.Generate (Filename);
        }

        public event EventHandler<Action> OnLoadFile;
        private void LoadFile ()
        {
            OnLoadFile?.Invoke (this, () => ChangeButtonEnablity (true));
        }

        public event EventHandler OnRotatedImage;
        private void RotateImage ()
        {
            OnRotatedImage?.Invoke (this, EventArgs.Empty);
        }

        public bool CanGeneratePdf { get; set; }
        public bool CanAddSignature { get; set; }
        public bool CanAddText { get; set; }
        public bool CanLoadFile { get; set; }
        public bool CanRotate { get; set; }

        private void ChangeButtonEnablity (bool enabled)
        {
            CanAddText = enabled;
            CanAddSignature = enabled;
            CanGeneratePdf = enabled;
            CanRotate = enabled;
            CanLoadFile = enabled;

            RaisePropertyChanged (() => CanAddText);
            RaisePropertyChanged (() => CanAddSignature);
            RaisePropertyChanged (() => CanGeneratePdf);
            RaisePropertyChanged (() => CanRotate);
            RaisePropertyChanged (() => CanLoadFile);
        }
    }
}
