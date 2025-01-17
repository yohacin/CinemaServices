using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using wDualAssistence.Models;
using EPU = Entities.Public;
using BPU = Business.Public;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.Authorization;

namespace wDualAssistence.Controllers
{
    [Authorize]
    public class TareaController : MainController
    {
        public IActionResult Listado()
        {
            TareaViewModel vTareaViewModel = new TareaViewModel(this.User);

            //Cargo Items para el Menu
            vTareaViewModel.menuItems.Add(new
            {
                id = "1",
                text = "Editar",
                iconCss = "fa fa-pencil"
            });
            vTareaViewModel.menuItems.Add(new
            {
                id = "2",
                text = "Eliminar",
                iconCss = "fa fa-remove"
            });

            return View(vTareaViewModel);
        }

        /// <summary>
        /// Odata para listado
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [EnableQuery]
        [HttpGet]
        public ActionResult<IEnumerable<EPU.Tarea>> Get(ODataQueryOptions<EPU.Tarea> queryOptions)
        {
            try
            {
                var lista = new BPU.Tarea(this._appCnx).GetLista();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpGet("Tarea/GetCategoriaTarea")]
        public List<Entities.Helpers.TreeCollection> GetCategoriaTarea()
        {
            try
            {
                return new BPU.Tarea(this._appCnx).GetListaCategoriaTarea();
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
                TareaViewModel vTareaViewModel = new TareaViewModel(this.User);
                vTareaViewModel.listaCategoria_Tarea = new BPU.Categoria_Tarea(this._appCnx).GetLista();

                if (id != 0)
                {
                    vTareaViewModel.eTarea = new BPU.Tarea(this._appCnx).GetEntity(id);
                }
                else
                {
                    vTareaViewModel.eTarea = new EPU.Tarea();
                }

                return View(vTareaViewModel);
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
        public IActionResult Post(EPU.Tarea eTarea)
        {
            try
            {
                if (eTarea == null) throw new Exception("Los Datos no fueron recibidos");
                if (eTarea.nombre == null) throw new Exception("El campo nombre es requerido !");
                if (eTarea.id_categoria <= 0) throw new Exception("El campo categoria es requerido !");

                var eCategoriaRepetida = new BPU.Tarea(this._appCnx).GetEntityByNombre(eTarea.nombre);
                if (eTarea.id_categoria <= 0) throw new Exception("Debe seleccionar la categoria de la tarea !");

                eTarea.estado = 0;
                if (eTarea.id != 0)
                {
                    if (eCategoriaRepetida != null && eCategoriaRepetida.id != eTarea.id) throw new Exception("Ya existe una tarea registrada con el mismo nombre !");
                    new BPU.Tarea(this._appCnx).Modificar(eTarea);
                }
                else
                {
                    if (eCategoriaRepetida != null) throw new Exception("Ya existe una tarea registrada con el mismo nombre !");
                    new BPU.Tarea(this._appCnx).Guardar(eTarea);
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
                bool bEstaUsado = new BPU.Tarea(this._appCnx).EstaUsado(id);
                if (bEstaUsado)
                {
                    return Ok(new { transaccion = false, mensaje = "La Tarea ya tiene horas asignadas !" });
                }
                new BPU.Tarea(this._appCnx).Eliminar(id);
                return Ok(new { transaccion = true });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
            }
        }

        [HttpGet("Tarea/GetListaByEngagement/{id_engagement}")]
        public List<EPU.Tarea> GetListaByEngagement(long id_engagement)
        {
            try
            {
                return new BPU.Engagement(this._appCnx).GetListaTareaByEngagment(id_engagement);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }
    }
}