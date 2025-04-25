using Newtonsoft.Json;
using System.Text;
using TuApp.Entidades;
using TuApp.Entidades.Entity;
using TuApp.VistasModelo;
using TuApp.Styles; // Añadido para importar los custom dialogs

namespace TuApp;

public partial class ResponderPregunta : ContentPage
{
    private PreguntaViewModel viewModel;
    private int preguntaActualIndex = 0;
    private List<bool> respuestasCorrectas = new();
    private JuegoPaciente juegoSeleccionado;
    private int idUsuario;

    // Añadir los diálogos personalizados
    private CustomAlertDialog customAlertDialog;
    private NoChangesDialog noChangesDialog;

    public ResponderPregunta(JuegoPaciente juegoSeleccionado, int idUsuario)
    {
        InitializeComponent();

        // Crear los diálogos personalizados
        customAlertDialog = new CustomAlertDialog();
        noChangesDialog = new NoChangesDialog();

        // Guardar el contenido original
        var originalContent = Content;

        // Crear una nueva grid para contener todo
        var mainGrid = new Grid();

        // Añadir el contenido original
        mainGrid.Children.Add(originalContent);

        // Añadir los diálogos (inicialmente invisibles)
        mainGrid.Children.Add(customAlertDialog);
        mainGrid.Children.Add(noChangesDialog);

        // Establecer la grid como el nuevo contenido
        Content = mainGrid;

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
            // Reemplazar DisplayAlert con customAlertDialog
            await customAlertDialog.ShowAsync("Aviso", "No se encontraron preguntas.", "OK");
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
        // Verificar que tengamos una opción válida
        if (e.Parameter is not Opcion opcionSeleccionada)
            return;

        // Obtener el frame que se tocó
        Frame frameSeleccionado = null;

        // Esta es una forma más robusta de encontrar el frame
        if (sender is TappedEventArgs)
        {
            // El sender es el event args
            var element = (sender as TappedEventArgs).Parameter as Element;
            while (element != null && !(element is Frame))
            {
                element = element.Parent as Element;
            }
            frameSeleccionado = element as Frame;
        }
        else if (sender is Frame)
        {
            // El sender es directamente el frame
            frameSeleccionado = sender as Frame;
        }
        else if (sender is Element)
        {
            // El sender es un elemento, buscar su frame padre
            Element element = sender as Element;
            while (element != null && !(element is Frame))
            {
                element = element.Parent as Element;
            }
            frameSeleccionado = element as Frame;
        }

        // Evaluar si la opción es correcta
        bool esCorrecta = opcionSeleccionada.Condicion;
        respuestasCorrectas.Add(esCorrecta);

        if (frameSeleccionado == null)
        {
            // Si no podemos encontrar el frame, al menos procesamos la selección
            // y continuamos con la siguiente pregunta
            preguntaActualIndex++;

            if (preguntaActualIndex < viewModel.ListaPregunta.Count)
            {
                MostrarPregunta();
            }
            else
            {
                await EnviarPuntaje();
            }

            return;
        }

        // Aplicar color dinámico del tema
        var colorCorrecto = Application.Current.Resources.TryGetValue("OkColor", out var ok) ? (Color)ok : Colors.Green;
        var colorIncorrecto = Application.Current.Resources.TryGetValue("ErrorColor", out var err) ? (Color)err : Colors.Red;

        frameSeleccionado.BackgroundColor = esCorrecta ? colorCorrecto : colorIncorrecto;

        // Deshabilitar las opciones temporalmente para evitar múltiples selecciones
        OpcionesCollection.IsEnabled = false;

        await Task.Delay(500);

        preguntaActualIndex++;

        if (preguntaActualIndex < viewModel.ListaPregunta.Count)
        {
            MostrarPregunta();
            OpcionesCollection.IsEnabled = true;
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
            // Reemplazar DisplayAlert con customAlertDialog
            await customAlertDialog.ShowAsync("Aviso", "No se respondieron preguntas.", "OK");
            return;
        }

        await InsertarPuntaje(juegoSeleccionado.idJuego, correctas, total);
    }

    private void MostrarResultadoFinal()
    {
        lblPregunta.Text = "¡Preguntas respondidas correctamente!";
        OpcionesCollection.IsVisible = false;
        imgPregunta.IsVisible = false;
    
        lblResultadoFinal.IsVisible = true;
        lblResultadoFinal.Text = $"Tuviste {respuestasCorrectas.Count(r => r)} respuestas correctas de {respuestasCorrectas.Count}.";
        btnRegresar.IsVisible = true;
    }

    private async void btnRegresar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    public async Task InsertarPuntaje(int idJuego, int totalCorrectas, int totalPreguntas)
    {
        int puntaje = (int)Math.Round(((double)totalCorrectas / totalPreguntas) * 100);

        var req = new ReqInsertarPuntaje
        {
            idPaciente = SesionActiva.sesionActiva.usuario.IdUsuario,
            idJuego = idJuego,
            puntaje = puntaje
        };

        var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

        using HttpClient httpClient = new();
        var respuestaHttp = await httpClient.PostAsync(App.API_URL + "juego/insertarpuntaje", jsonContent);

        if (respuestaHttp.IsSuccessStatusCode)
        {
            var contenido = await respuestaHttp.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<ResInsertarPuntaje>(contenido);

            if (res?.resultado == true)
            {
                MostrarResultadoFinal();
            }
            else
            {
                // Reemplazar DisplayAlert con customAlertDialog
                await customAlertDialog.ShowAsync("Error", "No se pudo registrar el puntaje.", "OK");
                await Navigation.PopAsync();
            }
        }
        else
        {
            // Reemplazar DisplayAlert con customAlertDialog
            await customAlertDialog.ShowAsync("Error", "Error de conexión al registrar puntaje.", "OK");
            await Navigation.PopAsync();
        }
    }

    private async void BtnFlechaVolver_Clicked(object sender, EventArgs e)
    {
        if (respuestasCorrectas.Count > 0)
        {
            var resultado = await customAlertDialog.ShowWithConfirmationAsync(
                "Advertencia",
                "Si regresas, se perderán las respuestas registradas. ¿Deseas continuar?",
                "Sí", "No");

           
            if (resultado == "Sí")
            {
                respuestasCorrectas.Clear();
                await Navigation.PopAsync();
            }
          
        }
        else
        {
            
            respuestasCorrectas.Clear();
            await Navigation.PopAsync();
        }
    }
}