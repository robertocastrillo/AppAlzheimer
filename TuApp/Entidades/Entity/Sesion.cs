using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuApp
{
    public class Sesion
    {
        public Usuario usuario { get; set; }
        public string tokem { get; set; }
        public DateTime expira { get; set; }
    }

}

