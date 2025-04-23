using Newtonsoft.Json;
using TuApp.Entidades.Entity;
using TuApp.Entidades.Req.ReqUsuario;
using TuApp.Entidades.Res.ResUsuario;

namespace TuApp.Vistas;

public partial class EditarPinPacientePage : ContentPage
{
    public EditarPinPacientePage()
    {
        InitializeComponent();
    }
    private async void RegresarInicio_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new InicioPaciente());
    }
    private async void ActualizarPin_Clicked(object sender, EventArgs e)
    {
        
     
        ReqActualizarPinPaciente req = new ReqActualizarPinPaciente();
        req.PinActual = PinActualEntry.Text;
        req.NuevoPin = NuevoPinEntry.Text;
        req.IdUsuario = SesionActiva.sesionActiva.usuario.IdUsuario;

        HttpResponseMessage respuestaHttp = new HttpResponseMessage();

            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), System.Text.Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "usuario/actualizarping", jsonContent);
            }

            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();

                ResActualizarPinPaciente res = new ResActualizarPinPaciente();
                res = JsonConvert.DeserializeObject<ResActualizarPinPaciente>(responseContent);

                if (res.resultado)
                {
                SesionActiva.sesionActiva.usuario.pin.Codigo = req.NuevoPin;
                await DisplayAlert("Actualización correcta", "Pin actualizado correctamente", "Aceptar");
                await Navigation.PushAsync(new InicioPaciente());

            }
                else
                {
                    await DisplayAlert("Error cambiando pin", "Pin actual erroneo", "Aceptar");
                }

            }
            else
            {
                await DisplayAlert("Error de conexion", "No hay respuesta del servidor", "Aceptar");


            }
        
    }
}