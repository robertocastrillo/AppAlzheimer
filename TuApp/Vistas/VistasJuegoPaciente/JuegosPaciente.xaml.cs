using TuApp.Entidades;
using TuApp.Entidades.Entity;
using TuApp.VistasModelo;

namespace TuApp;

public partial class JuegosPaciente : ContentPage
{
    private readonly JuegoViewModel viewModel;
    public JuegosPaciente()
	{
		InitializeComponent();
        viewModel = new JuegoViewModel();
        BindingContext = viewModel;
    }

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is JuegoPaciente juegoSeleccionado)
        {
            int idUsuarioPaciente = SesionActiva.sesionActiva.usuario.IdUsuario;

            await Navigation.PushAsync(new ResponderPregunta(juegoSeleccionado, idUsuarioPaciente));

            // Desmarcar selección al volver
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}