using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuApp.Entidades.Entity;

namespace TuApp.Entidades
{
    public class ResObtenerJuegosCuidador : ResBase
    {
        public int IdJuego { get; set; }
        public string Nombre { get; set; }
        public int numPreg { get; set; }
    }
}
