using iSign.Services;

namespace iSign.Core
{
    public class SigningViewModel : BaseViewModel
    {
        public PaletteViewModel PaletteContext { get; }
        private const string DefaultColor = Colors.Black;

        public SigningViewModel (IViewModelServices viewModelService, PaletteViewModel paletteContext) : base(viewModelService)
        {
            PaletteContext = paletteContext;
            PaletteContext.PropertyChanged += PaletteContext_PropertyChanged;
            PaletteContext.SetDefaultColor (DefaultColor);
        }

        public string AddSignatureTxt => "Add signature";
        public string CancelTxt => "Cancel";

        void PaletteContext_PropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof (PaletteContext.SelectedColor)) {
                RaisePropertyChanged (nameof (DrawingColor));
            }
        }

        public string DrawingColor => PaletteContext.SelectedColor?.Color;
    }
}
