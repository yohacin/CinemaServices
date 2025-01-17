using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using wDualAssistence.Models;
using EPU = Entities.Public;
using BPU = Business.Public;
using Microsoft.AspNetCore.OData.Query;
using wDualAssistence.Helpers;
using Entities.Helpers;
using System.Linq;

namespace wDualAssistence.Controllers
{
    [Authorize]
    public class EmpleadoController : MainController
    {
        public IActionResult Listado()
        {
            EmpleadoViewModel vEmpleadoViewModel = new EmpleadoViewModel(this.User);
            if (!this._RevisarAcceso(vEmpleadoViewModel.idUsuario, vEmpleadoViewModel.tipo, Url.ActionContext.HttpContext.Request.Path))
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }
            //Cargo Items para el Menu
            vEmpleadoViewModel.menuItems.Add(new
            {
                id = "1",
                text = "Editar",
                iconCss = "fa fa-pencil"
            });
            //vEmpleadoViewModel.menuItems.Add(new
            //{
            //    id = "2",
            //    text = "Eliminar",
            //    iconCss = "fa fa-remove"
            //});

            string vista = Startup.modo_independiente ? "ModoIndependiente/Listado" : "Listado";
            return View(vista, vEmpleadoViewModel);

        }

        /// <summary>
        /// Odata para listado de usuarios
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [EnableQuery]
        [HttpGet]
        public ActionResult<IEnumerable<EPU.Empleado>> Get(ODataQueryOptions<EPU.Empleado> queryOptions)
        {
            try
            {
                var empleadoBPU = new BPU.Empleado(this._appCnx);
                var lista = Startup.modo_independiente
                            ? empleadoBPU.GetListaModoIndependiente()
                            : empleadoBPU.GetLista();

                return Ok(lista);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpPost]
        public IActionResult Crear([FromForm] long id)
        {
            try
            {
                EmpleadoViewModel vEmpleadoViewModel = new EmpleadoViewModel(this.User);
                vEmpleadoViewModel.listaPerfil = new Business.Security.Perfil(this._appCnx).GetLista();

                if (id != 0)
                {
                    vEmpleadoViewModel.eEmpleado = new BPU.Empleado(this._appCnx).GetEntityConTurno(id);
                    if (vEmpleadoViewModel.eEmpleado == null) vEmpleadoViewModel.eEmpleado = new EPU.Empleado();
                    if (vEmpleadoViewModel.eEmpleado.listaTurno == null) vEmpleadoViewModel.eEmpleado.listaTurno = new List<EPU.Turno>();
                }
                else
                {
                    vEmpleadoViewModel.eEmpleado = new EPU.Empleado();
                    vEmpleadoViewModel.eEmpleado.listaTurno = new List<EPU.Turno>();
                    vEmpleadoViewModel.eEmpleado.fecha_nacimiento = DateTime.Now;
                }

                string vista = "Crear";
                if (Startup.modo_independiente)
                {
                    vEmpleadoViewModel.listaJornadas = new BPU.Jornada(this._appCnx).GetLista();
                    vEmpleadoViewModel.listaSucursales = new BPU.Sucursal(this._appCnx).GetLista();
                    vEmpleadoViewModel.foto_empleado = new BPU.Empleado(this._appCnx).ObtenerFotos(id, (int)EPU.TipoFotoEmpleado.Perfil).FirstOrDefault();
                    vista = "ModoIndependiente/Crear";
                }

                return View(vista, vEmpleadoViewModel);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpGet("Empleado/GetEmpresaByEmpleado/{idEmpleado}")]
        public List<EPU.Empresa> GetEmpresaByEmpleado(long idEmpleado)
        {
            try
            {
                return new BPU.Empresa(this._appCnx).GetListaByEmpleado(idEmpleado);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpGet("Empleado/GetTurnoByEmpleado/{idEmpleado}")]
        public List<EPU.Turno> GetTurnoByEmpleado(long idEmpleado)
        {
            try
            {
                return new BPU.Empleado(this._appCnx).GetTurnoByEmpleado(idEmpleado);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        ///  Para la parte de carga de datos en base
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post(EmpleadoViewModel vEmpleadoViewModel)
        {
            try
            {
                vEmpleadoViewModel.eEmpleado.estado = 0;
                vEmpleadoViewModel.eEmpleado.fcm = "0";
                if (vEmpleadoViewModel.eEmpleado.id_perfil <= 0)
                    throw new Exception("Debe seleccionar el perfil del empleado");
                if (vEmpleadoViewModel.eEmpleado.area != null) vEmpleadoViewModel.eEmpleado.area = vEmpleadoViewModel.eEmpleado.area.ToUpper();
                if (vEmpleadoViewModel.eEmpleado.id != 0)
                {
                    // cambio para solo modificar el id_perfil del empleado
                    EPU.Empleado eEmpleadoRepetido = new BPU.Empleado(this._appCnx).GetEntity(vEmpleadoViewModel.eEmpleado.id);
                    //vEmpleadoViewModel.eEmpleado.clave = eEmpleadoRepetido.clave;
                    eEmpleadoRepetido.id_perfil = vEmpleadoViewModel.eEmpleado.id_perfil;
                    new BPU.Empleado(this._appCnx).Modificar(eEmpleadoRepetido);
                }
                else
                {
                    vEmpleadoViewModel.eEmpleado.clave = Cryptography.Encrypt(vEmpleadoViewModel.eEmpleado.clave, Startup.llaveCryptography);
                    new BPU.Empleado(this._appCnx).Guardar(vEmpleadoViewModel.eEmpleado);
                }
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
                throw ex;
            }
            return Ok(new { transaccion = true });
        }

        [HttpPost]
        public IActionResult setContrasena(long id_usuario, string nueva_contrasena)
        {
            try
            {
                nueva_contrasena = Cryptography.EncriptarMD5(nueva_contrasena);
                if (new BPU.Empleado(this._appCnx).setContrasena(id_usuario, nueva_contrasena))
                {
                    return Ok(new { transaccion = true, mensaje = "¡ Contraseña cambiada correctamente !" });
                }
                else
                {
                    return Ok(new { transaccion = false, mensaje = "¡ Error al cambiar la contraseña !" });
                }
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                new BPU.Empleado(this._appCnx).Eliminar(id);
                return Ok(new { transaccion = true });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpGet("Empleado/GetListaByArea/{area}")]
        public List<EPU.Empleado> GetListaByArea(string area)
        {
            try
            {
                return new BPU.Empleado(this._appCnx).GetListaByArea(area);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }


        /// <summary>
        ///  Version del POST para el modo independiente
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GuardarModoIndependiente(EmpleadoViewModel vEmpleadoViewModel)
        {
            try
            {
                vEmpleadoViewModel.eEmpleado.estado = 0;
                vEmpleadoViewModel.eEmpleado.fcm = "0";
                if (vEmpleadoViewModel.eEmpleado.id_perfil <= 0)
                    throw new Exception("Debe seleccionar el perfil del empleado");
                if (vEmpleadoViewModel.eEmpleado.area != null) vEmpleadoViewModel.eEmpleado.area = vEmpleadoViewModel.eEmpleado.area.ToUpper();
                if (vEmpleadoViewModel.eEmpleado.id_sucursal > 0)
                {
                    EPU.Sucursal eSucursal = new BPU.Sucursal(this._appCnx).GetEntity(vEmpleadoViewModel.eEmpleado.id_sucursal);
                    if (eSucursal != null)
                    {
                        vEmpleadoViewModel.eEmpleado.nombre_sucursal = eSucursal.nombre;
                    }
                }
                if (vEmpleadoViewModel.eEmpleado.id != 0)
                {
                    EPU.Empleado eEmpleadoRepetido = new BPU.Empleado(this._appCnx).GetEntity(vEmpleadoViewModel.eEmpleado.id);

                    vEmpleadoViewModel.eEmpleado.fecha_ingreso = eEmpleadoRepetido.fecha_ingreso;
                    vEmpleadoViewModel.eEmpleado.fecha_salida = eEmpleadoRepetido.fecha_salida;
                    vEmpleadoViewModel.eEmpleado.cargo = eEmpleadoRepetido.cargo;
                    vEmpleadoViewModel.eEmpleado.id_sucursal = eEmpleadoRepetido.id_sucursal;
                    vEmpleadoViewModel.eEmpleado.nombre_sucursal = eEmpleadoRepetido.nombre_sucursal;
                    //vEmpleadoViewModel.eEmpleado.url_foto = eEmpleadoRepetido.url_foto;
                    
                    new BPU.Empleado(this._appCnx).ModificarEmpleadoModoIndependiente(vEmpleadoViewModel.eEmpleado);
                }
                else
                {
                    vEmpleadoViewModel.eEmpleado.fecha_ingreso = DateTime.Now.AddDays(-1);
                    vEmpleadoViewModel.eEmpleado.fecha_salida = DateTime.Now.AddYears(100);
                    vEmpleadoViewModel.eEmpleado.cargo = "";
                    vEmpleadoViewModel.eEmpleado.id_sucursal = 0;
                    vEmpleadoViewModel.eEmpleado.nombre_sucursal = "";

                    if (vEmpleadoViewModel.eEmpleado.id_jornada != null && vEmpleadoViewModel.eEmpleado.id_jornada > 0)
                    {
                        // Como es nuevo empleado se debe añadir sus turnos
                        EPU.Jornada eJornada = new BPU.Jornada(this._appCnx).GetEntity(vEmpleadoViewModel.eEmpleado.id_jornada.Value);
                        if (eJornada.listaDetalleJornada != null && eJornada.listaDetalleJornada.Any())
                        {
                            vEmpleadoViewModel.eEmpleado.listaHorario = new List<HorarioHelper>();

                            foreach (var detalle in eJornada.listaDetalleJornada)
                            {
                                var horario = new HorarioHelper()
                                {
                                    codigo_horario = (int)detalle.id,
                                    hora_entrada = detalle.hora_entrada,
                                    hora_inicio_entrada = detalle.inicio_marcacion_entrada,
                                    hora_fin_entrada = detalle.fin_marcacion_entrada,
                                    hora_salida = detalle.hora_salida,
                                    hora_inicio_salida = detalle.inicio_marcacion_salida,
                                    hora_fin_salida = detalle.fin_marcacion_salida,
                                    codigo_dia = detalle.dia,
                                };

                                vEmpleadoViewModel.eEmpleado.listaHorario.Add(horario);
                            }
                        }
                    }

                    vEmpleadoViewModel.eEmpleado.clave = Cryptography.EncriptarMD5(vEmpleadoViewModel.eEmpleado.clave);
                    new BPU.Empleado(this._appCnx).GuardarNuevoEmpleado(vEmpleadoViewModel.eEmpleado);
                }
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
                throw ex;
            }
            return Ok(new { transaccion = true });
        }

    }
}