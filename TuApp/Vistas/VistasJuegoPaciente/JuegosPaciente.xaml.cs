using TuApp.Entidades;
using TuApp.Entidades.Entity;
using TuApp.Vistas;
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
    private async void BtnFlechaVolver_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new InicioPaciente());
    }

    private async void Juego_Tapped(object sender, TappedEventArgs e)
    {
        // Asegurarse de que tenemos un juego válido
        if (e.Parameter is JuegoPaciente juego)
        {
            // Indicar visualmente que fue seleccionado (cambiando el color del Frame)
            if (sender is Element element && element.Parent is Frame frame)
            {
                // Guardar el color original
                var colorOriginal = frame.BackgroundColor;

                // Cambiar a color de selección
                frame.BackgroundColor = Colors.White;

                // Esperar para el efecto visual
                await Task.Delay(100);

                // Restaurar color original
                frame.BackgroundColor = colorOriginal;

                // Navegar a la página de responder pregunta
                int idUsuarioPaciente = SesionActiva.sesionActiva.usuario.IdUsuario;
                await Navigation.PushAsync(new ResponderPregunta(juego, idUsuarioPaciente));
            }
            else if (sender is Frame directFrame)
            {
                // Si el sender es directamente el Frame
                var colorOriginal = directFrame.BackgroundColor;
                directFrame.BackgroundColor = Colors.White;
                await Task.Delay(100);
                directFrame.BackgroundColor = colorOriginal;

                int idUsuarioPaciente = SesionActiva.sesionActiva.usuario.IdUsuario;
                await Navigation.PushAsync(new ResponderPregunta(juego, idUsuarioPaciente));
            }
            else
            {
                // En caso de que no podamos acceder al Frame, al menos navegamos
                int idUsuarioPaciente = SesionActiva.sesionActiva.usuario.IdUsuario;
                await Navigation.PushAsync(new ResponderPregunta(juego, idUsuarioPaciente));
            }
        }
    }
}