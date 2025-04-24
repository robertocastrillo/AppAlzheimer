using TuApp.Entidades.Entity;
using TuApp.Vistas;

namespace TuApp;

public partial class FlyoutInicioCuidador : FlyoutPage
{
    public FlyoutInicioCuidador()
    {
        InitializeComponent();

        if (SesionActiva.sesionActiva?.usuario != null)
        {
            lblNombreUsuario.Text = SesionActiva.sesionActiva.usuario.Nombre;
        }
    }

    private async void Perfil_Clicked(object sender, EventArgs e)
    {
        await (Detail as NavigationPage)?.Navigation.PushAsync(new PerfilPage());
        IsPresented = false; // Cierra el menú
    }

    private void CerrarSesion_Clicked(object sender, EventArgs e)
    {
        SesionActiva.sesionActiva = null;
        Application.Current.MainPage = new NavigationPage(new MainPage());
    }
}
