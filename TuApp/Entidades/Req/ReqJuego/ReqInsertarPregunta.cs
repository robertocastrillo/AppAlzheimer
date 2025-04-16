using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuApp.Entidades
{
    public class ReqInsertarPregunta
    {
        public int idUsuario { get; set; }
        public int idJuego { get; set; }
        public List<Pregunta> preguntas { get; set; } = new();
    }
}
