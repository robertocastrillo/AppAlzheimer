using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuApp.Entidades
{
    public class ReqActualizarEvento
    {
        public int IdEvento { get; set; }
        public int IdCuidador { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaHora { get; set; }
        public int IdPrioridad { get; set; }
    }
}
