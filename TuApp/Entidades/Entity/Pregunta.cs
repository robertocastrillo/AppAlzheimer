using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuApp.Entidades
{
    public class Pregunta
    {
        public int IdPregunta { get; set; }
        public string Descripcion { get; set; }
        public byte[] Imagen { get; set; }
        public List<Opcion> opciones { get; set; } = new List<Opcion>();
        public ImageSource ImagenSource
        {
            get
            {
                if (Imagen == null || Imagen.Length == 0)
                    return null;

                return ImageSource.FromStream(() => new MemoryStream(Imagen));
            }
        }
    }
}
