using Newtonsoft.Json;
using System.Text;

namespace TuApp;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
	}
    private async void btnRegistrarse_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
            string.IsNullOrWhiteSpace(txtCorreo.Text) ||
            string.IsNullOrWhiteSpace(txtPassword.Text)||
            pickerTipoUsuario.SelectedIndex == -1)

        {
            await DisplayAlert("Campos incompletos", "Por favor completa los campos obligatorios.", "Aceptar");
            return;
        }

        var req = new ReqInsertarUsuario
        {
            Nombre = txtNombre.Text,
            CorreoElectronico = txtCorreo.Text,
            Contrasena = txtPassword.Text,
            Direccion = txtDireccion.Text,
            FechaNacimiento = dpFechaNacimiento.Date,
            FotoPerfil = null,
            IdTipoUsuario = pickerTipoUsuario.SelectedIndex == 0 ? 1 : 2
        };


        var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

        HttpResponseMessage respuestaHttp;
        using (HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri("https://localhost:44347/api/");
            respuestaHttp = await client.PostAsync("usuario/insertar", jsonContent);
        }

        if (respuestaHttp.IsSuccessStatusCode)
        {
            var content = await respuestaHttp.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<ResInsertarUsuario>(content);

            if (res.resultado)
            {
                await DisplayAlert("Éxito", "Usuario registrado correctamente", "Aceptar");
                await Navigation.PopAsync(); // Volver al login
            }
            else
            {
                await DisplayAlert("Error", res.listaDeErrores.FirstOrDefault()?.error ?? "Ocurrió un error", "Aceptar");
            }
        }
        else
        {
            await DisplayAlert("Error", "No se pudo conectar con el servidor", "Aceptar");
        }
    }

    private async void btnVolver_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
