using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuApp
{
    public class ReqIniciarSesion
    {
        public string CorreoElectronico { get; set; }
        public string Contrasena { get; set; }
        public string Origen { get; set; } = "APP";
    }
}
