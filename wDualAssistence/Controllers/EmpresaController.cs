using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using wDualAssistence.Models;

using EPU = Entities.Public;
using BPU = Business.Public;
using Microsoft.AspNetCore.OData.Query;
using Syncfusion.EJ2.Navigations;
using Newtonsoft.Json;

namespace wDualAssistence.Controllers
{
    [Authorize]
    public class EmpresaController : MainController
    {
        public IActionResult Listado()
        {
            EmpresaViewModel vEmpresaViewModel = new EmpresaViewModel(this.User);
            if (!this._RevisarAcceso(vEmpresaViewModel.idUsuario, vEmpresaViewModel.tipo, Url.ActionContext.HttpContext.Request.Path))
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }

            string vista = "ModoIndependiente/Listado";

            if (!Startup.modo_independiente)
            {
                vista = "Listado";
                vEmpresaViewModel.menuItems.Add(new
                {
                    id = "1",
                    text = "Engagement",
                    iconCss = "fa fa-cogs"
                });
            }
            vEmpresaViewModel.menuItems.Add(new
            {
                id = "2",
                text = "Editar",
                iconCss = "fa fa-pencil"
            });
            vEmpresaViewModel.menuItems.Add(new
            {
                id = "3",
                text = "Eliminar",
                iconCss = "fa fa-remove"
            });

            return View(vista, vEmpresaViewModel);
        }

        /// <summary>
        /// Odata para listado de usuarios
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [EnableQuery]
        [HttpGet]
        public ActionResult<IEnumerable<EPU.Empresa>> Get(ODataQueryOptions<EPU.Empresa> queryOptions)
        {
            try
            {
                var lista = new BPU.Empresa(this._appCnx).GetLista();
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
                EmpresaViewModel vEmpresaViewModel = new EmpresaViewModel(this.User);
                vEmpresaViewModel.listaEmpleado = new BPU.Empleado(this._appCnx).GetListaSinTurno();

                List<string> groupedArea = vEmpresaViewModel.listaEmpleado
                .GroupBy(u => u.area)
                .Select(grp => grp.Key)
                .ToList();

                ViewBag.listaAreas = groupedArea;

                vEmpresaViewModel.listaEmpleado = new List<EPU.Empleado>();

                if (id != 0)
                {
                    vEmpresaViewModel.eEmpresa = new BPU.Empresa(this._appCnx).GetEntity(id);
                    if (vEmpresaViewModel.eEmpresa.listaEmpleado == null) vEmpresaViewModel.eEmpresa.listaEmpleado = new List<EPU.Empleado>();
                }
                else
                {
                    vEmpresaViewModel.eEmpresa = new EPU.Empresa();
                    vEmpresaViewModel.eEmpresa.perimetro = 50;
                    vEmpresaViewModel.eEmpresa.latitud = -17.805742852443;
                    vEmpresaViewModel.eEmpresa.longitud = -63.1601130589843;
                    vEmpresaViewModel.eEmpresa.listaEmpleado = new List<EPU.Empleado>();
                    vEmpresaViewModel.eEmpresa.listFotos = new List<EPU.Foto_Empresa>();
                }

                string vista = "Crear";
                string tituloTab = "Empleados";

                if (Startup.modo_independiente)
                {
                    vista = "ModoIndependiente/Crear";
                    tituloTab = "Funcionarios";
                }

                ViewBag.positionData = new string[] { "Scrollable", "Popup" };
                ViewBag.orientationData = new string[] { "Top", "Bottom", "Left", "Right" };
                ViewBag.headerTextThree = new TabHeader { Text = tituloTab, IconCss = "fa e-group" };
                ViewBag.headerTextTwo = new TabHeader { Text = "Ubicacion", IconCss = "fa fa-map-marker" };
                ViewBag.headerTextOne = new TabHeader { Text = "Logo", IconCss = "fa e-photo" };
                ViewBag.data = new string[] { "Bolivianos", "Dolares" };

                return View(vista, vEmpresaViewModel);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpGet]
        public IActionResult Engagement(long id)
        {
            try
            {
                EmpresaViewModel vEmpresaViewModel = new EmpresaViewModel(this.User);

                if (id == 0) throw new Exception("No existe una empresa con codigo:" + id);

                vEmpresaViewModel.listaEngagement = new BPU.Engagement(this._appCnx).GetListaByEmpresa(id);
                vEmpresaViewModel.nombre_empresa = new BPU.Empresa(this._appCnx).GetEntityNombre(id);
                vEmpresaViewModel.id_empresa = id;

                //Cargo Items para el Menu
                vEmpresaViewModel.menuItems.Add(new
                {
                    id = "1",
                    text = "Asociacion Empleado",
                    iconCss = "fa fa-cogs"
                });
                vEmpresaViewModel.menuItems.Add(new
                {
                    id = "2",
                    text = "Editar",
                    iconCss = "fa fa-pencil"
                });
                vEmpresaViewModel.menuItems.Add(new
                {
                    id = "3",
                    text = "Eliminar",
                    iconCss = "fa fa-remove"
                });

                return View(vEmpresaViewModel);
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
        public IActionResult Post(EmpresaViewModel vEmpresaViewModel)
        {
            try
            {
                BPU.Empresa oEmpresa = new BPU.Empresa(this._appCnx);
                if (vEmpresaViewModel.stringListaPerfiles == null) throw new Exception("La lista de empleados no puede ser nulo");
                vEmpresaViewModel.listaEmpleado = JsonConvert.DeserializeObject<List<EPU.Empleado>>(vEmpresaViewModel.stringListaPerfiles);
                if (vEmpresaViewModel.listaEmpleado == null) vEmpresaViewModel.listaEmpleado = new List<EPU.Empleado>();
                vEmpresaViewModel.eEmpresa.listaEmpleado_Empresa = new List<EPU.Empleado_Empresa>();
                foreach (EPU.Empleado item in vEmpresaViewModel.listaEmpleado)
                {
                    EPU.Empleado_Empresa eEmpleado_Empresa = new EPU.Empleado_Empresa();
                    eEmpleado_Empresa.id_empleado = item.id;
                    eEmpleado_Empresa.codigo_empleado = item.codigo;
                    eEmpleado_Empresa.fecha_inicio = DateTime.Now;
                    vEmpresaViewModel.eEmpresa.listaEmpleado_Empresa.Add(eEmpleado_Empresa);
                }

                if (vEmpresaViewModel.eEmpresa.id != 0)
                {
                    oEmpresa.Modificar(vEmpresaViewModel.eEmpresa);
                }
                else
                {
                    oEmpresa.Guardar(vEmpresaViewModel.eEmpresa);
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
                new BPU.Empresa(this._appCnx).Eliminar(id);
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