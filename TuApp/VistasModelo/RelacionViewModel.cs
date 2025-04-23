using MvvmHelpers;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using TuApp.Entidades;
using TuApp.Entidades.Entity;

namespace TuApp.ViewModels;

public class RelacionViewModel : BaseViewModel
{
    public ObservableCollection<Usuario> Relaciones { get; set; } = new();

    private string _codigoPaciente;
    public string CodigoPaciente
    {
        get => _codigoPaciente;
        set
        {
            _codigoPaciente = value;
            OnPropertyChanged();
        }
    }

    public ICommand AgregarCommand { get; }
    public ICommand EliminarCommand { get; }

    public RelacionViewModel()
    {
        AgregarCommand = new Command(async () => await AgregarRelacion());
        EliminarCommand = new Command<Usuario>(async (usuario) => await EliminarRelacion(usuario));
        _ = CargarRelaciones();
    }

    private async Task CargarRelaciones()
    {
        try
        {
            var req = new ReqObtenerRelacion
            {
                IdUsuario = SesionActiva.sesionActiva.usuario.IdUsuario
            };

            var json = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
            using HttpClient client = new();
            var resp = await client.PostAsync(App.API_URL + "usuario/obtenerrelacion", json);

            if (resp.IsSuccessStatusCode)
            {
                var content = await resp.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<ResObtenerRelacion>(content);

                if (res.resultado && res.listaUsuarios != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Relaciones.Clear();
                        foreach (var item in res.listaUsuarios)
                            Relaciones.Add(item);
                    });
                }else if (res.resultado && !res.listaUsuarios.Any())
                {
                    await App.Current.MainPage.DisplayAlert("Error", "No hay relaciones definidas", "Aceptar");
                }
                else
                {
                    //Relaciones.Clear();
                    await App.Current.MainPage.DisplayAlert("Error", "No se pudieron obtener las relaciones", "Aceptar");
                }
            }
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Aceptar");
        }
    }

    private async Task AgregarRelacion()
    {
        if (string.IsNullOrWhiteSpace(CodigoPaciente))
        {
            await App.Current.MainPage.DisplayAlert("Advertencia", "Debes ingresar un código de paciente", "Aceptar");
            return;
        }

        var req = new ReqInsertarRelacion
        {
            IdUsuarioCuidador = SesionActiva.sesionActiva.usuario.IdUsuario,
            CodigoPaciente = CodigoPaciente.Trim()
        };

        var json = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
        using HttpClient client = new();
        var resp = await client.PostAsync(App.API_URL + "usuario/insertarrelacion", json);

        if (resp.IsSuccessStatusCode)
        {
            var content = await resp.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<ResInsertarRelacion>(content);

            if (res.resultado)
            {
                await App.Current.MainPage.DisplayAlert("Éxito", "Relación agregada correctamente", "Aceptar");
                CodigoPaciente = string.Empty;
                await CargarRelaciones();
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

        bool confirmar = await App.Current.MainPage.DisplayAlert("Confirmar", $"¿Deseas eliminar la relación con {usuarioPaciente.Nombre}?", "Sí", "No");
        if (!confirmar) return;

        var req = new ReqEliminarRelacion
        {
            IdUsuarioCuidador = SesionActiva.sesionActiva.usuario.IdUsuario,
            IdUsuarioPaciente = usuarioPaciente.IdUsuario,
            CodigoPing = usuarioPaciente.pin?.Codigo
        };

        var json = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
        using HttpClient client = new();
        var resp = await client.PostAsync(App.API_URL + "usuario/eliminarrelacion", json);

        if (resp.IsSuccessStatusCode)
        {
            var content = await resp.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<ResEliminarRelacion>(content);

            if (res.resultado)
            {
                await App.Current.MainPage.DisplayAlert("Éxito", "Relación eliminada correctamente", "Aceptar");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Relaciones.Remove(usuarioPaciente);
                });
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo eliminar la relación", "Aceptar");
            }
        }
    }
}
