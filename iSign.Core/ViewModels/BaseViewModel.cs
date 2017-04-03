using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using System.Collections.Generic;
using System;

namespace iSign.Core
{
	public class BaseViewModel : MvxViewModel
	{
        protected IViewModelServices ViewModelServices { get; }
        protected INavigationService NavigationService => ViewModelServices.NavigationService;

        private IMvxMessenger Messenger => ViewModelServices.Messenger;
        private Dictionary<Type, MvxSubscriptionToken> Tokens { get; }
		public BaseViewModel(IViewModelServices viewModelService)
		{
            ViewModelServices = viewModelService;
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
