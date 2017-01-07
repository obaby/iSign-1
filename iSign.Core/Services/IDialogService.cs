using System;
namespace iSign.Core
{
    public interface IDialogService
    {
        void ShowDialog (Action<string> okAction);
    }
}
