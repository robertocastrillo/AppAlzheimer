using System.Collections.ObjectModel;
using TuApp.VistasModelo;

namespace TuApp.Vistas.PopUp
{
    public partial class PopupAsignarPacientes : ContentView
    {
        public PopupAsignarPacientes(EventoViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private async void Guardar_Clicked(object sender, EventArgs e)
        {
            if (BindingContext is EventoViewModel vm)
            {
                await vm.GuardarAsignaciones();
            }
        }
    }
}