using System.Diagnostics.CodeAnalysis;
using iSign.Core.Services;
using iSign.Services;

namespace iSign.Core.ViewModels
{
    [SuppressMessage ("ReSharper", "ExplicitCallerInfoArgument")]
    public class SigningViewModel : DialogViewModel
    {
        public PaletteViewModel PaletteContext { get; }
        private Texture DefaultColor = new Texture { Color = Color.Black };

        public SigningViewModel (IViewModelServices viewModelService, PaletteViewModel paletteContext) : base (viewModelService)
        {
            PaletteContext = paletteContext;
            PaletteContext.PropertyChanged += PaletteContext_PropertyChanged;
            PaletteContext.SetDefaultColor (DefaultColor);
            OkTxt = "Add signature";
        }

        void PaletteContext_PropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof (PaletteContext.SelectedColor)) {
                RaisePropertyChanged (nameof (DrawingColor));
            }

            if (e.PropertyName == nameof (PaletteContext.PointThickness)) {
                RaisePropertyChanged (nameof (Thickness));
            }
        }

        public Texture DrawingColor => PaletteContext.SelectedColor?.Texture;
        public float Thickness => PaletteContext.PointThickness;

        public override void Reload ()
        {
            OkTxt = "Modify";
            RaisePropertyChanged (nameof (OkTxt));
        }
    }
}
