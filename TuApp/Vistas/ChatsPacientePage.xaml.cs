using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Text;
using TuApp.Entidades.Entity;
using TuApp.Entidades.Res.Chat;
using TuApp.Entidades;

namespace TuApp.Vistas;
public partial class ChatsPacientePage : ContentPage
{
    
    public ObservableCollection<Mensaje> Mensajes { get; set; } = new ObservableCollection<Mensaje>();

    public ChatsPacientePage()
    {
        InitializeComponent();
        CargarPreguntas();
        BindingContext = this;
    }

    public async Task CargarPreguntas()
    {
        var jsonContent = new StringContent(JsonConvert.SerializeObject(SesionActiva.sesionActiva.usuario.IdUsuario), Encoding.UTF8, "application/json");
        HttpResponseMessage respuestaHttp = null;
        using (HttpClient httpClient = new HttpClient())
        {
            respuestaHttp = await httpClient.PostAsync(App.API_URL + "mensaje/obtener", jsonContent);
        }
        if (respuestaHttp.IsSuccessStatusCode)
        {
            var contenido = await respuestaHttp.Content.ReadAsStringAsync();
            ResCargarChatPaciente res = new ResCargarChatPaciente();
            res = JsonConvert.DeserializeObject<ResCargarChatPaciente>(contenido);
            if (res != null && res.resultado)
            {
                Mensajes.Clear();
                foreach (Mensaje chats in res.chatspaciente)
                {
                    Mensaje chat = new Mensaje();
                    chat.IdUsuarioCuidador = chats.IdUsuarioCuidador;
                    chat.Contenido = chats.Contenido;
                    chat.FechaEnviado = chats.FechaEnviado;
                    Mensajes.Add(chat);
                }
            }
        }
    }
    private async void RegresarInicio_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new InicioPaciente());
    }
}