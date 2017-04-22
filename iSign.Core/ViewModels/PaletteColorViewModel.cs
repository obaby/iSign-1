using System.Diagnostics.CodeAnalysis;
using iSign.Core.Services;
using iSign.Core.ViewModels.Messages;

namespace iSign.Core.ViewModels
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class PaletteColorViewModel : BaseViewModel
    {
        public Texture Texture { get; }
        public int Id { get; }
        public PaletteColorViewModel (IViewModelServices viewModelService, Texture texture, int id) : base(viewModelService)
        {
            Id = id;
            Texture = texture;
        }

        bool _isSelected;

        public bool IsSelected {
            get { return _isSelected; } 
            set {
                PublishMessage (new PaletteColorSelectedMessage (this, Id));
                SetProperty (ref _isSelected, value);
            }
        }

        internal void Unselect ()
        {
            _isSelected = false;
            RaisePropertyChanged (nameof (IsSelected));
        }

        internal void Select ()
        {
            _isSelected = true;
            RaisePropertyChanged (nameof (IsSelected));
        }

   }
}
