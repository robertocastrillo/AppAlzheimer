using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using TuApp.Entidades;
using TuApp.Entidades.Entity;

namespace TuApp.VistasModelo
{
    public class EventoPacienteViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Evento> ListaEventos { get; set; } = new();

        private Evento _eventoSeleccionado;
        public Evento EventoSeleccionado
        {
            get => _eventoSeleccionado;
            set
            {
                _eventoSeleccionado = value;
                OnPropertyChanged();
                if (value != null)
                {
                    // Navegación al detalle del evento si es necesario
                    // Application.Current.MainPage.Navigation.PushAsync(new DetalleEventoPage(value));
                    EventoSeleccionado = null; // Reset selección
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        private string _debugMessage = "No messages yet";
        public string DebugMessage
        {
            get => _debugMessage;
            set { _debugMessage = value; OnPropertyChanged(); }
        }

        public ICommand CargarEventosCommand { get; }
        public ICommand ForceRefreshCommand { get; }

        public EventoPacienteViewModel()
        {
            Debug.WriteLine("EventoPacienteViewModel constructor called");

            // Check if session is active
            if (SesionActiva.sesionActiva?.usuario?.IdUsuario > 0)
            {
                Debug.WriteLine($"Usuario ID: {SesionActiva.sesionActiva.usuario.IdUsuario}");
            }
            else
            {
                Debug.WriteLine("WARNING: SesionActiva may be null or user ID is invalid");
            }

            CargarEventosCommand = new Command(async () => await CargarEventosPacienteAsync());
            ForceRefreshCommand = new Command(async () =>
            {
                Debug.WriteLine("Force refresh triggered");
                await CargarEventosPacienteAsync();
            });

            // No autoload in constructor, let OnAppearing handle it
        }

        public async Task CargarEventosPacienteAsync()
        {
            Debug.WriteLine("CargarEventosPacienteAsync called");
            IsLoading = true;
            DebugMessage = "Cargando eventos...";

            try
            {
                if (SesionActiva.sesionActiva?.usuario == null)
                {
                    DebugMessage = "ERROR: Sesión de usuario no está activa";
                    Debug.WriteLine("ERROR: SesionActiva.sesionActiva.usuario is null");
                    return;
                }

                var req = new ReqObtenerEventosPaciente
                {
                    IdPaciente = SesionActiva.sesionActiva.usuario.IdUsuario
                };

                var json = JsonConvert.SerializeObject(req);
                Debug.WriteLine($"Request: {json}");
                DebugMessage = $"Enviando petición para ID: {req.IdPaciente}";

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using HttpClient client = new();

                // Set timeout to avoid hanging
                client.Timeout = TimeSpan.FromSeconds(15);

                var apiUrl = "https://localhost:44347/api/evento/obtenereventospaciente";
                Debug.WriteLine($"Calling API: {apiUrl}");

                var resp = await client.PostAsync(apiUrl, content);

                Debug.WriteLine($"Response Status: {resp.StatusCode}");
                DebugMessage = $"Respuesta API: {resp.StatusCode}";

                if (resp.IsSuccessStatusCode)
                {
                    var jsonRes = await resp.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Response: {jsonRes}");

                    try
                    {
                        var res = JsonConvert.DeserializeObject<ResObtenerEventosPaciente>(jsonRes);

                        if (res != null)
                        {
                            DebugMessage = res.eventos != null
                                ? $"Se encontraron {res.eventos.Count} eventos"
                                : "La lista de eventos es nula";

                            if (res.eventos != null)
                            {
                                await MainThread.InvokeOnMainThreadAsync(() =>
                                {
                                    ListaEventos.Clear();
                                    foreach (var evento in res.eventos)
                                    {
                                        ListaEventos.Add(evento);
                                    }

                                    Debug.WriteLine($"Eventos agregados a ListaEventos: {ListaEventos.Count}");
                                });
                            }
                            else
                            {
                                Debug.WriteLine("res.eventos is null");
                            }
                        }
                        else
                        {
                            Debug.WriteLine("res is null after deserialization");
                            DebugMessage = "Error al procesar la respuesta del servidor";
                        }
                    }
                    catch (JsonException jex)
                    {
                        Debug.WriteLine($"JSON Parsing Error: {jex.Message}");
                        DebugMessage = $"Error al parsear JSON: {jex.Message}";
                    }
                }
                else
                {
                    var errorContent = await resp.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Error API: {resp.StatusCode}, Content: {errorContent}");
                    DebugMessage = $"Error de API: {resp.StatusCode}";

                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await App.Current.MainPage.DisplayAlert("Error",
                            $"Error al obtener eventos: {resp.StatusCode}", "Aceptar");
                    });
                }
            }
            catch (TaskCanceledException tex)
            {
                Debug.WriteLine($"Request timed out: {tex.Message}");
                DebugMessage = "Tiempo de espera agotado en la petición";

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await App.Current.MainPage.DisplayAlert("Error",
                        "La solicitud tardó demasiado tiempo. Comprueba tu conexión.", "Aceptar");
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.GetType().Name} - {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
                DebugMessage = $"Error: {ex.Message}";

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Aceptar");
                });
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string nombre = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
    }


 
}