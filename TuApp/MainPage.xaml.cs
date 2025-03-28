using Newtonsoft.Json;
using System.Text;

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
                httpClient.BaseAddress = new Uri("https://localhost:44347/api/");
                respuestaHttp = await httpClient.PostAsync("usuario/iniciarsesion", jsonContent);
            }

            if (respuestaHttp.IsSuccessStatusCode)
            {
                var contenido = await respuestaHttp.Content.ReadAsStringAsync();
                ResIniciarSesion res = JsonConvert.DeserializeObject<ResIniciarSesion>(contenido);

                if (res != null && res.resultado)
                {
                    Sesion.Usuario = res.Usuario; // Guardas en tu clase estática

                    await Navigation.PushAsync(new InicioPage(Sesion.Usuario));
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


