using Microsoft.Maui;
using Newtonsoft.Json;
using TuApp.Entidades.Entity;
using TuApp.Entidades.Req.ReqUsuario;
using TuApp.Entidades.Res.ResUsuario;

namespace TuApp.Vistas;

public partial class GenerarPinPacientePage : ContentPage
{
    public GenerarPinPacientePage()
    {
        InitializeComponent();
    }

    private async void RegresarInicio_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new InicioPaciente());
    }
    private async void GenerarPin_Clicked(object sender, EventArgs e)
    {
        ReqGenerarPinPaciente req = new ReqGenerarPinPaciente();
        req.Codigo = PinEntry.Text;
        req.IdUsuario = SesionActiva.sesionActiva.usuario.IdUsuario;

        HttpResponseMessage respuestaHttp = new HttpResponseMessage();

        var jsonContent = new StringContent(JsonConvert.SerializeObject(req), System.Text.Encoding.UTF8, "application/json");

        using (HttpClient httpClient = new HttpClient())
        {
            respuestaHttp = await httpClient.PostAsync(App.API_URL + "usuario/insertarping", jsonContent);
        }

        if (respuestaHttp.IsSuccessStatusCode)
        {
            var responseContent = await respuestaHttp.Content.ReadAsStringAsync();

            ResGenerarPin res = new ResGenerarPin();
            res = JsonConvert.DeserializeObject<ResGenerarPin>(responseContent);

            if (res.resultado)
            {
                SesionActiva.sesionActiva.usuario.pin.Codigo = req.Codigo;
                await DisplayAlert("Creacion correcta", "Pin generado correctamente", "Aceptar");
                await Navigation.PushAsync(new InicioPaciente());

            }
            else
            {
                await DisplayAlert("Error cambiando pin", "erroneo", "Aceptar");
            }

        }
        else
        {
            await DisplayAlert("Error de conexion", "No hay respuesta del servidor", "Aceptar");


        }
    }
}