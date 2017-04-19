using Foundation;
using ObjCRuntime;
using UIKit;

namespace iSign
{
    public class ViewFactory
    {
        public static TView Create<TView> () where TView : UIView
        {
            var arr = NSBundle.MainBundle.LoadNib(typeof (TView).Name, null, null);
            return Runtime.GetNSObject<TView> (arr.ValueAt (0));
        }
    }
}
