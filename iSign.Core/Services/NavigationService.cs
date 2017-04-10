using iSign.Core.ViewModels;
using iSign.Services.Attributes;
using MvvmCross.Core.ViewModels;

namespace iSign.Core.Services
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
