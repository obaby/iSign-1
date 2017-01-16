using System;
using MvvmCross.Plugins.Messenger;

namespace iSign.Core
{
    public interface IViewModelServices
    {
        IMvxMessenger Messenger { get; }
        INavigationService NavigationService { get; }
    }
}
