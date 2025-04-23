using TuApp.Entidades;
using TuApp.VistasModelo;

namespace TuApp.Vistas
{
    public partial class EventosPage : ContentPage
    {
        public EventosPage()
        {
            InitializeComponent();
            BindingContext = new EventoViewModel();
        }

        private async void OnAgregarEventoClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InsertarEventoPage());
        }

        private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Evento eventoSeleccionado)
            {
                await Navigation.PushAsync(new DetalleEventoPage(eventoSeleccionado));
                ((CollectionView)sender).SelectedItem = null;
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is VistasModelo.EventoViewModel vm)
            {
                vm.CargarEventosCommand.Execute(null);
            }
        }
        private async void Evento_Tapped(object sender, EventArgs e)
        {
            if (sender is Frame frame && frame.BindingContext is Evento evento)
            {
                await Navigation.PushAsync(new DetalleEventoPage(evento));
            }
        }


    }

}
