using TuApp.Entidades;
using TuApp.Entidades.Entity;
using TuApp.VistasModelo;


namespace TuApp.Vistas;

public partial class DetalleJuego : ContentPage
{
    private PreguntaViewModel viewModel;

    public DetalleJuego(ResObtenerJuegosCuidador juego)
    {
        InitializeComponent();

        viewModel = new PreguntaViewModel();
        BindingContext = viewModel;

        // Llama a un método async aparte
        CargarDatos(juego);
    }

    private async void CargarDatos(ResObtenerJuegosCuidador juego)
    {
        await viewModel.CargarPreguntas(juego);
    }
}

