using iSign.Services.Attributes;
using MvvmCross.Plugins.Messenger;

namespace iSign.Core.Services
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