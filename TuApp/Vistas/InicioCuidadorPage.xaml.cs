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
        else
        {
            lblBienvenida.Text = "Bienvenido..!!";
        }
    }
}