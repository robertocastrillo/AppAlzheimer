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
    public class PreguntaViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Pregunta> ListaPregunta { get; set; } = new ObservableCollection<Pregunta>();


        public PreguntaViewModel()
        {
        }


        public async Task CargarPreguntas(ResObtenerJuegosCuidador juegoselec)
        {
            ReqObtenerPreguntas req = new ReqObtenerPreguntas
            {
                idJuego = juegoselec.IdJuego
            };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            HttpResponseMessage respuestaHttp = null;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://localhost:44347/api/");
                respuestaHttp = await httpClient.PostAsync("juego/obtenerpreguntas", jsonContent);
            }
            if (respuestaHttp.IsSuccessStatusCode)
            {
                var contenido = await respuestaHttp.Content.ReadAsStringAsync();
                ResObtenerPreguntas res = new ResObtenerPreguntas();
                res = JsonConvert.DeserializeObject<ResObtenerPreguntas>(contenido);

                if (res != null && res.juego.preguntas.Any())
                {
                    ListaPregunta.Clear();
                    foreach (Pregunta pregunta in res.juego.preguntas)
                    {
                        Pregunta preg = new Pregunta();
                        preg.IdPregunta = pregunta.IdPregunta;
                        preg.Descripcion = pregunta.Descripcion;
                        preg.Imagen = pregunta.Imagen;
                        foreach (Opcion opcion in pregunta.opciones)
                        {
                            Opcion opc = new Opcion();
                            opc.Id_Opcion = opcion.Id_Opcion;
                            opc.Descripcion = opcion.Descripcion;
                            opc.Condicion = opcion.Condicion;
                            preg.opciones.Add(opcion);
                        }
                        ListaPregunta.Add(preg);
                    }
                }

            }

        }



        public event PropertyChangedEventHandler PropertyChanged;
    }
}