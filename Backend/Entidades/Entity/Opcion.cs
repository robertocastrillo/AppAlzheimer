﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Entidades
{
    public class Opcion
    {
        public int Id_Opcion { get; set; }
        public string Descripcion { get; set; }
        public bool Condicion { get; set; }
    }
}