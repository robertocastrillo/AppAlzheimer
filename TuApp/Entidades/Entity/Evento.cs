﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuApp.Entidades
{
    public class Evento
    {
        public int IdEvento { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; } // Puede ser null
        public DateTime FechaHora { get; set; }
        public int IdPrioridad { get; set; }
        public int IdUsuario { get; set; } // Cuidador
    }

