using System;
using MvvmCross.Core.ViewModels;

namespace iSign.Core.ViewModels
{
    public interface IReloadableViewModel : IMvxViewModel
    {
        void Reload ();
    }
}
