using MvvmCross.Core.ViewModels;

namespace iSign.Core
{
	public class BaseViewModel : MvxViewModel
	{
        protected INavigationService NavigationService { get; }
		public BaseViewModel(INavigationService navigationService)
		{
            NavigationService = navigationService;
		}
	}
}
