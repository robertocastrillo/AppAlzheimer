﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuApp.Entidades
{
    public class ReqCerrarSesion
    {
        public int IdUsuario { get; set; }
        public string Origen { get; set; } = "APP";
    }
}
