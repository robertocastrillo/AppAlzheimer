﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuApp.Entidades
{
    public class ReqObtenerEvento
    {
        public bool resultado { get; set; }
        public List<Evento> listaEventos { get; set; }
        public List<Error> listaDeErrores { get; set; }

    }

}
