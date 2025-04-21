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
    private JuegoCuidador juego;
    public DetalleJuego(JuegoCuidador juego)
    {
        InitializeComponent();

        try
        {
            viewModel = BindingContext as PreguntaViewModel;
            this.juego = juego;
            CargarDatos(juego);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cargando DetalleJuego: {ex.Message}");
        }
    }

    private async void CargarDatos(JuegoCuidador juegocuidador)
    {
        await viewModel.CargarPreguntas(juegocuidador.idJuego);

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

        if (pregunta != null)
        {
            int idJuego = juego.idJuego;
            int idUsuario = SesionActiva.sesionActiva.usuario.IdUsuario;

            await Navigation.PushAsync(new DetallePregunta(pregunta, viewModel, idJuego, idUsuario));
        }
    }



}

