using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using iSign.Core.Services;
using iSign.Core.ViewModels.Messages;
using iSign.Services;
using MvvmCross.Core.ViewModels;

namespace iSign.Core.ViewModels
{
    public class PaletteViewModel : BaseViewModel
    {
        public PaletteViewModel (IViewModelServices viewModelServices) : base(viewModelServices)
        {
            Subscribe<PaletteColorSelectedMessage> (PaletteColorSelectedMessageReceived);
            var colors = new List<string> {
                Colors.Red,
                Colors.Blue,
                Colors.Yellow,
                Colors.Green,
                Colors.Purple,
                Colors.Orange,
                Colors.Black,
                Colors.White
            };

            var list = new List<PaletteColorViewModel> ();
            var i = 0;
            foreach (var color in colors) {
                var vm = new PaletteColorViewModel (viewModelServices, color, i);
                list.Add (vm);
                i++;
            }
            PaletteColors = new List<PaletteColorViewModel> (list);
            MaxThickness = 10;
            MinThickness = 1;
            PointThickness = 2;
        }

        public void SetDefaultColor (string defaultColor)
        {
            foreach (var vm in PaletteColors) {
                if (vm.Color == defaultColor) {
                    vm.IsSelected = true;
                    return;
                }
            }
        }

        public IEnumerable<PaletteColorViewModel> PaletteColors { get; }
        PaletteColorViewModel _selectedColor;

        public PaletteColorViewModel SelectedColor {
            get { return _selectedColor; }
            set { SetProperty (ref _selectedColor, value); }
        }

        private void PaletteColorSelectedMessageReceived (PaletteColorSelectedMessage message)
        {
            if (SelectedColor != null && SelectedColor.Id != message.Id) {
                SelectedColor.Unselect ();
            }
            SelectedColor = PaletteColors.First (x => x.Id == message.Id);
        }

        public void SetSelectedColor (string color)
        {
            SelectedColor?.Unselect ();
            SelectedColor = PaletteColors.First (x => x.Color == color);
            SelectedColor.Select ();
        }

        public ICommand UndoCommand => new MvxCommand (Undo);

        private void Undo ()
        {
            PublishMessage (new UndoMessage (this));
        }

        public string UndoText => "Undo";

        private float _pointThickness;

        public float PointThickness {
            get { return _pointThickness; }

            set {
                _pointThickness = value;
                RaisePropertyChanged ();
            }
        }

        public float MaxThickness { get; }
        public float MinThickness { get; }
    }
}
