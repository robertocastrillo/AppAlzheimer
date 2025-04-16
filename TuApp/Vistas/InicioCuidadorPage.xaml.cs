using Newtonsoft.Json;
using System.Text;
using TuApp.Entidades;
using TuApp.Entidades.Entity;

namespace TuApp.Vistas;

public partial class InicioCuidadorPage : ContentPage
{
    public InicioCuidadorPage()
    {
        InitializeComponent();

        if (SesionActiva.sesionActiva != null && SesionActiva.sesionActiva.usuario != null)
        {
            lblBienvenida.Text = $"Bienvenido {SesionActiva.sesionActiva.usuario.Nombre}..!!";
        }

    }

    private async void btnRealacion_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RelacionPage()); 
    }
    private async void btnMensajes_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MensajesPage());
    }
    private async void btnJuegos_Cliked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new JuegoCuidador());
    }
    private async void btnEventos_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EventosPage());
    }

}