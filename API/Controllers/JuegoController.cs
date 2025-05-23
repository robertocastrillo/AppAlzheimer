﻿using Backend.Entidades;
using Backend.Logica.Juego;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class JuegoController : ApiController
    {
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/juego/insertarjuego")]
        public ResInsertarJuego insertarUsuario(ReqInsertarJuego req) //INVESTIGAR: Recibir y retornar HTTP
        {
            return new LogJuego().insertarJuego(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/juego/insertarpregunta")]
        public List<ResInsertarPregunta> insertarPregunta(ReqInsertarPregunta req) //INVESTIGAR: Recibir y retornar HTTP
        {
            return new LogJuego().insertarPregunta(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/juego/insertarrelacion")]
        public ResInsertarRelacionJuego insertarRelacionJuego(ReqInsertarRelacionJuego req) //INVESTIGAR: Recibir y retornar HTTP
        {
            return new LogJuego().insertarRelacionJuego(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/juego/obtenerjuegoscuidador")]
        public ResObtenerJuegosCuidador obtenerJuegosCuidador(ReqObtenerJuegosCuidador req) //INVESTIGAR: Recibir y retornar HTTP
        {
            return new LogJuego().obtenerJuegosCuidador(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/juego/obtenerjuegospaciente")]
        public List<ResObtenerJuegosPaciente> obtenerJuegosCuidador(ReqObtenerJuegosPaciente req) //INVESTIGAR: Recibir y retornar HTTP
        {
            return new LogJuego().obtenerJuegosPaciente(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/juego/obtenerpreguntas")]
        public ResObtenerPregunta obtenerPreguntas(ReqObtenerPregunta req) //INVESTIGAR: Recibir y retornar HTTP
        {
            return new LogJuego().obtenerPregunta(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/juego/eliminarjuego")]
        public ResEliminarJuego obtenerPreguntas(ReqEliminarJuego req) //INVESTIGAR: Recibir y retornar HTTP
        {
            return new LogJuego().eliminarJuego(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/juego/obtenerpuntaje")]
        public ResObtenerPuntaje obtenerPuntaje(ReqObtenerPuntaje req) //INVESTIGAR: Recibir y retornar HTTP
        {
            return new LogJuego().obtenerPuntaje(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/juego/insertarpuntaje")]
        public ResInsertarPuntaje insertarPuntaje(ReqInsertarPuntaje req) //INVESTIGAR: Recibir y retornar HTTP
        {
            return new LogJuego().insertarPuntaje(req);
        }

    }
}