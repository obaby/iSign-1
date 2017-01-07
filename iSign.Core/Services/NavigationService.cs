using System;
using iSign.Services;
using MvvmCross.Core.ViewModels;

namespace iSign.Core
{
	[RegisterInterfacesAsSingleton]
	public class NavigationService : MvxNavigatingObject, INavigationService
	{
		public void ShowHomePage()
		{
			ShowViewModel<HomePageViewModel>();
		}

        public void ShowSigningPage ()
        {
            ShowViewModel<SigningDocViewModel> ();
        }
    }
}
