using System;
using MvvmCross.Plugins.Messenger;

namespace iSign.Core
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
