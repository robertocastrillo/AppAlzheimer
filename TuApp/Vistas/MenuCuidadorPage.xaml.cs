using TuApp.Entidades.Entity;
using TuApp.Vistas;

namespace TuApp.Vistas;

public partial class MenuCuidadorPage : FlyoutPage
{
	public MenuCuidadorPage()
	{
        InitializeComponent();
	}
    private async void Perfil_Clicked(object sender, EventArgs e)
    {
        await NavigationHelper.Navegar(new PerfilPage());
    }

    private async void Configuracion_Clicked(object sender, EventArgs e)
    {
        //await Navigation.PushAsync(new ConfiguracionPage());
    }

    private async void CerrarSesion_Clicked(object sender, EventArgs e)
    {
        SesionActiva.sesionActiva = null;
        Application.Current.MainPage = new NavigationPage(new MainPage()); // vuelve al login
    }
}
