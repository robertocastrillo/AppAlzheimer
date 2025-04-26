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
        // Definimos un origen de datos observable para la lista de eventos
        public ObservableCollection<Evento> ListaEventos { get; set; } = new ObservableCollection<Evento>();

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
        private string _apiUrl = string.Empty;
        private string ApiUrl  // Ya no es pública
        {
            get => _apiUrl;
            set { _apiUrl = value; OnPropertyChanged(); }
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
                DebugMessage = $"Usuario activo: ID {SesionActiva.sesionActiva.usuario.IdUsuario}";
            }
            else
            {
                Debug.WriteLine("WARNING: SesionActiva may be null or user ID is invalid");
                DebugMessage = "ERROR: Sesión de usuario no válida";
            }

            // Obtener la URL de la API desde la configuración de la aplicación
            try
            {
                ApiUrl = App.API_URL;
                Debug.WriteLine($"API URL from configuration: {ApiUrl}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting API URL: {ex.Message}");
                ApiUrl = App.API_URL + "/evento/obtenereventospaciente"; // Coloca aquí la URL correcta de tu API en la nube
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
                    IsLoading = false;
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

                // Configuración adicional para API en la nube
                client.Timeout = TimeSpan.FromSeconds(30); // Aumentar timeout para conexiones lentas

                // Asegurarnos de tener una URL válida
                string apiUrl = App.API_URL + "/evento/obtenereventospaciente";

                Debug.WriteLine($"Calling API: {apiUrl}");
                DebugMessage = $"Llamando a API: {apiUrl}";

                // Imprimir headers para debug
                client.DefaultRequestHeaders.Add("User-Agent", "TuApp-MAUI-Client");

                var resp = await client.PostAsync(apiUrl, content);

                Debug.WriteLine($"Response Status: {resp.StatusCode}");
                DebugMessage = $"Respuesta API: {resp.StatusCode}";

                if (resp.IsSuccessStatusCode)
                {
                    var jsonRes = await resp.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Response Content: {jsonRes}");

                    try
                    {
                        var res = JsonConvert.DeserializeObject<ResObtenerEventosPaciente>(jsonRes);

                        if (res != null)
                        {
                            DebugMessage = res.eventos != null
                                ? $"Se encontraron {res.eventos.Count} eventos"
                                : "La lista de eventos es nula";

                            if (res.eventos != null && res.eventos.Count > 0)
                            {
                                // Actualizar la UI en el hilo principal
                                await Microsoft.Maui.Controls.Application.Current.Dispatcher.DispatchAsync(() =>
                                {
                                    ListaEventos.Clear();
                                    foreach (var evento in res.eventos)
                                    {
                                        ListaEventos.Add(evento);
                                    }
                                    Debug.WriteLine($"Eventos agregados a ListaEventos: {ListaEventos.Count}");
                                    // Forzamos la notificación de cambio en la colección
                                    OnPropertyChanged(nameof(ListaEventos));
                                });
                            }
                            else
                            {
                                Debug.WriteLine("res.eventos is null or empty");
                                await Microsoft.Maui.Controls.Application.Current.Dispatcher.DispatchAsync(() =>
                                {
                                    ListaEventos.Clear();
                                    OnPropertyChanged(nameof(ListaEventos));
                                });
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

                        // Mostrar el JSON exacto para facilitar depuración
                        Debug.WriteLine($"JSON problemático: {await resp.Content.ReadAsStringAsync()}");
                    }
                }
                else
                {
                    var errorContent = await resp.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Error API: {resp.StatusCode}, Content: {errorContent}");
                    DebugMessage = $"Error de API: {resp.StatusCode} - {errorContent}";

                    await Microsoft.Maui.Controls.Application.Current.Dispatcher.DispatchAsync(async () =>
                    {
                        await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error",
                            $"Error al obtener eventos: {resp.StatusCode}", "Aceptar");
                    });
                }
            }
            catch (HttpRequestException hex)
            {
                Debug.WriteLine($"HTTP Request Error: {hex.Message}");
                DebugMessage = $"Error de conexión: {hex.Message}";

                await Microsoft.Maui.Controls.Application.Current.Dispatcher.DispatchAsync(async () =>
                {
                    await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error de Conexión",
                        "No se pudo conectar con el servidor. Verifica tu conexión a internet y la URL de la API.", "Aceptar");
                });
            }
            catch (TaskCanceledException tex)
            {
                Debug.WriteLine($"Request timed out: {tex.Message}");
                DebugMessage = "Tiempo de espera agotado en la petición";

                await Microsoft.Maui.Controls.Application.Current.Dispatcher.DispatchAsync(async () =>
                {
                    await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error",
                        "La solicitud tardó demasiado tiempo. Comprueba tu conexión.", "Aceptar");
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.GetType().Name} - {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
                DebugMessage = $"Error: {ex.Message}";

                await Microsoft.Maui.Controls.Application.Current.Dispatcher.DispatchAsync(async () =>
                {
                    await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Aceptar");
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