using System;
using System.Collections.Generic;
using iSign.Core.Exceptions;
using iSign.Core.Services;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;

namespace iSign.Core.ViewModels
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
