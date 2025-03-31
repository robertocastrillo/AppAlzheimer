using Newtonsoft.Json;
using System.Text;
using TuApp.Entidades;
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



        ReqCerrarSesion req = new ReqCerrarSesion
        {
            IdUsuario = SesionActiva.sesionActiva.usuario.IdUsuario,
            Origen = "APP"

        };

        var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

        HttpResponseMessage respuestaHttp = null;

        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri("https://localhost:44347/api/");
            respuestaHttp = await httpClient.PostAsync("usuario/cerrarsesion", jsonContent);
        }

        if (respuestaHttp.IsSuccessStatusCode)
        {
            var contenido = await respuestaHttp.Content.ReadAsStringAsync();

            ResCerrarSesion res = new ResCerrarSesion();
            res = JsonConvert.DeserializeObject<ResCerrarSesion>(contenido);

            if (res != null && res.resultado)
            {
                Sesion sesion = new Sesion();

                SesionActiva.sesionActiva = sesion;
                await Navigation.PushAsync(new InicioPage());

                
            }
            else
            {
                await DisplayAlert("Login incorrecto", "Credenciales incorrectas", "Aceptar");
            }
        }
        else
        {
            await DisplayAlert("Error", "No se pudo conectar con el servidor", "Aceptar");
        }
    }
}

