using CommunityToolkit.Maui.Views;
using TuApp.Entidades;
using TuApp.Styles;
using TuApp.VistasModelo;

namespace TuApp.Vistas
{
    public partial class DetalleEventoPage : ContentPage
    {
        private readonly EventoViewModel viewModel;
        private CustomAlertDialog customAlertDialog;
        private Evento evento;
        public DetalleEventoPage(Evento evento)
        {
            InitializeComponent();
            BindingContext = new EventoViewModel(evento);
            this.viewModel = (EventoViewModel)BindingContext; // ? ahora pasamos el ViewModel
            this.evento = evento;
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

        private async void AgregarPaciente_Clicked(object sender, EventArgs e)
        {
            var popup = new InsertarPacienteEventoPopup();
            var paciente = await this.ShowPopupAsync(popup) as Usuario;
            if (paciente != null)
            {
                await viewModel.GuardarAsignaciones(paciente, evento);
            }
        }
    }
}
