// RelacionPage.xaml.cs
using Newtonsoft.Json;
using System.Text;
using System.Windows.Input;
using TuApp.Entidades.Entity;
using System.Collections.ObjectModel;
using TuApp.Entidades;
using MvvmHelpers;

namespace TuApp.ViewModels
{
    public class RelacionViewModel : BaseViewModel
    {
        public ObservableCollection<Usuario> Relaciones { get; set; } = new();

        public string CodigoPaciente { get; set; }

        public ICommand AgregarCommand { get; }
        public ICommand EliminarCommand { get; }

        public RelacionViewModel()
        {
            AgregarCommand = new Command(async () => await AgregarRelacion());
            EliminarCommand = new Command<Usuario>(async (usuario) => await EliminarRelacion(usuario));

            _ = Task.Run(async () => await CargarRelaciones());
        }

        private async Task CargarRelaciones()
        {
            try
            {
                ReqObtenerRelacion req = new ReqObtenerRelacion
                {
                    IdUsuario = SesionActiva.sesionActiva.usuario.IdUsuario
                };

                var json = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

                using HttpClient client = new();
                var resp = await client.PostAsync("https://localhost:44347/api/usuario/obtenerrelacion", json);

                if (resp.IsSuccessStatusCode)
                {
                    var content = await resp.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<ResObtenerRelacion>(content);

                    if (res.resultado)
                    {
                        Relaciones.Clear();
                        foreach (var item in res.listaUsuarios)
                            Relaciones.Add(item);
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "No se pudieron obtener las relaciones", "Aceptar");
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", $"Error HTTP: {resp.StatusCode}", "Aceptar");
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Excepción", $"Error al cargar relaciones: {ex.Message}", "Aceptar");
            }
        }


        private async Task AgregarRelacion()
        {
            if (string.IsNullOrWhiteSpace(CodigoPaciente))
            {
                await App.Current.MainPage.DisplayAlert("Advertencia", "Debes ingresar un código de paciente", "Aceptar");
                return;
            }

            ReqInsertarRelacion req = new ReqInsertarRelacion
            {
                IdUsuarioCuidador = SesionActiva.sesionActiva.usuario.IdUsuario,
                CodigoPaciente = CodigoPaciente.Trim()
            };

            var json = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
            using HttpClient client = new();
            var resp = await client.PostAsync("https://localhost:44347/api/usuario/insertarrelacion", json);

            if (resp.IsSuccessStatusCode)
            {
                var content = await resp.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<ResInsertarRelacion>(content);

                if (res.resultado)
                {
                    await App.Current.MainPage.DisplayAlert("Éxito", "Relación agregada correctamente", "Aceptar");
                    CodigoPaciente = string.Empty;
                    CargarRelaciones();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "No se pudo agregar la relación", "Aceptar");
                }
            }
        }

        private async Task EliminarRelacion(Usuario usuarioPaciente)
        {
            if (usuarioPaciente == null) return;

            bool confirmar = await App.Current.MainPage.DisplayAlert("Confirmar", $"Deseas eliminar la relación con {usuarioPaciente.Nombre}?", "Sí", "No");
            if (!confirmar) return;

            ReqEliminarRelacion req = new ReqEliminarRelacion
            {
                IdUsuarioCuidador = SesionActiva.sesionActiva.usuario.IdUsuario,
                IdUsuarioPaciente = usuarioPaciente.IdUsuario,
                CodigoPing = usuarioPaciente.pin?.Codigo
            };

            var json = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
            using HttpClient client = new();
            var resp = await client.PostAsync("https://localhost:44347/api/usuario/eliminarrelacion", json);

            if (resp.IsSuccessStatusCode)
            {
                var content = await resp.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<ResEliminarRelacion>(content);

                if (res.resultado)
                {
                    await App.Current.MainPage.DisplayAlert("Éxito", "Relación eliminada correctamente", "Aceptar");
                    CargarRelaciones();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "No se pudo eliminar la relación", "Aceptar");
                }
            }
        }
    }
}
