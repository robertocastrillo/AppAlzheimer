using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuApp.Entidades
{
    public class ResObtenerJuegosPaciente : ResBase
    {
        public int idJuego { get; set; }
        public string nombre { get; set; }
        public int numPreguntas { get; set; }
    }

    public class JuegoPaciente
    {
        public int idJuego { get; set; }
        public string nombre { get; set; }
        public int numPreguntas { get; set; }
    }

}
