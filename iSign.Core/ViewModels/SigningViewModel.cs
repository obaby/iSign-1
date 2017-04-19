using System.Diagnostics.CodeAnalysis;
using iSign.Core.Services;
using iSign.Services;

namespace iSign.Core.ViewModels
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class SigningViewModel : BaseViewModel, IReloadableViewModel
    {
        public PaletteViewModel PaletteContext { get; }
        private const string DefaultColor = Colors.Black;

        public SigningViewModel (IViewModelServices viewModelService, PaletteViewModel paletteContext) : base(viewModelService)
        {
            PaletteContext = paletteContext;
            PaletteContext.PropertyChanged += PaletteContext_PropertyChanged;
            PaletteContext.SetDefaultColor (DefaultColor);
            AddSignatureTxt = "Add signature";
            CancelTxt = "Cancel";
        }

        public string AddSignatureTxt { get; private set;}
        public string CancelTxt { get;  }

        void PaletteContext_PropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof (PaletteContext.SelectedColor)) {
                RaisePropertyChanged (nameof (DrawingColor));
            }

            if (e.PropertyName == nameof (PaletteContext.PointThickness)) {
                RaisePropertyChanged (nameof (Thickness));
            }
        }

        public string DrawingColor => PaletteContext.SelectedColor?.Color;
        public float Thickness => PaletteContext.PointThickness;

        public void Reload ()
        {
            AddSignatureTxt = "Modify";
            RaisePropertyChanged (nameof (AddSignatureTxt));
        }
    }
}
