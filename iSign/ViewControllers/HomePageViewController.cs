using System;
using iSign.Core;
using MvvmCross.iOS.Views;
using UIKit;

namespace iSign
{
	public partial class HomePageViewController : MvxViewController<HomePageViewModel>
	{
		public HomePageViewController() : base("HomePageView", null)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

