using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using TuApp.Entidades;
using TuApp.Entidades.Entity;
using TuApp.Entidades.Res.ResMensaje;
using TuApp.Vistas;

namespace TuApp.ViewModels
{
    public class ChatsPacienteViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Mensaje> _mensajes;
        private bool _isLoading;
        private string _errorMessage;

        public ICommand RegresarInicioCommand { get; private set; }

        public ObservableCollection<Mensaje> Mensajes
        {
            get => _mensajes;
            set
            {
                _mensajes = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        private readonly INavigation _navigation;

        public ChatsPacienteViewModel(INavigation navigation)
        {
            _navigation = navigation;
            Mensajes = new ObservableCollection<Mensaje>();

            RegresarInicioCommand = new Command(async () => await RegresarInicio());

            // Load chats when the ViewModel is created
            Task.Run(async () => await CargarPreguntas());
        }

        public async Task CargarPreguntas()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                // Use proper request object similar to MensajeViewModel
                var req = new ReqObtenerMensajes
                {
                    IdPaciente = SesionActiva.sesionActiva.usuario.IdUsuario
                };

                var jsonContent = new StringContent(
                    JsonConvert.SerializeObject(req),
                    Encoding.UTF8,
                    "application/json");

                HttpResponseMessage respuestaHttp;
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(App.API_URL);
                    respuestaHttp = await httpClient.PostAsync("mensaje/obtener", jsonContent);
                }

                if (respuestaHttp.IsSuccessStatusCode)
                {
                    var contenido = await respuestaHttp.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<ResObtenerMensajes>(contenido);

                    if (res != null && res.resultado)
                    {
                        // Use dispatcher to update UI on main thread
                        await Application.Current.MainPage.Dispatcher.DispatchAsync(() =>
                        {
                            Mensajes.Clear();
                            foreach (var mensaje in res.listaMensajes)
                            {
                                Mensajes.Add(mensaje);
                            }
                        });
                    }
                    else
                    {
                        ErrorMessage = "No se encontraron mensajes o ocurrió un error.";
                    }
                }
                else
                {
                    ErrorMessage = $"Error al cargar los mensajes: {respuestaHttp.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task RegresarInicio()
        {
            await _navigation.PushAsync(new InicioPaciente());
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}