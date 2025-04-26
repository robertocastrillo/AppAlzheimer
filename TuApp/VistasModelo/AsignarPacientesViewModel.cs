// AsignarPacientesViewModel.cs
using MvvmHelpers;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using TuApp.Entidades;
using TuApp.Entidades.Entity;

namespace TuApp.VistasModelo
{
    public class AsignarPacientesViewModel : BaseViewModel
    {
        private readonly EventoViewModel _eventoViewModel;

        public ObservableCollection<UsuarioSeleccionable> PacientesRelacionados { get; set; } = new();
        public ICommand GuardarAsignacionesCommand { get; }

        public AsignarPacientesViewModel(EventoViewModel eventoViewModel)
        {
            _eventoViewModel = eventoViewModel;
            GuardarAsignacionesCommand = new Command(async () => await GuardarAsignacionesAsync());
            _ = CargarPacientesRelacionados();
        }

        private async Task CargarPacientesRelacionados()
        {
            try
            {
                var req = new ReqObtenerRelacion { IdUsuario = SesionActiva.sesionActiva.usuario.IdUsuario };
                var json = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

                using HttpClient client = new();
                var resp = await client.PostAsync(App.API_URL + "usuario/obtenerrelacion", json);

                if (resp.IsSuccessStatusCode)
                {
                    var content = await resp.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<ResObtenerRelacion>(content);

                    if (res != null && res.resultado)
                    {
                        PacientesRelacionados.Clear();
                        foreach (var usuario in res.listaUsuarios)
                        {
                            PacientesRelacionados.Add(new UsuarioSeleccionable
                            {
                                Usuario = usuario,
                                Seleccionado = false,
                                PuedeAsignar = true
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Aceptar");
            }
        }

        private async Task GuardarAsignacionesAsync()
        {
            try
            {
                foreach (var paciente in PacientesRelacionados.Where(p => p.Seleccionado && p.PuedeAsignar))
                {
                    var req = new ReqInsertarPacienteEvento
                    {
                        IdEvento = _eventoViewModel.IdEvento,
                        IdCuidador = SesionActiva.sesionActiva.usuario.IdUsuario,
                        IdPaciente = paciente.Usuario.IdUsuario
                    };

                    var json = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
                    using HttpClient client = new();
                    var resp = await client.PostAsync(App.API_URL + "evento/insertarpacienteevento", json);

                    if (!resp.IsSuccessStatusCode)
                    {
                        await App.Current.MainPage.DisplayAlert("Error", $"No se pudo asignar a {paciente.Usuario.Nombre}", "Aceptar");
                    }
                }

                await App.Current.MainPage.DisplayAlert("Éxito", "Pacientes asignados correctamente", "Aceptar");
                await Shell.Current.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Aceptar");
            }
        }
    }


}
