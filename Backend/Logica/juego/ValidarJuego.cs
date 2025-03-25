﻿using Backend.Entidades;
using Backend.Entidades.Entity;
using Backend.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Logica.Juego
{
    public static class ValidarJuego
    {
        public static List<Error> validarJuego(ReqInsertarJuego req)
        {
            List<Error> errores = new List<Error>();
            Error error = new Error();

            if (req == null)
            {
                error.idError = (int)CatalogoErrores.requestNull;
                error.error = "Request null";
                errores.Add(error);
            }
            else
            {
                if (String.IsNullOrEmpty(req.nombre))
                {
                    error.idError = (int)CatalogoErrores.nombreJuegoNuloVacio;
                    error.error = "Falta el nombre del usuario";
                    errores.Add(error);
                }

                if (req.idUsuario < 1 )
                {
                    error.idError = (int)CatalogoErrores.usuarioInvalido;
                    error.error = "Falta el correo electronico del usuario";
                    errores.Add(error);
                }

            }
            return errores;
        }

        public static List<Error> validarPregunta(Pregunta pregunta, int idJuego)
        {
            List<Error> errores = new List<Error>();
            Error error = new Error();

            if (pregunta == null)
            {
                error.idError = (int)CatalogoErrores.requestNull;
                error.error = "Request null";
                errores.Add(error);
            }
            else
            {
                if (String.IsNullOrEmpty(pregunta.Descripcion))
                {

                    error.idError = (int)CatalogoErrores.descripcionNuloVacia;
                    error.error = "Falta la Descripcion";
                    errores.Add(error);
                }
                if(idJuego < 1)
                {

                    error.idError = (int)CatalogoErrores.juegoNoValido;
                    error.error = "Juego no valido";
                    errores.Add(error);
                }

            }
            bool opcionCorrecta = false;

            foreach (Opcion opcion in pregunta.opciones)
            {
                if (opcion == null)
                {

                    error.idError = (int)CatalogoErrores.opcionNula;
                    error.error = "Opcion Nula";
                    errores.Add(error);
                }
                else
                {
                    if (String.IsNullOrEmpty(opcion.Descripcion))
                    {

                        error.idError = (int)CatalogoErrores.descripcionNuloVacia;
                        error.error = "Falta la Descripcion";
                        errores.Add(error);
                    }
                    if (opcionCorrecta && opcion.Condicion)
                    {

                        error.idError = (int)CatalogoErrores.yaExisteOpcionCorrecta;
                        error.error = "Ya se asigno una opcion correcta";
                        errores.Add(error);
                    }
                    if (opcion.Condicion)
                    {
                        opcionCorrecta = true;
                    }


                }
            }
            if (!opcionCorrecta)
            {
                error.idError = (int)CatalogoErrores.noIngresoOpcionCorrecta;
                error.error = "No se asigno ninguna opcion correcta";
                errores.Add(error);
            }
            return errores;
        }

        public static List<Error> validarRelacion(ReqInsertarRelacionJuego req)
        {
            List<Error> errores = new List<Error>();
            Error error = new Error();


            if (req == null)
            {
                error.idError = (int)CatalogoErrores.requestNull;
                error.error = "Request null";
                errores.Add(error);
            }
            else
            {
                if(req.idPaciente < 1 )
                {
                    error.idError = (int)CatalogoErrores.usuarioInvalido;
                    error.error = "Usuario Paciente NO valido";
                    errores.Add(error);
                }
                if(req.idUsuario < 1)
                {
                    error.idError = (int)CatalogoErrores.usuarioInvalido;
                    error.error = "Usuario Cuidador NO valido";
                    errores.Add(error);
                }
                if(req.idJuego < 1)
                {
                    error.idError = (int)CatalogoErrores.juegoNoValido;
                    error.error = "Juego NO valido";
                    errores.Add(error);
                }
            }

                return errores;
        }

        public static List<Error> validarUsuario(int id)
        {
            List<Error> errores = new List<Error>();
            Error error = new Error();

            if (id < 1)
            {
                error.idError = (int)CatalogoErrores.usuarioInvalido;
                error.error = "Usuario NO valido";
                errores.Add(error);
            }

            return errores;
        }
    }
}
