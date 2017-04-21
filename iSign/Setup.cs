using iSign.Core;
using iSign.Core.Services;
using iSign.IoC;
using iSign.Service;
using MvvmCross.Core.ViewModels;
using MvvmCross.iOS.Platform;
using MvvmCross.Platform;
using UIKit;

namespace iSign
{
    public class Setup : MvxIosSetup
    {
        public Setup (IMvxApplicationDelegate appDelegate, UIWindow window)
            : base (appDelegate, window)
        {
        }

        protected override IMvxApplication CreateApp ()
        {
            return new App ();
        }

        protected override void InitializePlatformServices ()
        {
            base.InitializePlatformServices ();
            Mvx.RegisterType<IDialogService, DialogService> ();
            Mvx.RegisterType<IFileStorageChooser, FileStorageChooser> ();
            Mvx.RegisterType<IPdfGeneratorService, PdfGeneratorService> ();
        }
    }
}
