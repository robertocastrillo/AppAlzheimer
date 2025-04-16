using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuApp.Entidades
{
    public class ResObtenerEventosCuidador : ResBase
    {
        public Evento eventos { get; set; }
        public List<Usuario> usuarios { get; set; }
    }
}
