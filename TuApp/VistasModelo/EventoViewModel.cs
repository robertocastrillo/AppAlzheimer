using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using TuApp.Entidade;
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
                var resp = await client.PostAsync(App.API_URL + "evento/obtenereventoscuidador", content);

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
            if (string.IsNullOrWhiteSpace(Titulo))
            {
                await App.Current.MainPage.DisplayAlert("Error", "El título del evento es obligatorio", "Aceptar");
                return;
            }

            if (Fecha == default(DateTime))
            {
                await App.Current.MainPage.DisplayAlert("Error", "La fecha del evento es obligatoria", "Aceptar");
                return;
            }

            if (PrioridadSeleccionada == null || PrioridadSeleccionada.IdPrioridad <= 0)
            {
                await App.Current.MainPage.DisplayAlert("Error", "La prioridad debe ser válida", "Aceptar");
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
            var response = await client.PostAsync(App.API_URL + "evento/insertar", content);

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

            bool confirm = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Deseas eliminar este evento?", "Sí", "No");
            if (!confirm) return;

            var req = new ReqEliminarEvento {
                IdEvento = IdEvento,
                IdCuidador = SesionActiva.sesionActiva.usuario.IdUsuario};
            var json = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");



            HttpResponseMessage respuestaHttp = null;

            using (HttpClient httpClient = new HttpClient())
            {

                respuestaHttp = await httpClient.PostAsync(App.API_URL + "evento/eliminar", json);

            }

            if (respuestaHttp.IsSuccessStatusCode)
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
            // Sin cambios
        }

        public async Task GuardarAsignaciones()
        {
            // Sin cambios
        }

        private async Task ActualizarEventoAsync()
        {
            if (string.IsNullOrWhiteSpace(Titulo))
            {
                await App.Current.MainPage.DisplayAlert("Error", "El título del evento es obligatorio", "Aceptar");
                return;
            }

            if (Fecha.Date + Hora < DateTime.Now)
            {
                await App.Current.MainPage.DisplayAlert("Error", "La fecha del evento debe ser futura", "Aceptar");
                return;
            }

            if (PrioridadSeleccionada == null || PrioridadSeleccionada.IdPrioridad <= 0)
            {
                await App.Current.MainPage.DisplayAlert("Error", "ID de prioridad inválido", "Aceptar");
                return;
            }

            var req = new ReqActualizarEvento
            {
                IdEvento = IdEvento,
                IdCuidador = SesionActiva.sesionActiva.usuario.IdUsuario,
                Titulo = Titulo,
                Descripcion = Descripcion,
                FechaHora = Fecha.Date + Hora,
                IdPrioridad = PrioridadSeleccionada.IdPrioridad
            };

            var json = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
            using HttpClient client = new();
            var resp = await client.PostAsync(App.API_URL + "evento/actualizar", json);

            if (resp.IsSuccessStatusCode)
            {
                var content = await resp.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<ResActualizarEvento>(content);

                if (res.resultado)
                {
                    await App.Current.MainPage.DisplayAlert("Éxito", "Evento actualizado correctamente", "Aceptar");
                    Application.Current.MainPage = new MenuCuidadorPage(new EventosPage());
                }
                else
                {
                    var mensaje = res.listaDeErrores.FirstOrDefault()?.error ?? "No se pudo actualizar";
                    await App.Current.MainPage.DisplayAlert("Error", mensaje, "Aceptar");
                }
            }
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
