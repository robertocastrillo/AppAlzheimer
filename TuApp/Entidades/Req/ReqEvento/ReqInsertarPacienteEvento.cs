using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuApp.Entidades
{
    public class ReqInsertarPacienteEvento
    {
        public int IdEvento { get; set; }
        public int IdCuidador { get; set; }
        public int IdPaciente { get; set; }
    }

}
