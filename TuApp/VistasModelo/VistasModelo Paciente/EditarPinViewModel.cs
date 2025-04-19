using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using TuApp.Entidades;
using TuApp.Entidades.Entity;
using TuApp.Entidades.Req.ReqUsuario;
using TuApp.Entidades.Res.ResUsuario;
using TuApp.Vistas;

namespace TuApp.ViewModels
{
    public class EditarPinPacienteViewModel : INotifyPropertyChanged
    {
        private string _pinActual;
        private string _nuevoPin;
        private bool _isLoading;
        private string _errorMessage;

        public ICommand RegresarInicioCommand { get; private set; }
        public ICommand ActualizarPinCommand { get; private set; }

        public string PinActual
        {
            get => _pinActual;
            set
            {
                _pinActual = value;
                OnPropertyChanged();
            }
        }

        public string NuevoPin
        {
            get => _nuevoPin;
            set
            {
                _nuevoPin = value;
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
        private readonly Page _page;

        public EditarPinPacienteViewModel(INavigation navigation, Page page)
        {
            _navigation = navigation;
            _page = page;

            RegresarInicioCommand = new Command(async () => await RegresarInicio());
            ActualizarPinCommand = new Command(async () => await ActualizarPin());
        }

        private async Task RegresarInicio()
        {
            await _navigation.PushAsync(new InicioPaciente());
        }

        private async Task ActualizarPin()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(PinActual) || string.IsNullOrWhiteSpace(NuevoPin))
                {
                    await _page.DisplayAlert("Error", "Debe ingresar su pin actual y el nuevo pin", "Aceptar");
                    return;
                }

                IsLoading = true;
                ErrorMessage = string.Empty;

                // Corrected request object to match API expectations
                var req = new
                {
                    IdUsuario = SesionActiva.sesionActiva.usuario.IdUsuario,
                    PinActual = PinActual.Trim(),
                    NuevoCodigo = NuevoPin.Trim()  // Changed from NuevoPin to NuevoCodigo
                };

                var jsonContent = new StringContent(
                    JsonConvert.SerializeObject(req),
                    Encoding.UTF8,
                    "application/json");

                HttpResponseMessage respuestaHttp;
                using (HttpClient httpClient = new HttpClient())
                {
                    // Corrected endpoint URL (assuming 'ping' was a typo)
                    respuestaHttp = await httpClient.PostAsync("https://localhost:44347/api/usuario/actualizarping", jsonContent);
                }

                if (respuestaHttp.IsSuccessStatusCode)
                {
                    var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<ResActualizarPinPaciente>(responseContent);

                    if (res != null && res.resultado)
                    {
                        SesionActiva.sesionActiva.usuario.pin.Codigo = NuevoPin;
                        await _page.DisplayAlert("Actualización correcta", "Pin actualizado correctamente", "Aceptar");
                        await _navigation.PushAsync(new InicioPaciente());
                    }
                    else
                    {
                        // Mostrar mensaje más detallado si está disponible
                        string errorDetail =  "Pin actual erróneo";
                        await _page.DisplayAlert("Error cambiando pin", errorDetail, "Aceptar");
                    }
                }
                else
                {
                    // Capturar más información del error HTTP
                    string errorContent = await respuestaHttp.Content.ReadAsStringAsync();
                    string errorMessage = $"Error de conexión: {respuestaHttp.StatusCode}";
                    if (!string.IsNullOrEmpty(errorContent))
                    {
                        errorMessage += $"\nDetalle: {errorContent}";
                    }
                    await _page.DisplayAlert("Error de conexión", errorMessage, "Aceptar");
                }
            }
            catch (Exception ex)
            {
                await _page.DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "Aceptar");
            }
            finally
            {
                IsLoading = false;
            }
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