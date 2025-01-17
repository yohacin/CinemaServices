using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using wDualAssistence.Models;
using EPU = Entities.Public;
using BPU = Business.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData.Query;

namespace wDualAssistence.Controllers
{
    [Authorize]
    public class DiaFeriadoController : MainController
    {
        public IActionResult Listado()
        {
            DiaFeriadoViewModel vDiaFeriadoViewModel = new DiaFeriadoViewModel(this.User);

            //Cargo Items para el Menu
            vDiaFeriadoViewModel.menuItems.Add(new
            {
                id = "1",
                text = "Editar",
                iconCss = "fa fa-pencil"
            });
            vDiaFeriadoViewModel.menuItems.Add(new
            {
                id = "2",
                text = "Eliminar",
                iconCss = "fa fa-remove"
            });

            string vista = Startup.modo_independiente ? "ModoIndependiente/Listado" : "Listado";
            return View(vista, vDiaFeriadoViewModel);
        }

        /// <summary>
        /// Odata para listado
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [EnableQuery]
        [HttpGet]
        public ActionResult<IEnumerable<EPU.Dia_Feriado>> Get(ODataQueryOptions<EPU.Dia_Feriado> queryOptions)
        {
            try
            {
                var diaFeriadoBPU = new BPU.Dia_Feriado(this._appCnx);
                var lista = Startup.modo_independiente ? 
                        diaFeriadoBPU.GetListaModoIndependiente() : 
                        diaFeriadoBPU.GetLista();

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
                DiaFeriadoViewModel vDiaFeriadoViewModel = new DiaFeriadoViewModel(this.User);
                string vista = "Crear";

                if (Startup.modo_independiente)
                {
                    vDiaFeriadoViewModel.listaCiudades = new BPU.Ciudad(this._appCnx).GetLista();
                    vista = "ModoIndependiente/Crear";
                }

                if (id != 0)
                {
                    vDiaFeriadoViewModel.eDia_Feriado = new BPU.Dia_Feriado(this._appCnx).GetEntity(id);
                }
                else
                {
                    vDiaFeriadoViewModel.eDia_Feriado = new EPU.Dia_Feriado
                    {
                        fecha = DateTime.Now,
                        fecha_fin = Startup.modo_independiente ? DateTime.Now : null
                    };
                }

                return View(vista, vDiaFeriadoViewModel);
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
        public IActionResult Post(EPU.Dia_Feriado eDia_Feriado)
        {
            try
            {
                if (eDia_Feriado == null) throw new Exception("Los Datos no fueron recibidos");
                if (eDia_Feriado.descripcion == null) throw new Exception("El campo nombre es requerido !");

                var eDiaFeriadoRepetido = new BPU.Dia_Feriado(this._appCnx).GetEntityByFecha(eDia_Feriado.fecha);

                if (eDia_Feriado.id != 0)
                {
                    if (eDiaFeriadoRepetido != null && eDiaFeriadoRepetido.id != eDia_Feriado.id) throw new Exception("Ya existe un dia feriado registrado con la misma fecha !");
                    new BPU.Dia_Feriado(this._appCnx).Modificar(eDia_Feriado);
                }
                else
                {
                    if (eDiaFeriadoRepetido != null) throw new Exception("Ya existe un dia feriado registrado con la misma fecha !");
                    new BPU.Dia_Feriado(this._appCnx).Guardar(eDia_Feriado);
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
                new BPU.Dia_Feriado(this._appCnx).Eliminar(id);
                return Ok(new { transaccion = true });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
            }
        }


        /// <summary>
        ///  Para la parte de carga de datos en base
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GuardarFeriadoModoIndependiente(EPU.Dia_Feriado eDia_Feriado)
        {
            try
            {
                if (eDia_Feriado == null) throw new Exception("Los Datos no fueron recibidos");
                if (eDia_Feriado.descripcion == null) throw new Exception("El campo descripción es requerido !");

                var eDiaFeriadoRepetido = new BPU.Dia_Feriado(this._appCnx).GetEntityByFecha(eDia_Feriado.fecha);

                if (eDia_Feriado.id != 0)
                {
                    if (eDiaFeriadoRepetido != null && eDiaFeriadoRepetido.id != eDia_Feriado.id) throw new Exception("Ya existe un dia feriado registrado con la misma fecha !");
                    new BPU.Dia_Feriado(this._appCnx).Modificar(eDia_Feriado);
                }
                else
                {
                    if (eDiaFeriadoRepetido != null) throw new Exception("Ya existe un dia feriado registrado con la misma fecha !");
                    new BPU.Dia_Feriado(this._appCnx).Guardar(eDia_Feriado);
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