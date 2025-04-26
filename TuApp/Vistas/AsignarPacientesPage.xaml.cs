using TuApp.VistasModelo;

namespace TuApp.Vistas
{
    public partial class AsignarPacientesPage : ContentPage
    {
        public AsignarPacientesPage(EventoViewModel eventoViewModel)
        {
            InitializeComponent();
            BindingContext = new AsignarPacientesViewModel(eventoViewModel);
        }
    }
}
