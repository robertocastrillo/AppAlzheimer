using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuApp.Entidades.Req.ReqUsuario
{
    public class ReqActualizarPinPaciente
    {
        public string PinActual { get; set; }
        public string NuevoPin { get; set; }
        public int IdUsuario { get; set; }
    }
}
