using System.Collections.ObjectModel;
using System.Windows.Input;
using TuApp.Entidades;
using TuApp.Entidades.Res.ResMensaje;
using Newtonsoft.Json;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using TuApp.Entidades.Entity;
using Timer = System.Timers.Timer;
using System.Diagnostics;

namespace TuApp.VistasModelo
{
    public class MensajeViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Usuario> ListaPacientesRelacionados { get; set; } = new();
        public ObservableCollection<Mensaje> ListaMensajes { get; set; } = new();

        private Usuario _pacienteSeleccionado;
        public Usuario PacienteSeleccionado
        {
            get => _pacienteSeleccionado;
            set
            {
                _pacienteSeleccionado = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(NombrePacienteConversacion));
                if (SesionActiva.sesionActiva.usuario.IdTipoUsuario == 2 && value != null)
                    _ = ObtenerMensajesAsync();
            }
        }

        private string _contenidoNuevoMensaje;
        public string ContenidoNuevoMensaje
        {
            get => _contenidoNuevoMensaje;
            set { _contenidoNuevoMensaje = value; OnPropertyChanged(); }
        }

        public string NombrePacienteConversacion =>
            PacienteSeleccionado != null ? $"Conversando con: {PacienteSeleccionado.Nombre}" : string.Empty;

        public ICommand EnviarMensajeCommand { get; }

        private readonly Timer _autoRefreshTimer;

        public int IdUsuarioLogueado => SesionActiva.sesionActiva.usuario.IdUsuario;

        public bool PuedeEnviarMensajes => SesionActiva.sesionActiva.usuario.IdTipoUsuario == 2;

        public MensajeViewModel()
        {
            EnviarMensajeCommand = new Command(async () => await EnviarMensajeAsync());
            _ = Inicializar();

            _autoRefreshTimer = new Timer(5000);
            _autoRefreshTimer.Elapsed += async (s, e) => await AutoRefreshMensajes();
            _autoRefreshTimer.AutoReset = true;
            _autoRefreshTimer.Start();
        }

        private async Task Inicializar()
        {
            try
            {
                var tipo = SesionActiva.sesionActiva.usuario.IdTipoUsuario;

                if (tipo == 2)
                    await CargarPacientesRelacionados();
                else if (tipo == 1)
                    await ObtenerMensajesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Inicialización falló: {ex.Message}");
            }
        }

        private async Task AutoRefreshMensajes()
        {
            if (SesionActiva.sesionActiva.usuario.IdTipoUsuario == 1 || PacienteSeleccionado != null)
                await ObtenerMensajesAsync();
        }

        private async Task CargarPacientesRelacionados()
        {
            ListaPacientesRelacionados.Clear();
            try
            {
                var req = new ReqObtenerRelacion
                {
                    IdUsuario = SesionActiva.sesionActiva.usuario.IdUsuario
                };

                var json = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
                using HttpClient client = new();
                var resp = await client.PostAsync(App.API_URL + "usuario/obtenerrelacion", json);

                if (resp.IsSuccessStatusCode)
                {
                    var content = await resp.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<ResObtenerRelacion>(content);

                    if (res.resultado && res.listaUsuarios != null)
                    {
                        foreach (var item in res.listaUsuarios)
                            ListaPacientesRelacionados.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.Dispatcher.DispatchAsync(() =>
                    App.Current.MainPage.DisplayAlert("Error", ex.Message, "Aceptar")
                );
            }
        }

        private async Task ObtenerMensajesAsync()
        {
            var usuario = SesionActiva.sesionActiva.usuario;

            int idPaciente;
            if (usuario.IdTipoUsuario == 1)
            {
                idPaciente = usuario.IdUsuario;
            }
            else if (PacienteSeleccionado != null)
            {
                idPaciente = PacienteSeleccionado.IdUsuario;
            }
            else
            {
                return;
            }

            var req = new ReqObtenerMensajes { IdPaciente = idPaciente };
            var json = JsonConvert.SerializeObject(req);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using HttpClient client = new();
            var response = await client.PostAsync(App.API_URL + "mensaje/obtener", content);
            if (response.IsSuccessStatusCode)
            {
                var jsonStr = await response.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<ResObtenerMensajes>(jsonStr);
                if (res.resultado)
                {
                    App.Current.MainPage.Dispatcher.Dispatch(() =>
                    {
                        ListaMensajes.Clear();
                        foreach (var msg in res.listaMensajes)
                            ListaMensajes.Add(msg);
                    });
                }
            }
        }

        private async Task EnviarMensajeAsync()
        {
            if (SesionActiva.sesionActiva.usuario.IdTipoUsuario != 2 || string.IsNullOrWhiteSpace(ContenidoNuevoMensaje) || PacienteSeleccionado == null)
                return;

            var req = new ReqInsertarMensaje
            {
                IdCuidador = SesionActiva.sesionActiva.usuario.IdUsuario,
                IdPaciente = PacienteSeleccionado.IdUsuario,
                Contenido = ContenidoNuevoMensaje
            };

            var json = JsonConvert.SerializeObject(req);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using HttpClient client = new();
            var response = await client.PostAsync(App.API_URL + "mensaje/insertar", content);

            if (response.IsSuccessStatusCode)
            {
                ContenidoNuevoMensaje = string.Empty;
                await ObtenerMensajesAsync();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
