using MvvmCross.Plugins.Messenger;

namespace iSign.Core.ViewModels.Messages
{
    public class PaletteColorSelectedMessage : MvxMessage
    {
        public int Id { get; }
        public PaletteColorSelectedMessage (object sender, int id) : base(sender)
        {
            Id = id;
        }
    }
}
