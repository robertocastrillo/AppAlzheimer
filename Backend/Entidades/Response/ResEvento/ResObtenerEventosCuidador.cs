using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Entidades.Response.ResEvento
{
    public class ResObtenerEventosCuidador : ResBase
    {
        public Evento evento { get; set; }
        public List<UsuarioEvento> usuarios { get; set; }

 
    }
    public class UsuarioEvento
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
