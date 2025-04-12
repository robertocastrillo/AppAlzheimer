using TuApp.Entidades;
using TuApp.Entidades.Entity;
using TuApp.VistasModelo;
using CommunityToolkit.Maui.Storage;
using System.IO;
using SkiaSharp;
using CommunityToolkit.Maui.Views;



namespace TuApp.Vistas;

public partial class DetalleJuego : ContentPage
{
    private PreguntaViewModel viewModel;

    public DetalleJuego(ResObtenerJuegosCuidador juego)
    {
        InitializeComponent();

        try
        {
            viewModel = BindingContext as PreguntaViewModel;
            CargarDatos(juego);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cargando DetalleJuego: {ex.Message}");
        }
    }

    private async void CargarDatos(ResObtenerJuegosCuidador juego)
    {
        await viewModel.CargarPreguntas(juego);

        // Si no hay preguntas, muestra una alerta
        if (viewModel.ListaPregunta == null || !viewModel.ListaPregunta.Any())
        {
            await DisplayAlert("Atención", "No se encontraron preguntas para este juego.", "Aceptar");
        }
    }
    private async void PreguntaSeleccionada(object sender, SelectionChangedEventArgs e)
    {
        var pregunta = e.CurrentSelection.FirstOrDefault() as Pregunta;
        if (pregunta != null)
        {
            await Navigation.PushAsync(new DetallePregunta(pregunta));
        }
    ((CollectionView)sender).SelectedItem = null;
    }
    private async void AgregarPregunta_Clicked(object sender, EventArgs e)
    {
        var popup = new InsertarPreguntaPopup();
        var pregunta = await this.ShowPopupAsync(popup) as Pregunta;

        if (pregunta != null && BindingContext is PreguntaViewModel vm)
        {
            vm.ListaPregunta.Add(pregunta);
        }
    }


}

