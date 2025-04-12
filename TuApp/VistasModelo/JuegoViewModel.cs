using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TuApp.Entidades;
using TuApp.Entidades.Entity;
using TuApp.Entidades.Req.ReqJuego;

namespace TuApp.VistasModelo
{
    public class JuegoViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ResObtenerJuegosCuidador> ListaJuegos { get; set; } = new ObservableCollection<ResObtenerJuegosCuidador>();

        public ICommand CargarCommand { get; }
        public JuegoViewModel()
        {
            CargarCommand = new Command(async () => await CargarJuegos());
            _ = CargarJuegos(); // Carga al instanciarse
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
                List<ResObtenerJuegosCuidador> res = new List<ResObtenerJuegosCuidador>();
                res = JsonConvert.DeserializeObject<List<ResObtenerJuegosCuidador>>(contenido);

                if (res != null && res.First().resultado)
                {
                    ListaJuegos.Clear();
                    foreach (ResObtenerJuegosCuidador juego in res)
                    {
                        ResObtenerJuegosCuidador juegonuevo = new ResObtenerJuegosCuidador();
                        juegonuevo.IdJuego = juego.IdJuego;
                        juegonuevo.Nombre = juego.Nombre;
                        juegonuevo.numPreguntas = juego.numPreguntas;
                        ListaJuegos.Add(juegonuevo);
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
                    ListaJuegos.Clear();
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
                    ListaJuegos.Clear();
                    CargarJuegos();
                }

            }

        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}