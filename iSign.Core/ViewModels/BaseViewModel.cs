using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using System.Collections.Generic;

namespace iSign.Core
{
	public class BaseViewModel : MvxViewModel
	{
        protected INavigationService NavigationService { get; }
        private IMvxMessenger Messenger { get; }
        private List<MvxSubscriptionToken> Tokens { get; }
		public BaseViewModel(IViewModelServices viewModelService)
		{
            NavigationService = viewModelService.NavigationService;
            Messenger = viewModelService.Messenger;
            Tokens = new List<MvxSubscriptionToken> ();
		}

        protected void PublishMessage<T> (T message) where T : MvxMessage
        {
            Messenger.Publish (message);
        }

        protected void Subscribe<T> () where T : MvxMessage
        {
            
        }
	}
}
