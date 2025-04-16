using TuApp.Entidades;
using TuApp.VistasModelo;

namespace TuApp.Vistas
{
    public partial class DetalleEventoPage : ContentPage
    {
        public DetalleEventoPage(Evento evento)
        {
            InitializeComponent();
            BindingContext = new EventoViewModel(evento); // ? ahora pasamos el ViewModel
        }

        private async void OnEditarEventoClicked(object sender, EventArgs e)
        {
            if (BindingContext is EventoViewModel vm)
            {
                var evento = new Evento
                {
                    IdEvento = vm.IdEvento,
                    Titulo = vm.Titulo,
                    Descripcion = vm.Descripcion,
                    FechaHora = vm.Fecha.Date + vm.Hora,
                    IdPrioridad = vm.PrioridadSeleccionada.IdPrioridad
                };

                await Navigation.PushAsync(new EditarEventoPage(evento));
            }
        }
    }
}
