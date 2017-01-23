using MvvmCross.Plugins.Messenger;

namespace iSign.Core
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
