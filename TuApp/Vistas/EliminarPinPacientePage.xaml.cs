using Microsoft.Maui;
using Newtonsoft.Json;
using TuApp.Entidades.Entity;
using TuApp.Entidades.Req.ReqUsuario;
using TuApp.Entidades.Res.ResUsuario;

namespace TuApp.Vistas;

public partial class EliminarPinPacientePage : ContentPage
{
	public EliminarPinPacientePage()
	{
		InitializeComponent();
	}

    private async void RegresarInicio_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new InicioPaciente());
    }
    private async void EliminarPin_Clicked(object sender, EventArgs e)
    {
        ReqEliminarPinPaciente req = new ReqEliminarPinPaciente();
        req.Codigo = PinEntry.Text;
        req.IdUsuario = SesionActiva.sesionActiva.usuario.IdUsuario;

        HttpResponseMessage respuestaHttp = new HttpResponseMessage();

        var jsonContent = new StringContent(JsonConvert.SerializeObject(req), System.Text.Encoding.UTF8, "application/json");

        using (HttpClient httpClient = new HttpClient())
        {
            respuestaHttp = await httpClient.PostAsync(App.API_URL + "usuario/eliminarping", jsonContent);
        }

        if (respuestaHttp.IsSuccessStatusCode)
        {
            var responseContent = await respuestaHttp.Content.ReadAsStringAsync();

            ResEliminarPinPaciente res = new ResEliminarPinPaciente();
            res = JsonConvert.DeserializeObject<ResEliminarPinPaciente>(responseContent);

            if (res.resultado)
            {
                SesionActiva.sesionActiva.usuario.pin.Codigo = null;
                await DisplayAlert("Eliminacion correcta", "Pin eliminado correctamente", "Aceptar");
                await Navigation.PushAsync(new InicioPaciente());

            }
            else
            {
                await DisplayAlert("Error eliminado pin", "pin erroneo", "Aceptar");
            }

        }
        else
        {
            await DisplayAlert("Error de conexion", "No hay respuesta del servidor", "Aceptar");


        }
    }
}