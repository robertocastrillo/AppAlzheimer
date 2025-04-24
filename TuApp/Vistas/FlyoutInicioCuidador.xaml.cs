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
        var navPage = Detail as NavigationPage;

        if (navPage != null)
        {
            var currentPage = navPage.CurrentPage;

            if (currentPage == null || currentPage.GetType() != typeof(PerfilPage))
            {
                await navPage.PushAsync(new PerfilPage());
            }

            // Esperar un pequeño delay mejora la transición visual antes de cerrar el flyout
            await Task.Delay(10);
            IsPresented = false; // ← Cierra el menú lateral
        }
    }



    private void CerrarSesion_Clicked(object sender, EventArgs e)
    {
        SesionActiva.sesionActiva = null;
        Application.Current.MainPage = new NavigationPage(new MainPage());
    }
}
