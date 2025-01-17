using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Collections.Generic;
using System;
using wDualAssistence.Models;
using EPU = Entities.Public;
using BPU = Business.Public;
using Newtonsoft.Json;
using Entities.Public;
using System.Linq;

namespace wDualAssistence.Controllers;

[Authorize]
public class JornadaController : MainController
{
    public IActionResult Listado()
    {
        JornadaViewModel vJornadaViewModel = new JornadaViewModel(this.User);
        vJornadaViewModel.menuItems.Add(new
        {
            id = "1",
            text = "Editar",
            iconCss = "fa fa-pencil"
        });
        vJornadaViewModel.menuItems.Add(new
        {
            id = "2",
            text = "Eliminar",
            iconCss = "fa fa-remove"
        });

        return View(vJornadaViewModel);
    }

    /// <summary>
    /// Odata para listado
    /// </summary>
    /// <param name="queryOptions"></param>
    /// <returns></returns>
    [EnableQuery]
    [HttpGet]
    public ActionResult<IEnumerable<EPU.Jornada>> Get(ODataQueryOptions<EPU.Jornada> queryOptions)
    {
        try
        {
            var lista = new BPU.Jornada(this._appCnx).GetLista();
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
            JornadaViewModel vJornadaViewModel = new JornadaViewModel(this.User);

            if (id != 0)
            {
                vJornadaViewModel.eJornada = new BPU.Jornada(this._appCnx).GetEntity(id);
                if (vJornadaViewModel.eJornada == null) vJornadaViewModel.eJornada = new EPU.Jornada();
            }
            else
            {
                vJornadaViewModel.eJornada = new EPU.Jornada();
            }

            return View(vJornadaViewModel);
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
    public IActionResult Post(EPU.Jornada eJornada)
    {
        try
        {
            if (eJornada == null) throw new Exception("Los Datos no fueron recibidos");
            if (eJornada.nombre == null) throw new Exception("El campo nombre es requerido !");

            var listaDetalleJornada = JsonConvert.DeserializeObject<List<Detalle_Jornada>>(eJornada.detalleJSON);
            var listaTurnosPlantilla = JsonConvert.DeserializeObject<List<Turno_Plantilla>>(eJornada.turnoPlantillaJSON);

            if (!listaDetalleJornada.Any()) throw new Exception("La jornada no tiene ningún turno registrado.");
            
            eJornada.listaDetalleJornada = listaDetalleJornada;
            eJornada.listaTurnoPlantilla = listaTurnosPlantilla;

            if (eJornada.id != 0)
            {
                new BPU.Jornada(this._appCnx).Modificar(eJornada);
            }
            else
            {
                new BPU.Jornada(this._appCnx).Guardar(eJornada);
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
            new BPU.Jornada(this._appCnx).Eliminar(id);
            return Ok(new { transaccion = true });
        }
        catch (Exception ex)
        {
            this._log.Error(ex);
            return Ok(new { transaccion = false, mensaje = ex.Message });
        }
    }

}
