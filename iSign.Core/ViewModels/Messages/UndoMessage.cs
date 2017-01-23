using System;
using MvvmCross.Plugins.Messenger;

namespace iSign.Core
{
    public class UndoMessage : MvxMessage
    {
        public UndoMessage (object sender) : base(sender)
        {
        }
    }
}
