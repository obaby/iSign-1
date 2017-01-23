using iSign.Services;
using MvvmCross.Plugins.Messenger;

namespace iSign.Core
{
    [RegisterInterfacesAsSingleton]
    public class ViewModelServices : IViewModelServices
    {
        public ViewModelServices (IMvxMessenger messenger, INavigationService navigationService)
        {
            Messenger = messenger;
            NavigationService = navigationService;
        }

        public IMvxMessenger Messenger { get; }
        public INavigationService NavigationService { get; }
    }

}