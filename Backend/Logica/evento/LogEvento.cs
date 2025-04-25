using AccesoDatos;
using Backend.Entidades;
using Backend.Entidades.Response.ResEvento;
using Backend.Logica.Usuario.Varios;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Logica
{
   public class LogEvento
    {
        //Metodos Evento
        public ResInsertarEvento insertarEvento(ReqInsertarEvento req)
        {
            ResInsertarEvento res = new ResInsertarEvento();
            res.listaDeErrores = new List<Error>();

            try
            {
                // Validar la solicitud
                res.listaDeErrores = Validaciones.validarInsertarEvento(req);

                if (!res.listaDeErrores.Any()) // Si no hay errores, ejecutar el SP
                {
                    int? idReturn = 0;
                    int? errorId = 0;
                    string errorCode = "";
                    string errorDescrip = "";

                    using (MiLinqDataContext linq = new MiLinqDataContext())
                    {
                        linq.SP_INSERTAR_EVENTO(req.IdCuidador, req.Titulo, req.Descripcion, req.FechaHora, req.IdPrioridad, ref idReturn, ref errorId, ref errorCode, ref errorDescrip);
                    }

                    if (idReturn != null && idReturn > 0)
                    {
                        res.resultado = true;
                    }
                    else
                    {
                        res.resultado = false;
                        res.listaDeErrores.Add(new Error
                        {
                            idError = (int)errorId,
                            error = errorCode
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                res.resultado = false;
                res.listaDeErrores.Add(new Error
                {
                    idError = -1,
                    error = ex.Message
                });
            }

            return res;
        }
        public ResActualizarEvento actualizarEvento(ReqActualizarEvento req)
        {
            ResActualizarEvento res = new ResActualizarEvento();
            res.listaDeErrores = new List<Error>();

            try
            {
                // Validar los datos de la solicitud
                res.listaDeErrores = Validaciones.validarActualizarEvento(req);

                if (!res.listaDeErrores.Any())
                {
                    int? errorId = 0;
                    string errorCode = "";
                    string errorDescrip = "";

                    using (MiLinqDataContext linq = new MiLinqDataContext())
                    {
                        linq.SP_ACTUALIZAR_EVENTO(req.IdEvento, req.IdCuidador, req.Titulo, req.Descripcion, req.FechaHora, req.IdPrioridad, ref errorId, ref errorCode, ref errorDescrip);
                    }

                    if (errorId == null || errorId == 0)
                    {
                        res.resultado = true;
                    }
                    else // Manejar errores del SP
                    {
                        res.resultado = false;
                        res.listaDeErrores.Add(new Error
                        {
                            idError = (int)errorId,
                            error = errorCode
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                res.resultado = false;
                res.listaDeErrores.Add(new Error
                {
                    idError = -1,
                    error = ex.Message
                });
            }

            return res;
        }
        public ResEliminarEvento eliminarEvento(ReqEliminarEvento req)
        {
            ResEliminarEvento res = new ResEliminarEvento();
            res.listaDeErrores = new List<Error>();

            try
            {
                // Validaciones
                res.listaDeErrores = Validaciones.validarEliminarEvento(req);

                if (!res.listaDeErrores.Any()) // Si no hay errores, proceder con la eliminación
                {
                    int? errorId = 0;
                    string errorCode = "";
                    string errorDescrip = "";

                    using (MiLinqDataContext linq = new MiLinqDataContext())
                    {
                        linq.SP_ELIMINAR_EVENTO(req.IdEvento, req.IdCuidador, ref errorId, ref errorCode, ref errorDescrip);
                    }

                    if (errorId == null || errorId == 0) // Si no hay errores, eliminación exitosa
                    {
                        res.resultado = true;
                    }
                    else
                    {
                        res.resultado = false;
                        res.listaDeErrores.Add(new Error
                        {
                            idError = (int)errorId,
                            error = errorCode
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                res.resultado = false;
                res.listaDeErrores.Add(new Error
                {
                    idError = -1,
                    error = ex.Message
                });
            }

            return res;
        }
        public ResInsertarPacienteEvento insertarPacienteEvento(ReqInsertarPacienteEvento req)
        {
            ResInsertarPacienteEvento res = new ResInsertarPacienteEvento();
            res.listaDeErrores = new List<Error>();

            try
            {
                // Validaciones
                res.listaDeErrores = Validaciones.validarInsertarPacienteEvento(req);

                if (!res.listaDeErrores.Any())
                {
                    int? idReturn = 0;
                    int? errorId = 0;
                    string errorCode = "";
                    string errorDescrip = "";

                    using (MiLinqDataContext linq = new MiLinqDataContext())
                    {
                        linq.SP_INSERTAR_PACIENTE_EVENTO(req.IdEvento, req.IdCuidador, req.IdPaciente, ref idReturn, ref errorId, ref errorCode, ref errorDescrip);
                    }
                    if (idReturn.HasValue && idReturn > 0)
                    {
                        res.resultado = true;
                    }
                    else
                    {
                        res.resultado = false;
                        res.listaDeErrores.Add(new Error
                        {
                            idError = (int)errorId,
                            error = errorCode
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                res.resultado = false;
                res.listaDeErrores.Add(new Error
                {
                    idError = -1,
                    error = ex.Message
                });
            }

            return res;
        }
        public ResEliminarPacienteEvento eliminarPacienteEvento(ReqEliminarPacienteEvento req)
        {
            ResEliminarPacienteEvento res = new ResEliminarPacienteEvento();
            res.listaDeErrores = new List<Error>();

            try
            {
                // Validaciones básicas
                res.listaDeErrores = Validaciones.validarEliminarPacienteEvento(req);

                if (!res.listaDeErrores.Any())
                {
                    int? errorId = 0;
                    string errorCode = "";
                    string errorDescrip = "";

                    using (MiLinqDataContext linq = new MiLinqDataContext())
                    {
                        linq.SP_ELIMINAR_PACIENTE_EVENTO(
                            req.IdEvento,
                            req.IdCuidador,
                            req.IdPaciente,
                            ref errorId,
                            ref errorCode,
                            ref errorDescrip
                        );
                    }

                    if (errorId == null || errorId == 0)
                    {
                        res.resultado = true;
                    }
                    else
                    {
                        res.resultado = false;
                        res.listaDeErrores.Add(new Error
                        {
                            idError = (int)errorId,
                            error = errorCode
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                res.resultado = false;
                res.listaDeErrores.Add(new Error
                {
                    idError = -1,
                    error = ex.Message
                });
            }

            return res;
        }
        public ResObtenerEventosPaciente obtenerEventosPaciente(ReqObtenerEventosPaciente req)
        {
            ResObtenerEventosPaciente res = new ResObtenerEventosPaciente();
            res.listaDeErrores = new List<Error>();
            res.eventos = new List<Evento>();

            try
            {

                res.listaDeErrores = Validaciones.validarObtenerEventosPaciente(req);

                if (!res.listaDeErrores.Any())
                {
                    int? idReturn = 0;
                    int? errorId = 0;
                    string errorCode = "";
                    string errorDescrip = "";


                    using (MiLinqDataContext linq = new MiLinqDataContext())
                    {
                        List<SP_OBTENER_EVENTOS_PACIENTEResult> listaeventos = new List<SP_OBTENER_EVENTOS_PACIENTEResult>();

                        listaeventos = linq.SP_OBTENER_EVENTOS_PACIENTE(
                            req.IdPaciente,
                            ref idReturn,
                            ref errorId,
                            ref errorCode,
                            ref errorDescrip
                        ).ToList();

                        foreach (SP_OBTENER_EVENTOS_PACIENTEResult evento in listaeventos)
                        {
                            res.eventos.Add(this.factoriaevento(evento));
                        }
                        res.resultado = true;

                    }

                    if (idReturn > 0 && (errorId == null || errorId == 0))
                    {
                        res.resultado = true;
                    }
                    else
                    {
                        res.resultado = false;
                        res.listaDeErrores.Add(new Error
                        {
                            idError = (int)(errorId ?? -1),
                            error = errorCode
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                res.resultado = false;
                res.listaDeErrores.Add(new Error
                {
                    idError = -1,
                    error = ex.Message
                });
            }

            return res;
        }
        private Evento factoriaevento(SP_OBTENER_EVENTOS_PACIENTEResult evento)
        {
            Evento eventosretornar = new Evento();
            eventosretornar.IdEvento = evento.ID_EVENTO;
            eventosretornar.Titulo = evento.TITULO;
            eventosretornar.Descripcion = evento.DESCRIPCION;
            eventosretornar.FechaHora = evento.FECHA_HORA;
            eventosretornar.IdPrioridad = evento.ID_PRIORIDAD;
            eventosretornar.Id_Cuidador = evento.ID_CUIDADOR;
            eventosretornar.Nombre_Cuidador = evento.NOMBRE_CUIDADOR;



            return eventosretornar;

        }
        public List<ResObtenerEventosCuidador> obtenerEventosCuidador(ReqObtenerEventosCuidador req)
        {
            List<ResObtenerEventosCuidador> res = new List<ResObtenerEventosCuidador>();
            ResObtenerEventosCuidador eventoCuidador = new ResObtenerEventosCuidador();
            eventoCuidador.listaDeErrores = new List<Error>();

            try
            {
                eventoCuidador.listaDeErrores = Validaciones.validarObtenerEventosCuidador(req);

                if (!eventoCuidador.listaDeErrores.Any())
                {
                    int? errorId = 0;
                    string errorCode = "";
                    string errorDescrip = "";

                    using (MiLinqDataContext linq = new MiLinqDataContext())
                    {
                        var resultado = linq.SP_OBTENER_EVENTOS_CUIDADOR(req.IdCuidador, ref errorId, ref errorCode, ref errorDescrip).ToList();

                        if (errorId < 1 || errorId == null)
                        {
                            res = factoryEventoCuidador(resultado);
                        }

                        if (errorId == null || errorId == 0)
                        {
                            eventoCuidador.resultado = true;
                        }
                        else
                        {
                            eventoCuidador.resultado = false;
                            eventoCuidador.listaDeErrores.Add(new Error
                            {
                                idError = (int)errorId,
                                error = errorCode
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                eventoCuidador.resultado = false;
                eventoCuidador.listaDeErrores.Add(new Error
                {
                    idError = -1,
                    error = ex.Message
                });
            }

            return res;
        }



        private List<ResObtenerEventosCuidador> factoryEventoCuidador(List<SP_OBTENER_EVENTOS_CUIDADORResult> item)
        {
            List<ResObtenerEventosCuidador> res = new List<ResObtenerEventosCuidador>();


            foreach (var eventos in item)
            {
                ResObtenerEventosCuidador resEvento = new ResObtenerEventosCuidador();

                Evento evento = new Evento();
                evento.IdEvento = eventos.ID_EVENTO;
                evento.Titulo = eventos.TITULO;
                evento.Descripcion = eventos.DESCRIPCION;
                evento.FechaHora = eventos.FECHA_HORA;
                evento.IdPrioridad = eventos.ID_PRIORIDAD;
                evento.Id_Cuidador = 0;
                List<UsuarioEvento> usuarios = new List<UsuarioEvento>();
                if (!string.IsNullOrWhiteSpace(eventos.PACIENTES))
                {
                    usuarios = JsonConvert.DeserializeObject<List<UsuarioEvento>>(eventos.PACIENTES);
                }
                resEvento.evento = evento;
                resEvento.usuarios = usuarios;
                resEvento.resultado = true;
                res.Add(resEvento);
            }
            return res;
        }



    }
}
