using Newtonsoft.Json;
using System.Text;
using TuApp.Entidades;
using TuApp.Entidades.Entity;
using TuApp.VistasModelo;

namespace TuApp;

public partial class ResponderPregunta : ContentPage
{
    private PreguntaViewModel viewModel;
    private int preguntaActualIndex = 0;
    private List<bool> respuestasCorrectas = new();
    private JuegoPaciente juegoSeleccionado;
    private int idUsuario;
    private Opcion opcionSeleccionada;
    private bool? opcionEsCorrecta;
    public ResponderPregunta(JuegoPaciente juegoSeleccionado, int idUsuario)
    {
        InitializeComponent();
        this.BindingContext = viewModel = new PreguntaViewModel();
        this.juegoSeleccionado = juegoSeleccionado;
        this.idUsuario = idUsuario;
        _ = CargarPreguntasAsync();
    }

    private async Task CargarPreguntasAsync()
    {
        await viewModel.CargarPreguntas(juegoSeleccionado.idJuego);

        if (viewModel.ListaPregunta.Any())
        {
            MostrarPregunta();
        }
        else
        {
            await DisplayAlert("Aviso", "No se encontraron preguntas.", "OK");
            await Navigation.PopAsync();
        }
    }

    private void MostrarPregunta()
    {
        if (preguntaActualIndex < viewModel.ListaPregunta.Count)
        {
            var pregunta = viewModel.ListaPregunta[preguntaActualIndex];
            lblPregunta.Text = pregunta.Descripcion;
            OpcionesCollection.ItemsSource = pregunta.opciones;
            imgPregunta.Source = pregunta.ImagenSource;
        }
    }

    private async void Opcion_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not Opcion opcionSeleccionada)
            return;

        // Buscar si es correcta
        bool esCorrecta = opcionSeleccionada.Condicion;
        respuestasCorrectas.Add(esCorrecta);

        // Cambiar el color del Frame seleccionado
        if (sender is Frame frame)
        {
            frame.BackgroundColor = esCorrecta ? Colors.LightGreen : Colors.LightCoral;
        }

        await Task.Delay(500);

        preguntaActualIndex++;

        if (preguntaActualIndex < viewModel.ListaPregunta.Count)
        {
            MostrarPregunta();
        }
        else
        {
            await EnviarPuntaje();

        }
    }

    private async Task EnviarPuntaje()
    {
        int correctas = respuestasCorrectas.Count(r => r);
        int total = respuestasCorrectas.Count;

        if (total == 0)
        {
            await DisplayAlert("Aviso", "No se respondieron preguntas.", "OK");
            return;
        }

        await InsertarPuntaje(juegoSeleccionado.idJuego, correctas, total);
    }

    private void MostrarResultadoFinal()
    {
        lblPregunta.Text = "¡Preguntas respondidas correctamente!";
        OpcionesCollection.IsVisible = false;
        imgPregunta.IsVisible = false;
        btnFlechaVolver.IsVisible = false;
        lblResultadoFinal.IsVisible = true;
        lblResultadoFinal.Text = $"Tuviste {respuestasCorrectas.Count(r => r)} respuestas correctas de {respuestasCorrectas.Count}.";
        btnRegresar.IsVisible = true;
    }

    private async void btnRegresar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync(); // Regresa a JuegosPaciente
    }
    public async Task InsertarPuntaje(int idJuego, int totalCorrectas, int totalPreguntas)
    {
        // Calcular puntaje sobre 100
        int puntaje = (int)Math.Round(((double)totalCorrectas / totalPreguntas) * 100);

        ReqInsertarPuntaje req = new ReqInsertarPuntaje
        {
            idPaciente = SesionActiva.sesionActiva.usuario.IdUsuario,
            idJuego = idJuego,
            puntaje = puntaje
        };

        var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

        HttpResponseMessage respuestaHttp = null;

        using (HttpClient httpClient = new HttpClient())
        {
            respuestaHttp = await httpClient.PostAsync(App.API_URL + "juego/insertarpuntaje", jsonContent);
        }

        if (respuestaHttp.IsSuccessStatusCode)
        {
            var contenido = await respuestaHttp.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<ResInsertarPuntaje>(contenido); // Asumiendo una clase genérica con bool resultado

            if (res != null && res.resultado)
            {
                MostrarResultadoFinal();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo registrar el puntaje.", "OK");
                await Navigation.PopAsync();
            }
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Error de conexión al registrar puntaje.", "OK");
            await Navigation.PopAsync();
        }
    }
    private async void BtnFlechaVolver_Clicked(object sender, EventArgs e)
    {
        if (respuestasCorrectas.Count > 0)
        {
            bool confirmacion = await DisplayAlert(
                "Advertencia",
                "Si regresas, se perderán las respuestas registradas. ¿Deseas continuar?",
                "Sí", "No");

            if (!confirmacion)
                return;
        }

        respuestasCorrectas.Clear(); // Limpia las respuestas
        await Navigation.PopAsync(); // Vuelve a la pantalla anterior
    }
}
