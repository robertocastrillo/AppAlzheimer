using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using TuApp.Entidades;
using TuApp.Entidades.Entity;
using TuApp.Vistas;
using TuApp.Vistas.PopUp;

namespace TuApp.VistasModelo
{
    public partial class EventoViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Evento> ListaEventos { get; set; } = new();
        public ObservableCollection<UsuarioSeleccionable> PacientesCheckbox { get; set; } = new();

        public ICommand CargarEventosCommand { get; }
        public ICommand InsertarEventoCommand { get; }
        public ICommand EliminarEventoCommand { get; }
        public ICommand ActualizarEventoCommand { get; }
        public ICommand MostrarPopupAsignacionCommand { get; }

        public int IdEvento { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Today;
        public TimeSpan Hora { get; set; } = DateTime.Now.TimeOfDay;

        public Prioridad PrioridadSeleccionada { get; set; }
        public List<Prioridad> ListaPrioridades { get; set; } = new()
        {
            new Prioridad { IdPrioridad = 1, Descripcion = "Alta" },
            new Prioridad { IdPrioridad = 2, Descripcion = "Media" },
            new Prioridad { IdPrioridad = 3, Descripcion = "Baja" }
        };

        public string FechaHoraFormateada => (Fecha.Date + Hora).ToString("dd/MM/yyyy HH:mm");

        public string DescripcionPrioridad =>
            PrioridadSeleccionada?.Descripcion ??
            ListaPrioridades.FirstOrDefault(p => p.IdPrioridad == PrioridadSeleccionada?.IdPrioridad)?.Descripcion ??
            $"Prioridad ID: {PrioridadSeleccionada?.IdPrioridad}";

        public EventoViewModel()
        {
            CargarEventosCommand = new Command(async () => await CargarEventosAsync());
            InsertarEventoCommand = new Command(async () => await InsertarEventoAsync());
        }

        public EventoViewModel(Evento evento) : this()
        {
            IdEvento = evento.IdEvento;
            Titulo = evento.Titulo;
            Descripcion = evento.Descripcion;
            Fecha = evento.FechaHora.Date;
            Hora = evento.FechaHora.TimeOfDay;
            PrioridadSeleccionada = ListaPrioridades.FirstOrDefault(p => p.IdPrioridad == evento.IdPrioridad);

            EliminarEventoCommand = new Command(async () => await EliminarEvento());
            ActualizarEventoCommand = new Command(async () => await ActualizarEventoAsync());
            MostrarPopupAsignacionCommand = new Command(async () => await MostrarPopupAsignacionAsync());
        }

        private async Task CargarEventosAsync()
        {
            ListaEventos.Clear();
            try
            {
                var req = new ReqObtenerEventosCuidador { IdCuidador = SesionActiva.sesionActiva.usuario.IdUsuario };
                var content = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

                using HttpClient client = new();
                var resp = await client.PostAsync("https://localhost:44347/api/evento/obtenereventoscuidador", content);

                if (resp.IsSuccessStatusCode)
                {
                    var jsonRes = await resp.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<List<ResObtenerEventosCuidador>>(jsonRes);

                    if (res != null)
                        foreach (var item in res)
                            ListaEventos.Add(item.evento);
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Aceptar");
            }
        }

        private async Task InsertarEventoAsync()
        {
            if (string.IsNullOrWhiteSpace(Titulo) || PrioridadSeleccionada == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Complete los campos requeridos.", "Aceptar");
                return;
            }

            var req = new ReqInsertarEvento
            {
                IdCuidador = SesionActiva.sesionActiva.usuario.IdUsuario,
                Titulo = Titulo,
                Descripcion = Descripcion,
                FechaHora = Fecha.Date + Hora,
                IdPrioridad = PrioridadSeleccionada.IdPrioridad
            };

            var content = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
            using HttpClient client = new();
            var response = await client.PostAsync("https://localhost:44347/api/evento/insertar", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonRes = await response.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<ResInsertarEvento>(jsonRes);

                if (res.resultado)
                {
                    await App.Current.MainPage.DisplayAlert("Éxito", "Evento insertado correctamente", "Aceptar");
                    Application.Current.MainPage = new MenuCuidadorPage(new EventosPage());
                }
                else
                {
                    var mensaje = res.listaDeErrores.FirstOrDefault()?.error ?? "Error desconocido";
                    await App.Current.MainPage.DisplayAlert("Error", mensaje, "Aceptar");
                }
            }
        }

        private async Task EliminarEvento()
        {
            await App.Current.MainPage.DisplayAlert("Debug", $"ID a eliminar: {IdEvento}", "OK");

            bool confirm = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Deseas eliminar este evento?", "Sí", "No");
            if (!confirm) return;

            var req = new ReqEliminarEvento { IdEvento = IdEvento };
            var json = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using HttpClient client = new();
            var resp = await client.PostAsync("https://localhost:44347/api/evento/eliminar", json);

            if (resp.IsSuccessStatusCode)
            {
                await App.Current.MainPage.DisplayAlert("Éxito", "Evento eliminado", "Aceptar");
                Application.Current.MainPage = new MenuCuidadorPage(new EventosPage());
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo eliminar el evento", "Aceptar");
            }
        }


        private async Task MostrarPopupAsignacionAsync()
        {
            try
            {
                var reqRelacion = new ReqObtenerRelacion { IdUsuario = SesionActiva.sesionActiva.usuario.IdUsuario };
                var jsonRelacion = new StringContent(JsonConvert.SerializeObject(reqRelacion), Encoding.UTF8, "application/json");

                using HttpClient client = new();
                var respRelacion = await client.PostAsync("https://localhost:44347/api/usuario/obtenerrelacion", jsonRelacion);

                if (!respRelacion.IsSuccessStatusCode)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "No se pudieron obtener las relaciones.", "Aceptar");
                    return;
                }

                var relacionesJson = await respRelacion.Content.ReadAsStringAsync();
                var resRelacion = JsonConvert.DeserializeObject<ResObtenerRelacion>(relacionesJson);

                var reqAsignados = new { IdEvento = IdEvento };
                var jsonAsignados = new StringContent(JsonConvert.SerializeObject(reqAsignados), Encoding.UTF8, "application/json");
                var respAsignados = await client.PostAsync("https://localhost:44347/api/evento/obtenerpacientesasignados", jsonAsignados);

                List<int> idsAsignados = new();
                if (respAsignados.IsSuccessStatusCode)
                {
                    var asignadosJson = await respAsignados.Content.ReadAsStringAsync();
                    var asignados = JsonConvert.DeserializeObject<List<PacienteAsignado>>(asignadosJson);
                    idsAsignados = asignados.Select(a => a.id_Usuario).ToList();
                }

                PacientesCheckbox.Clear();
                foreach (var u in resRelacion.listaUsuarios)
                {
                    PacientesCheckbox.Add(new UsuarioSeleccionable
                    {
                        Usuario = u,
                        Seleccionado = idsAsignados.Contains(u.IdUsuario)
                    });
                }

                var popup = new PopupAsignarPacientes(this);
                await App.Current.MainPage.Navigation.PushModalAsync(new ContentPage { Content = popup });
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Aceptar");
            }
        }

        public async Task GuardarAsignaciones()
        {
            foreach (var paciente in PacientesCheckbox)
            {
                object req = paciente.Seleccionado
                    ? new ReqInsertarPacienteEvento
                    {
                        IdEvento = IdEvento,
                        IdCuidador = SesionActiva.sesionActiva.usuario.IdUsuario,
                        IdPaciente = paciente.Usuario.IdUsuario
                    }
                    : new ReqEliminarPacienteEvento
                    {
                        IdEvento = IdEvento,
                        IdCuidador = SesionActiva.sesionActiva.usuario.IdUsuario,
                        IdPaciente = paciente.Usuario.IdUsuario
                    };

                var endpoint = paciente.Seleccionado
                    ? "https://localhost:44347/api/evento/agregarPaciente"
                    : "https://localhost:44347/api/evento/eliminarpaciente";

                var json = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
                using HttpClient client = new();
                await client.PostAsync(endpoint, json);
            }

            await App.Current.MainPage.DisplayAlert("Éxito", "Cambios guardados correctamente", "Aceptar");
            await App.Current.MainPage.Navigation.PopModalAsync();
        }

        private async Task ActualizarEventoAsync()
        {
            // Si implementás edición de evento, ponelo aquí
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public partial class UsuarioSeleccionable : INotifyPropertyChanged
    {
        public Usuario Usuario { get; set; }

        private bool _seleccionado;
        public bool Seleccionado
        {
            get => _seleccionado;
            set
            {
                _seleccionado = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
