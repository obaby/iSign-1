using MvvmCross.Plugins.Messenger;

namespace iSign.Core.ViewModels.Messages
{
    public class ViewActivatedMessage : MvxMessage
    {
        public int ViewId { get; }
        public ViewActivatedMessage (object sender, int viewId) : base(sender)
        {
            ViewId = viewId;
        }
    }
}
