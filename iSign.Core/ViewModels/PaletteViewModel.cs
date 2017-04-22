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
            var textures = new List<Texture> {
                new Texture{ Color = Color.Red},
                new Texture{ Color = Color.Blue},
                new Texture{ Color = Color.Yellow},
                new Texture{ Color = Color.Green},
                new Texture{ Path = "PencilTexture.png"},
                new Texture{ Path = "BluePenTexture.png"},
                new Texture{ Color = Color.Black},
                new Texture{ Color = Color.White},
            };

            var list = new List<PaletteColorViewModel> ();
            var i = 0;
            foreach (var texture in textures) {
                var vm = new PaletteColorViewModel (viewModelServices, texture, i);
                list.Add (vm);
                i++;
            }
            PaletteColors = new List<PaletteColorViewModel> (list);
            MaxThickness = 10;
            MinThickness = 1;
            PointThickness = 2;
        }

        public void SetDefaultColor (Texture defaultTexture)
        {
            foreach (var vm in PaletteColors) {
                if (vm.Texture == defaultTexture) {
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

        public void SetSelectedColor (Texture texture)
        {
            SelectedColor?.Unselect ();
            SelectedColor = PaletteColors.First (x => x.Texture == texture);
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
