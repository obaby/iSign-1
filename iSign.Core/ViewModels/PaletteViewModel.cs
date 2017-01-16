using System;
using System.Collections.Generic;
using iSign.Services;
using MvvmCross.Core.ViewModels;

namespace iSign.Core
{
    public class PaletteViewModel : MvxViewModel
    {
        public PaletteViewModel ()
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
            foreach (var color in colors) {
                var vm = new PaletteColorViewModel (color);
                vm.Selected += Vm_Selected;
                list.Add (vm);
            }
            PaletteColors = new List<PaletteColorViewModel> (list);
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

        void Vm_Selected (object sender, EventArgs e)
        {
            if (SelectedColor != null && SelectedColor != sender) {
                SelectedColor.IsSelected = false;
            }
            SelectedColor = (PaletteColorViewModel)sender;
        }
    }
}
