using System;

namespace iSign.Core.Services
{
    public interface IDialogService
    {
        void ShowDialog (Action<string> okAction);
    }
}
