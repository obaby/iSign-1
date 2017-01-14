using System;
using iSign.Core;
using MvvmCross.Core.ViewModels;
using MvvmCross.iOS.Platform;
using MvvmCross.Platform;
using UIKit;

namespace iSign
{
    public class Setup : MvxIosSetup
    {
        private UIWindow CurrentWindow { get; }
        private IUIApplicationDelegate AppDelegate { get; }

        public Setup (IMvxApplicationDelegate appDelegate, UIWindow window)
            : base (appDelegate, window)
        {
            CurrentWindow = window;
            AppDelegate = appDelegate;
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
        }
    }
}
