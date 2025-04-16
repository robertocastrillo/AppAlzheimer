using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Entidades
{
    public class ResObtenerJuegosCuidador : ResBase
    {
        public List<JuegoCuidador> juegosCuidadorList { get; set; }
    }

    public class JuegoCuidador
    {
        public int idJuego { get; set; }
        public string nombre { get; set; }
        public int numPreguntas { get; set; }
        public List<PacienteAsignado> pacientes { get; set; }
    }

    public class PacienteAsignado
    {
        public int id_Usuario { get; set; }
        public string nombre { get; set; }
    }
}
