using Newtonsoft.Json;
using System.Text;
using TuApp.Entidades.Entity;
using TuApp.Vistas;

namespace TuApp
{
    public partial class MainPage : ContentPage
    {


        public MainPage()
        {
            InitializeComponent();
        }
        private async void btnLogin_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCorreo.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                await DisplayAlert("Error", "Por favor, completa todos los campos", "Aceptar");
                return;
            }

            ReqIniciarSesion req = new ReqIniciarSesion
            {
                CorreoElectronico = txtCorreo.Text,
                Contrasena = txtPassword.Text
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            HttpResponseMessage respuestaHttp = null;

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "usuario/iniciarsesion", jsonContent);
            }

            if (respuestaHttp.IsSuccessStatusCode)
            {
                var contenido = await respuestaHttp.Content.ReadAsStringAsync();

                ResIniciarSesion res = new ResIniciarSesion();
                    res = JsonConvert.DeserializeObject<ResIniciarSesion>(contenido);

                if (res != null && res.resultado)
                {
                    SesionActiva.sesionActiva = res.Sesion;
                    if (SesionActiva.sesionActiva.usuario.IdTipoUsuario == 2)
                    {
                        Application.Current.MainPage = new MenuCuidadorPage(); // ← ESTE es el contenedor
                    }
                    if (SesionActiva.sesionActiva.usuario.IdTipoUsuario == 1)
                    {
                        Application.Current.MainPage = new InicioPaciente(); // ← ESTE es el contenedor
                    }
                    else
                    {
                        Application.Current.MainPage = new MenuCuidadorPage();
                    }
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

        private void btnRegistrarse_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new RegisterPage());
        }
    }


}


