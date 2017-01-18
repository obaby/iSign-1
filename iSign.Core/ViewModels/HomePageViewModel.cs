using System.Windows.Input;
using MvvmCross.Core.ViewModels;

namespace iSign.Core
{
	public class HomePageViewModel : BaseViewModel
	{
        
        public HomePageViewModel(IViewModelServices viewModelServices) : base(viewModelServices)
		{
            GoToSigningDocumentCommand = new MvxCommand (GoToSigningDocument);
		}

        public ICommand GoToSigningDocumentCommand { get; }
        private void GoToSigningDocument ()
        {
            NavigationService.ShowSigningPage ();
        }

        public string SigningPageButtonTxt => "Go to signing docs";
	}
}
