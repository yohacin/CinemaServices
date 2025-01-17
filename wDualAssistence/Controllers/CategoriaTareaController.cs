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
    public class CategoriaTareaController : MainController
    {
        public IActionResult Listado()
        {
            CategoriaTareaViewModel vCategoriaTareaViewModel = new CategoriaTareaViewModel(this.User);

            //Cargo Items para el Menu
            vCategoriaTareaViewModel.menuItems.Add(new
            {
                id = "1",
                text = "Editar",
                iconCss = "fa fa-pencil"
            });
            vCategoriaTareaViewModel.menuItems.Add(new
            {
                id = "2",
                text = "Eliminar",
                iconCss = "fa fa-remove"
            });

            return View(vCategoriaTareaViewModel);
        }

        /// <summary>
        /// Odata para listado
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [EnableQuery]
        [HttpGet]
        public ActionResult<IEnumerable<EPU.Categoria_Tarea>> Get(ODataQueryOptions<EPU.Categoria_Tarea> queryOptions)
        {
            try
            {
                var lista = new BPU.Categoria_Tarea(this._appCnx).GetLista();
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
                CategoriaTareaViewModel vCategoriaTareaViewModel = new CategoriaTareaViewModel(this.User);

                if (id != 0)
                {
                    vCategoriaTareaViewModel.eCategoria_Tarea = new BPU.Categoria_Tarea(this._appCnx).GetEntity(id);
                }
                else
                {
                    vCategoriaTareaViewModel.eCategoria_Tarea = new EPU.Categoria_Tarea();
                }

                return View(vCategoriaTareaViewModel);
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
        public IActionResult Post(EPU.Categoria_Tarea eCategoria_Tarea)
        {
            try
            {
                if (eCategoria_Tarea == null) throw new Exception("Los Datos no fueron recibidos");
                if (eCategoria_Tarea.nombre == null) throw new Exception("El campo nombre es requerido");

                eCategoria_Tarea.estado = 0;
                var eCategoriaRepetida = new BPU.Categoria_Tarea(this._appCnx).GetEntityByNombre(eCategoria_Tarea.nombre);
                if (eCategoria_Tarea.id != 0)
                {
                    if (eCategoriaRepetida != null && eCategoriaRepetida.id != eCategoria_Tarea.id) throw new Exception("Ya existe un categoria registrada con el mismo nombre !");
                    new BPU.Categoria_Tarea(this._appCnx).Modificar(eCategoria_Tarea);
                }
                else
                {
                    if (eCategoriaRepetida != null) throw new Exception("Ya existe un categoria registrada con el mismo nombre !");
                    new BPU.Categoria_Tarea(this._appCnx).Guardar(eCategoria_Tarea);
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
                bool bEstaUsado = new BPU.Categoria_Tarea(this._appCnx).EstaUsado(id);
                if (bEstaUsado)
                {
                    return Ok(new { transaccion = false, mensaje = "La Categoria ya tiene tareas con horas asignadas !" });
                }
                new BPU.Categoria_Tarea(this._appCnx).Eliminar(id);
                return Ok(new { transaccion = true });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
            }
        }
    }
}