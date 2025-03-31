using TuApp.Entidades.Entity;
using TuApp.Vistas;

namespace TuApp;

public partial class MainMenuPage : ContentPage
{
    public MainMenuPage()
    {
        InitializeComponent();

    }

    private async void Perfil_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PerfilPage());
    }



    private async void CerrarSesion_Clicked(object sender, EventArgs e)
    {
        SesionActiva.sesionActiva = null; // Limpiar la sesión actual

        // Volver al login
        Application.Current.MainPage = new NavigationPage(new MainPage());
    }

}
