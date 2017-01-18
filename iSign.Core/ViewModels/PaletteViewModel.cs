using System;
using System.Collections.Generic;
using System.Linq;
using iSign.Services;
using MvvmCross.Core.ViewModels;

namespace iSign.Core
{
    public class PaletteViewModel : BaseViewModel
    {
        public PaletteViewModel (IViewModelServices viewModelServices) : base(viewModelServices)
        {
            var colors = new List<string> {
                Colors.Red,
                Colors.Blue,
                Colors.Yellow,
                Colors.Green,
                Colors.Purple,
                Colors.Orange,
            };

            var list = new List<PaletteColorViewModel> ();
            var i = 0;
            foreach (var color in colors) {
                var vm = new PaletteColorViewModel (viewModelServices, color, i);
                list.Add (vm);
                i++;
            }
            PaletteColors = new List<PaletteColorViewModel> (list);
            Subscribe<PaletteColorSelectedMessage> (PaletteColorSelectedMessageReceived);
        }

        public IEnumerable<PaletteColorViewModel> PaletteColors { get; }
        PaletteColorViewModel _selectedColor;

        public PaletteColorViewModel SelectedColor {
            get {
                return _selectedColor;
            }

            set {
                SetProperty (ref _selectedColor, value);
            }
        }

        void PaletteColorSelectedMessageReceived (PaletteColorSelectedMessage message)
        {
            if (SelectedColor != null && SelectedColor.Id != message.Id) {
                SelectedColor.Unselect ();
            }
            SelectedColor = PaletteColors.First (x => x.Id == message.Id);
        }

        public void SetSelectedColor (string color)
        {
            if (SelectedColor != null) {
                SelectedColor.Unselect ();
            }
            SelectedColor = PaletteColors.First (x => x.Color == color);
            SelectedColor.Select ();
        }

        public void Undo ()
        {
            PublishMessage (new UndoMessage (this));
        }
    }
}
