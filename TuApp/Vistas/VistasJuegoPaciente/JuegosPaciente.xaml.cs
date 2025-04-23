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


    private async void Juego_Tapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is JuegoPaciente juego)
        {
            int idUsuarioPaciente = SesionActiva.sesionActiva.usuario.IdUsuario;
            await Navigation.PushAsync(new ResponderPregunta(juego, idUsuarioPaciente));
        }
    }
}