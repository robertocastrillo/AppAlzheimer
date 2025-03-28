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

namespace TuApp.VistasModelo
{
    public class JuegoViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<Juego> ListaJuegos { get; set; } = new ObservableCollection<Juego>();

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
                    foreach (var juego in res)
                    {
                        Juego juegonuevo = new Juego();
                        juegonuevo.IdJuego = juego.IdJuego;
                        juegonuevo.Nombre = juego.Nombre;
                        juegonuevo.numPreg = juego.numPreg;
                        ListaJuegos.Add(juegonuevo);
                    }
                }

            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
