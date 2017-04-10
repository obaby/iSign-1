using MvvmCross.Plugins.Messenger;

namespace iSign.Core.Services
{
    public interface IViewModelServices
    {
        IMvxMessenger Messenger { get; }
        INavigationService NavigationService { get; }
    }
}
