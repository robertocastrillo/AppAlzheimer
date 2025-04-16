using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuApp.Entidades.Entity;

namespace TuApp.Entidades
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
        public ObservableCollection<PacienteAsignado> pacientes { get; set; } = new ObservableCollection<PacienteAsignado>();

    }

    public class PacienteAsignado
    {
        public int id_Usuario { get; set; }
        public string nombre { get; set; }
    }
}
