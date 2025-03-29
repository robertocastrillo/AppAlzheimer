using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuApp.Entidades.Req.ReqUsuario
{
    public class ReqActualizarContrasena
    {
        public int IdUsuario { get; set; }
        public string ContrasenaActual { get; set; }
        public string NuevaContrasena { get; set; }
        public string Pin { get; set; } // Puede ser null
    }
}
