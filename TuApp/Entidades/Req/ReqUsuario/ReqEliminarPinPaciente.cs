using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuApp.Entidades.Req.ReqUsuario
{
    public class ReqEliminarPinPaciente
    {
        public int IdUsuario { get; set; }
        public string Codigo { get; set; }
    }
}
