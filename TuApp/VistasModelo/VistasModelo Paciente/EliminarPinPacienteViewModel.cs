using System;
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
using TuApp.Entidades.Req.ReqUsuario;
using TuApp.Entidades.Res.ResUsuario;
using TuApp.Vistas;

namespace TuApp.ViewModels
{
    public class EliminarPinPacienteViewModel : INotifyPropertyChanged
    {
        private string _pin;
        private bool _isLoading;
        private string _errorMessage;

        public ICommand RegresarInicioCommand { get; private set; }
        public ICommand EliminarPinCommand { get; private set; }

        public string Pin
        {
            get => _pin;
            set
            {
                _pin = value;
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

        public EliminarPinPacienteViewModel(INavigation navigation, Page page)
        {
            _navigation = navigation;
            _page = page;

            RegresarInicioCommand = new Command(async () => await RegresarInicio());
            EliminarPinCommand = new Command(async () => await EliminarPin());
        }

        private async Task RegresarInicio()
        {
            await _navigation.PushAsync(new InicioPaciente());
        }

        private async Task EliminarPin()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Pin))
                {
                    await _page.DisplayAlert("Error", "Debe ingresar el pin a eliminar", "Aceptar");
                    return;
                }

                IsLoading = true;
                ErrorMessage = string.Empty;

                ReqEliminarPinPaciente req = new ReqEliminarPinPaciente
                {
                    Codigo = Pin,
                    IdUsuario = SesionActiva.sesionActiva.usuario.IdUsuario
                };

                var jsonContent = new StringContent(
                    JsonConvert.SerializeObject(req),
                    Encoding.UTF8,
                    "application/json");

                HttpResponseMessage respuestaHttp;
                using (HttpClient httpClient = new HttpClient())
                {
                    respuestaHttp = await httpClient.PostAsync("https://localhost:44347/api/usuario/eliminarping", jsonContent);
                }

                if (respuestaHttp.IsSuccessStatusCode)
                {
                    var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<ResEliminarPinPaciente>(responseContent);

                    if (res != null && res.resultado)
                    {
                        SesionActiva.sesionActiva.usuario.pin.Codigo = null;
                        await _page.DisplayAlert("Eliminación correcta", "Pin eliminado correctamente", "Aceptar");
                        await _navigation.PushAsync(new InicioPaciente());
                    }
                    else
                    {
                        await _page.DisplayAlert("Error eliminando pin", "Pin erróneo", "Aceptar");
                    }
                }
                else
                {
                    await _page.DisplayAlert("Error de conexión", "No hay respuesta del servidor", "Aceptar");
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