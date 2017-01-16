using System;
using iSign.Services;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;

namespace iSign.Core
{
    public class PaletteColorViewModel : BaseViewModel
    {
        public string Color { get; }
        public int Id { get; }
        public PaletteColorViewModel (string color, int id)
        {
            Id = id;
            Color = color;
        }
        bool _isSelected;

        public bool IsSelected {
            get {
                return _isSelected;
            }

            set {
                SetProperty (ref _isSelected, value);
            }
        }
    }
}
