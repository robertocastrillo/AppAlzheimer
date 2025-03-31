using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuApp.Entidades
{
    public class ReqEliminarRelacion
    {
        public int IdUsuarioCuidador { get; set; }
        public int IdUsuarioPaciente { get; set; }
        public string CodigoPing { get; set; } // puede ser null
    }
}
