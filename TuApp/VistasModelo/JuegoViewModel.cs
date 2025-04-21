using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using TuApp.Entidades;
using TuApp.Entidades.Entity;
using TuApp.Entidades.Req.ReqJuego;


namespace TuApp.VistasModelo
{
    public class JuegoViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<JuegoCuidador> ListaJuegosCuidador { get; set; } = new ObservableCollection<JuegoCuidador>();
        public ObservableCollection<JuegoPaciente> ListaJuegosPaciente { get; set; } = new ObservableCollection<JuegoPaciente>();
        public ICommand CargarCommand { get; }
        public JuegoViewModel()
        {
            if(SesionActiva.sesionActiva.usuario.IdTipoUsuario == 1)
            {
                CargarCommand = new Command(async () => await CargarJuegosPaciente());
                _ = CargarJuegosPaciente(); // Carga al instanciarse
            } else if (SesionActiva.sesionActiva.usuario.IdTipoUsuario == 1)
            {
                CargarCommand = new Command(async () => await CargarJuegos());
                _ = CargarJuegos(); // Carga al instanciarse
            }

        }
        private async Task CargarJuegos()
        {
            ReqObtenerJuegosCuidador req = new ReqObtenerJuegosCuidador
            {
                idCuidador = SesionActiva.sesionActiva.usuario.IdUsuario
            };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            HttpResponseMessage respuestaHttp = null;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://localhost:44347/api/");
                respuestaHttp = await httpClient.PostAsync("juego/obtenerjuegoscuidador", jsonContent);
            }
            if (respuestaHttp.IsSuccessStatusCode)
            {
                var contenido = await respuestaHttp.Content.ReadAsStringAsync();
                ResObtenerJuegosCuidador res = new ResObtenerJuegosCuidador();
                res = JsonConvert.DeserializeObject<ResObtenerJuegosCuidador>(contenido);

                if (res != null && res.resultado)
                {
                    ListaJuegosCuidador.Clear();
                    foreach (JuegoCuidador juego in res.juegosCuidadorList)
                    {
                        JuegoCuidador juegoCuidador = new JuegoCuidador();

                        List<PacienteAsignado> pacientes = new List<PacienteAsignado>();

                        foreach (PacienteAsignado pac in juego.pacientes)
                        {
                            PacienteAsignado paciente = new PacienteAsignado();
                            paciente.id_Usuario = pac.id_Usuario;
                            paciente.nombre = pac.nombre;
                            pacientes.Add(paciente);
                        }
                        juegoCuidador.idJuego = juego.idJuego;
                        juegoCuidador.nombre = juego.nombre;
                        juegoCuidador.numPreguntas = juego.numPreguntas;
                        juegoCuidador.pacientes = new ObservableCollection<PacienteAsignado>(pacientes);
                        ListaJuegosCuidador.Add(juegoCuidador);
                    }
                }

            }

        }

        public async Task InsertarJuego(string nombreJuego)
        {
            ReqInsertarJuego req = new ReqInsertarJuego
            {
                idUsuario = SesionActiva.sesionActiva.usuario.IdUsuario,
                nombre = nombreJuego

            };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            HttpResponseMessage respuestaHttp = null;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://localhost:44347/api/");
                respuestaHttp = await httpClient.PostAsync("juego/insertarjuego", jsonContent);
            }
            if (respuestaHttp.IsSuccessStatusCode)
            {
                var contenido = await respuestaHttp.Content.ReadAsStringAsync();
                ResInsertarJuego res = new ResInsertarJuego();
                res = JsonConvert.DeserializeObject<ResInsertarJuego>(contenido);

                if (res != null && res.resultado)
                {
                    ListaJuegosCuidador.Clear();
                    CargarJuegos();
                }

            }

        }

        public async Task EliminarJuego(int idJuego)
        {
            ReqEliminarJuego req = new ReqEliminarJuego
            {
                idCuidador = SesionActiva.sesionActiva.usuario.IdUsuario,
                idJuego = idJuego

            };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            HttpResponseMessage respuestaHttp = null;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://localhost:44347/api/");
                respuestaHttp = await httpClient.PostAsync("juego/eliminarjuego", jsonContent);
            }
            if (respuestaHttp.IsSuccessStatusCode)
            {
                var contenido = await respuestaHttp.Content.ReadAsStringAsync();
                ResEliminarJuego res = new ResEliminarJuego();
                res = JsonConvert.DeserializeObject<ResEliminarJuego>(contenido);

                if (res != null && res.resultado)
                {
                    ListaJuegosCuidador.Clear();
                    CargarJuegos();
                }

            }

        }
        public async Task InsertarRelacionJuego(int idJuego, int idPaciente)
        {
            try
            {
                var req = new ReqInsertarRelacionJuego
                {
                    idJuego = idJuego,
                    idUsuario = SesionActiva.sesionActiva.usuario.IdUsuario,
                    idPaciente = idPaciente
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri("https://localhost:44347/api/");
                    var respuestaHttp = await httpClient.PostAsync("juego/insertarrelacion", jsonContent);

                    if (respuestaHttp.IsSuccessStatusCode)
                    {
                        var contenido = await respuestaHttp.Content.ReadAsStringAsync();
                        var resultado = JsonConvert.DeserializeObject<ResBase>(contenido);

                        if (resultado != null && resultado.resultado)
                        {
                            await App.Current.MainPage.DisplayAlert("Éxito", "Paciente asignado al juego correctamente", "Aceptar");
                            await CargarJuegos(); // Recarga lista con nuevos datos
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Error", "No se pudo asignar el paciente al juego", "Aceptar");
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Error en la conexión con el servidor", "Aceptar");
                    }
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Excepción", ex.Message, "OK");
            }
        }

        private async Task CargarJuegosPaciente()
        {
            ReqObtenerJuegosPaciente req = new ReqObtenerJuegosPaciente
            {
                idPaciente = SesionActiva.sesionActiva.usuario.IdUsuario
            };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            HttpResponseMessage respuestaHttp = null;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://localhost:44347/api/");
                respuestaHttp = await httpClient.PostAsync("juego/obtenerjuegospaciente", jsonContent);
            }
            if (respuestaHttp.IsSuccessStatusCode)
            {
                var contenido = await respuestaHttp.Content.ReadAsStringAsync();
                List<ResObtenerJuegosPaciente> res = new List<ResObtenerJuegosPaciente>();
                res = JsonConvert.DeserializeObject<List<ResObtenerJuegosPaciente>>(contenido);

                if (res != null && res.First().resultado)
                {
                    ListaJuegosCuidador.Clear();
                    foreach (ResObtenerJuegosPaciente juego in res)
                    {
                        JuegoPaciente juegoPac = new JuegoPaciente();
                        juegoPac.idJuego = juego.idJuego;
                        juegoPac.nombre = juego.nombre;
                        juegoPac.numPreguntas = juego.numPreguntas;
                        ListaJuegosPaciente.Add(juegoPac);
                    }
                }

            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}