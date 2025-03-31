using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuApp.Entidades.Entity
{
    public class Juego
    {
        public int IdJuego { get; set; }
        public string Nombre { get; set; }
        public List<Pregunta> preguntas { get; set; } = new List<Pregunta>();

    }
}
