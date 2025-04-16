using CommunityToolkit.Maui.Alerts;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using TuApp.Entidade;
using TuApp.Entidades;
using TuApp.Entidades.Entity;
using TuApp.Vistas;


namespace TuApp.VistasModelo
{
    public class EventoViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Evento> ListaEventos { get; set; } = new();
        public ObservableCollection<Usuario> PacientesRelacionados { get; set; } = new();

        private Evento _eventoSeleccionado;
        public Evento EventoSeleccionado
        {
            get => _eventoSeleccionado;
            set { _eventoSeleccionado = value; OnPropertyChanged(); }
        }

        private Usuario _pacienteSeleccionado;
        public Usuario PacienteSeleccionado
        {
            get => _pacienteSeleccionado;
            set { _pacienteSeleccionado = value; OnPropertyChanged(); }
        }

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

        public int IdEvento { get; set; }
        public int IdPrioridad { get; set; }

        public string FechaHoraFormateada => (Fecha.Date + Hora).ToString("dd/MM/yyyy HH:mm");

        public string DescripcionPrioridad => PrioridadSeleccionada?.Descripcion ?? ListaPrioridades.FirstOrDefault(p => p.IdPrioridad == IdPrioridad)?.Descripcion ?? $"Prioridad ID: {IdPrioridad}";

        public ICommand CargarEventosCommand { get; }
        public ICommand InsertarEventoCommand { get; }
        public ICommand EliminarEventoCommand { get; set; }
        public ICommand ActualizarEventoCommand { get; set; }
        public ICommand AsignarEventoCommand { get; set; }

        public EventoViewModel()
        {
            CargarEventosCommand = new Command(async () => await CargarEventosAsync());
            InsertarEventoCommand = new Command(async () => await InsertarEventoAsync());
            EliminarEventoCommand = new Command(async () => await EliminarEventoAsync());
        }

        public EventoViewModel(Evento evento)
        {
            Titulo = evento.Titulo;
            Descripcion = evento.Descripcion;
            Fecha = evento.FechaHora.Date;
            Hora = evento.FechaHora.TimeOfDay;
            IdEvento = evento.IdEvento;
            IdPrioridad = evento.IdPrioridad;
            PrioridadSeleccionada = ListaPrioridades.FirstOrDefault(p => p.IdPrioridad == evento.IdPrioridad);

            EliminarEventoCommand = new Command(async () => await EliminarEventoDesdeDetalleAsync());
            ActualizarEventoCommand = new Command(async () => await ActualizarEventoAsync());
            AsignarEventoCommand = new Command(async () => await AsignarEventoAPaciente());

            _ = CargarPacientesRelacionados();
        }

        private async Task CargarEventosAsync()
        {
            ListaEventos.Clear();
            try
            {
                var req = new ReqObtenerEventosCuidador { IdCuidador = SesionActiva.sesionActiva.usuario.IdUsuario };
                var json = JsonConvert.SerializeObject(req);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using HttpClient client = new();
                var resp = await client.PostAsync("https://localhost:44347/api/evento/obtenereventoscuidador", content);

                if (resp.IsSuccessStatusCode)
                {
                    var jsonRes = await resp.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<List<ResObtenerEventosCuidador>>(jsonRes);

                    if (res != null)
                        foreach (var item in res)
                            ListaEventos.Add(item.eventos);
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Aceptar");
            }
        }

        private async Task InsertarEventoAsync()
        {
            try
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

                var json = JsonConvert.SerializeObject(req);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

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
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Aceptar");
            }
        }

        private async Task EliminarEventoAsync() { /* ... Igual al anterior ... */ }
        private async Task EliminarEventoDesdeDetalleAsync() { /* ... Igual al anterior ... */ }
        private async Task ActualizarEventoAsync() { /* ... Igual al anterior ... */ }

        private async Task CargarPacientesRelacionados()
        {
            try
            {
                var req = new ReqObtenerRelacion { IdUsuario = SesionActiva.sesionActiva.usuario.IdUsuario };
                var json = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
                using HttpClient client = new();
                var resp = await client.PostAsync("https://localhost:44347/api/usuario/obtenerrelacion", json);

                if (resp.IsSuccessStatusCode)
                {
                    var content = await resp.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<ResObtenerRelacion>(content);

                    if (res.resultado && res.listaUsuarios != null)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            PacientesRelacionados.Clear();
                            foreach (var p in res.listaUsuarios)
                                PacientesRelacionados.Add(p);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Aceptar");
            }
        }

        private async Task AsignarEventoAPaciente()
        {
            if (PacienteSeleccionado == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Seleccione un paciente", "Aceptar");
                return;
            }

            var req = new ReqInsertarPacienteEvento
            {
                IdEvento = IdEvento,
                IdCuidador = SesionActiva.sesionActiva.usuario.IdUsuario,
                IdPaciente = PacienteSeleccionado.IdUsuario
            };

            var json = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
            using HttpClient client = new();
            var resp = await client.PostAsync("https://localhost:44347/api/evento/agregarPaciente", json);

            if (resp.IsSuccessStatusCode)
            {
                var content = await resp.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<ResInsertarPacienteEvento>(content);

                if (res.resultado)
                {
                    await App.Current.MainPage.DisplayAlert("Éxito", "Evento asignado al paciente", "Aceptar");
                    PacienteSeleccionado = null;
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "No se pudo asignar el evento", "Aceptar");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string nombre = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
    }
}