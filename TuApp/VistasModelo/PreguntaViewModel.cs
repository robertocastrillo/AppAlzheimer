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

        public Pregunta PreguntaNueva {get; set;} = new Pregunta();
        public PreguntaViewModel()
        {
        }


        public async Task CargarPreguntas(int idJuego)
        {
            ReqObtenerPreguntas req = new ReqObtenerPreguntas
            {
                idJuego = idJuego
            };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            HttpResponseMessage respuestaHttp = null;

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "juego/obtenerpreguntas", jsonContent);
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

        public async Task InsertarPregunta(Pregunta pregunta, int idJuego, int idUsuario)
        {
            try
            {
                // Preparamos la pregunta (convertimos imagen a base64)
                var preguntaJson = new
                {
                    Descripcion = pregunta.Descripcion,
                    Imagen = Convert.ToBase64String(pregunta.Imagen),
                    opciones = pregunta.opciones.Select(o => new
                    {
                        Descripcion = o.Descripcion,
                        Condicion = o.Condicion
                    }).ToList()
                };

                var req = new
                {
                    idUsuario = idUsuario,
                    idJuego = idJuego,
                    preguntas = new[] { preguntaJson }
                };

                var json = JsonConvert.SerializeObject(req);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage respuestaHttp;

                using (HttpClient httpClient = new HttpClient())
                {
                    respuestaHttp = await httpClient.PostAsync(App.API_URL + "juego/insertarpregunta", content);
                }

                if (respuestaHttp.IsSuccessStatusCode)
                {
                    // Podés asumir que fue insertada, o podrías leer la respuesta si retorna ID
                    ListaPregunta.Add(pregunta);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo insertar la pregunta", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error inesperado", ex.Message, "OK");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
    }
}