﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Entidades
{
    public class ResObtenerJuegosCuidador : ResBase
    {
        public int idJuego { get; set; }
        public string nombre { get; set; }
        public int numPreguntas { get; set; }
    }
}
