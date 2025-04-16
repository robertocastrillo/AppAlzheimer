using TuApp.Entidades;
using TuApp.VistasModelo;


namespace TuApp.Vistas
{
    public partial class EditarEventoPage : ContentPage
    {
        public EditarEventoPage(Evento evento)
        {
            InitializeComponent();
            BindingContext = new EventoViewModel(evento); // Constructor especial
        }
    }
}
