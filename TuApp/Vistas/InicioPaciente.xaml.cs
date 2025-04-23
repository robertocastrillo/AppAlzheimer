using TuApp.Entidades.Entity;
using TuApp.Vistas;
namespace TuApp.Vistas;

public partial class InicioPaciente : ContentPage
{
	public InicioPaciente()
	{
		InitializeComponent();
        if (SesionActiva.sesionActiva != null && SesionActiva.sesionActiva.usuario != null)
        {
            lblBienvenida.Text = $"Bienvenido {SesionActiva.sesionActiva.usuario.Nombre}..!!";
        }
        else
        {
            lblBienvenida.Text = "Bienvenido..!!";
        }

    }

    private async void OnPerfilClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PerfilPage());
    }

    private async void OnJuegosClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new JuegosPaciente());
    }

    private async void OnEventosClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EventosPacientePage());
    }

    private async void OnChatsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ChatsPacientePage());
    }
        
    private async void OnConfiguracionClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ConfiguracionPacientePage());
    }

}