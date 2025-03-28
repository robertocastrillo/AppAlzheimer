using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuApp
{
    public static class Sesion
    {
        public static Usuario Usuario { get; set; }
        public static string Tokem { get; set; }
        public static DateTime Expira { get; set; }
    }

}

