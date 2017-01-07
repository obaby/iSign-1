using System;
using iSign.Core;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using UIKit;

namespace iSign
{
	public partial class HomePageViewController : MvxViewController<HomePageViewModel>
	{
		public HomePageViewController() : base("HomePageView", null)
		{
            this.DelayBind (SetBindings);
		}

        private void SetBindings ()
        {
            var set = this.CreateBindingSet<HomePageViewController, HomePageViewModel> ();
            set.Bind (GoToSignDocButton).To (vm => vm.GoToSigningDocumentCommand);
            set.Bind (GoToSignDocButton).For ("Title").To (vm => vm.SigningPageButtonTxt);
            set.Apply ();
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            GoToSignDocButton.SizeToFit ();
        }
	}
}

