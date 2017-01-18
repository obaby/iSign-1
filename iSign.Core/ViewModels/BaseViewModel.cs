using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using System.Collections.Generic;
using System;

namespace iSign.Core
{
	public class BaseViewModel : MvxViewModel
	{
        protected INavigationService NavigationService { get; }
        private IMvxMessenger Messenger { get; }
        private Dictionary<Type, MvxSubscriptionToken> Tokens { get; }
		public BaseViewModel(IViewModelServices viewModelService)
		{
            NavigationService = viewModelService.NavigationService;
            Messenger = viewModelService.Messenger;
            Tokens = new Dictionary<Type, MvxSubscriptionToken> ();
		}

        protected void PublishMessage<T> (T message) where T : MvxMessage
        {
            Messenger.Publish (message);
        }

        protected void Subscribe<T> (Action<T> handleAction) where T : MvxMessage
        {
            if (Tokens.ContainsKey (typeof (T))) throw new AlreadyListeningThisMessageException();
            var token = Messenger.Subscribe(handleAction);
            Tokens.Add (typeof(T), token);
        }

        protected void Unsubscribe<T> () where T : MvxMessage
        {
            if (Tokens.ContainsKey (typeof (T))) {
                Tokens [typeof (T)]?.Dispose ();
                Tokens.Remove (typeof (T));
            }
        }
    }
}
