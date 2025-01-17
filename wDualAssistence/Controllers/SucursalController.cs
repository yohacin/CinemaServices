using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Collections.Generic;
using System;
using wDualAssistence.Models;
using EPU = Entities.Public;
using BPU = Business.Public;
using Microsoft.AspNetCore.Authorization;

namespace wDualAssistence.Controllers;

[Authorize]

public class SucursalController : MainController
{
    public IActionResult Listado()
    {
        SucursalViewModel vSucursalViewModel = new SucursalViewModel(this.User);

        //Cargo Items para el Menu
        vSucursalViewModel.menuItems.Add(new
        {
            id = "1",
            text = "Editar",
            iconCss = "fa fa-pencil"
        });
        vSucursalViewModel.menuItems.Add(new
        {
            id = "2",
            text = "Eliminar",
            iconCss = "fa fa-remove"
        });

        return View(vSucursalViewModel);
    }

    /// <summary>
    /// Odata para listado
    /// </summary>
    /// <param name="queryOptions"></param>
    /// <returns></returns>
    [EnableQuery]
    [HttpGet]
    public ActionResult<IEnumerable<EPU.Sucursal>> Get(ODataQueryOptions<EPU.Sucursal> queryOptions)
    {
        try
        {
            var lista = new BPU.Sucursal(this._appCnx).GetLista();
            return Ok(lista);
        }
        catch (Exception ex)
        {
            this._log.Error(ex);
            throw;
        }
    }

    [HttpPost]
    public IActionResult Crear([FromForm] long id)
    {
        try
        {
            SucursalViewModel vSucursalViewModel = new SucursalViewModel(this.User);
            vSucursalViewModel.listaCiudades = new BPU.Ciudad(this._appCnx).GetLista();

            if (id != 0)
            {
                vSucursalViewModel.eSucursal = new BPU.Sucursal(this._appCnx).GetEntity(id);
            }
            else
            {
                vSucursalViewModel.eSucursal = new EPU.Sucursal();
            }

            return View(vSucursalViewModel);
        }
        catch (Exception ex)
        {
            this._log.Error(ex);
            throw;
        }
    }

    /// <summary>
    ///  Para la parte de carga de datos en base
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Post(EPU.Sucursal eSucursal)
    {
        try
        {
            if (eSucursal == null) throw new Exception("Los Datos no fueron recibidos");
            if (eSucursal.id_ciudad == 0) throw new Exception("Debe seleccionar una ciudad");

            if (eSucursal.id != 0)
            {
                new BPU.Sucursal(this._appCnx).Modificar(eSucursal);
            }
            else
            {
                new BPU.Sucursal(this._appCnx).Guardar(eSucursal);
            }
        }
        catch (Exception ex)
        {
            this._log.Error(ex);
            return Ok(new { transaccion = false, mensaje = ex.Message });
            throw;
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
            new BPU.Sucursal(this._appCnx).Eliminar(id);
            return Ok(new { transaccion = true });
        }
        catch (Exception ex)
        {
            this._log.Error(ex);
            return Ok(new { transaccion = false, mensaje = ex.Message });
        }
    }
}
