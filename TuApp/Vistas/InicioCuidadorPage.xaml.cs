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
}