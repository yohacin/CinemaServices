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

public class FormatoMarcacionBiometricoController : MainController
{
    public IActionResult Listado()
    {
        FormatoMarcacionBiometricoViewModel vFormatoViewModel = new FormatoMarcacionBiometricoViewModel(this.User);
        vFormatoViewModel.menuItems.Add(new
        {
            id = "1",
            text = "Editar",
            iconCss = "fa fa-pencil"
        });
        vFormatoViewModel.menuItems.Add(new
        {
            id = "2",
            text = "Eliminar",
            iconCss = "fa fa-remove"
        });

        return View(vFormatoViewModel);
    }


    /// <summary>
    /// Odata para listado
    /// </summary>
    /// <param name="queryOptions"></param>
    /// <returns></returns>
    [EnableQuery]
    [HttpGet]
    public ActionResult<IEnumerable<EPU.Formato_Marcacion_Biometrico>> Get(ODataQueryOptions<EPU.Formato_Marcacion_Biometrico> queryOptions)
    {
        try
        {
            var lista = new BPU.Formato_Marcacion_Biometrico(this._appCnx).GetLista();
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
            FormatoMarcacionBiometricoViewModel vFormatoViewModel = new FormatoMarcacionBiometricoViewModel(this.User);

            if (id != 0)
            {
                vFormatoViewModel.eFormato = new BPU.Formato_Marcacion_Biometrico(this._appCnx).GetEntity(id);
                if (vFormatoViewModel.eFormato == null) vFormatoViewModel.eFormato = new EPU.Formato_Marcacion_Biometrico();
            }
            else
            {
                vFormatoViewModel.eFormato = new EPU.Formato_Marcacion_Biometrico();
            }

            return View(vFormatoViewModel);
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
    public IActionResult Post(EPU.Formato_Marcacion_Biometrico eFormato)
    {
        try
        {
            if (eFormato == null) throw new Exception("Los Datos no fueron recibidos");
            if (eFormato.estan_misma_fila && string.IsNullOrEmpty(eFormato.col_fecha_hora_salida)) throw new Exception("La Fecha y Hora (Salida) es un campo requerido.");

            if (eFormato.id != 0)
            {
                new BPU.Formato_Marcacion_Biometrico(this._appCnx).Modificar(eFormato);
            }
            else
            {
                new BPU.Formato_Marcacion_Biometrico(this._appCnx).Guardar(eFormato);
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
            new BPU.Formato_Marcacion_Biometrico(this._appCnx).Eliminar(id);
            return Ok(new { transaccion = true });
        }
        catch (Exception ex)
        {
            this._log.Error(ex);
            return Ok(new { transaccion = false, mensaje = ex.Message });
        }
    }

}
