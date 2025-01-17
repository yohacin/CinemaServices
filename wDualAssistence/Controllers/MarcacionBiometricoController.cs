using Entities.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using wDualAssistence.Configs;
using wDualAssistence.Helpers;
using wDualAssistence.Models;
using wDualAssistence.Utility;
using BPU = Business.Public;
using EPU = Entities.Public;

namespace wDualAssistence.Controllers;

[Authorize]

public class MarcacionBiometricoController : MainController
{
    public IActionResult Listado()
    {
        MarcacionBiometricoViewModel vMarcacionBiometricoViewModel = new MarcacionBiometricoViewModel(this.User);

        //Cargo Items para el Menu
        vMarcacionBiometricoViewModel.menuItems.Add(new
        {
            id = "3",
            text = "Ver detalle",
            iconCss = "fa fa-eye"
        });

        return View(vMarcacionBiometricoViewModel);
    }

    /// <summary>
    /// Odata para listado
    /// </summary>
    /// <param name="queryOptions"></param>
    /// <returns></returns>
    [EnableQuery]
    [HttpGet]
    public ActionResult<IEnumerable<EPU.Marcacion_Biometrico>> Get(ODataQueryOptions<EPU.Marcacion_Biometrico> queryOptions)
    {
        try
        {
            var lista = new BPU.Marcacion_Biometrico(this._appCnx).GetLista();
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
        MarcacionBiometricoViewModel vMarcacionBiometricoViewModel = new MarcacionBiometricoViewModel(this.User);
        vMarcacionBiometricoViewModel.eMarcacionBiometrico = new Marcacion_Biometrico();
        vMarcacionBiometricoViewModel.listaFormatos = new BPU.Formato_Marcacion_Biometrico(this._appCnx).GetLista();
        vMarcacionBiometricoViewModel.listaEmpresas = new BPU.Empresa(this._appCnx).GetLista();
        return View(vMarcacionBiometricoViewModel);
    }

    /// <summary>
    ///  Para la parte de carga de datos en base
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(MarcacionBiometricoViewModel viewModel)
    {
        try
        {
            // Validaciones 
            if (viewModel.archivo == null || viewModel.archivo.Length == 0) throw new Exception("El archivo no puede estar vacío.");
            // Validar la extensión del archivo
            var extension = Path.GetExtension(viewModel.archivo.FileName).ToLower();
            if (extension != ".xls" && extension != ".xlsx") throw new Exception("Solo se permiten archivos en formato Excel (.xls, .xlsx).");
            
            EPU.Formato_Marcacion_Biometrico formato = new BPU.Formato_Marcacion_Biometrico(this._appCnx).GetEntity(viewModel.eMarcacionBiometrico.id_formato_marcacion_biometrico);
            if (formato == null) throw new Exception("Debe seleccionar un formato.");
            if (viewModel.eMarcacionBiometrico.id_empresa == 0) throw new Exception("Debe seleccionar una empresa.");

            // Procesamiento del excel para obtener las marcaciones registradas por el biometrico
            List<Detalle_Marcacion_Biometrico> marcacionesBiometrico = await SYNCFUSION_EXCEL_UTILS.ObtenerMarcacionesAsync(viewModel.archivo, formato, viewModel.eMarcacionBiometrico.id_empresa);
            if (!marcacionesBiometrico.Any()) throw new Exception("No se encontraron registros válidos.");

            // Actualizar las marcaciones utilizando la lista marcacionesBiometrico
            MarcacionDesdeBiometricoConfig marcacionBioConfig = Startup.marcacionDesdeBiometricoConfig;

            // Guardar los datos de la marcacion
            viewModel.eMarcacionBiometrico.id_usuario = Convert.ToInt64(this.User.GetClaimValue("idUsuario"));
            viewModel.eMarcacionBiometrico.fecha_registro = DateTime.Now;
            viewModel.eMarcacionBiometrico.nombre_archivo = viewModel.archivo.FileName;
            viewModel.eMarcacionBiometrico.ruta_archivo= "";
            viewModel.eMarcacionBiometrico.listaDetalleMarcacion = marcacionesBiometrico;
            new BPU.Marcacion_Biometrico(this._appCnx).GuardarMarcaciones(viewModel.eMarcacionBiometrico, marcacionBioConfig.ToleranciaFija);

            // Actualizar las marcaciones reales
            //new BPU.Marcacion(this._appCnx).RegistrarMarcacionesBiometrico(marcacionesBiometrico, marcacionBioConfig.ToleranciaFija);

            return Ok(new { transaccion = true });
        }
        catch (Exception ex)
        {
            return Ok(new { transaccion = false, mensaje = ex.Message });
            throw;
        }
    }

    /// <summary>
    ///  Para la parte de carga de datos en base
    /// </summary>
    /// <returns></returns>
    [HttpPost("ProcesarExcel")]
    public async Task<IActionResult> ProcesarExcel(MarcacionBiometricoViewModel viewModel)
    {
        try
        {
            // Validaciones 
            if (viewModel.archivo == null || viewModel.archivo.Length == 0) throw new Exception("El archivo no puede estar vacío.");
            var extension = Path.GetExtension(viewModel.archivo.FileName).ToLower();
            if (extension != ".xls" && extension != ".xlsx") throw new Exception("Solo se permiten archivos en formato Excel (.xls, .xlsx).");
            EPU.Formato_Marcacion_Biometrico formato = new BPU.Formato_Marcacion_Biometrico(this._appCnx).GetEntity(viewModel.eMarcacionBiometrico.id_formato_marcacion_biometrico);
            if (formato == null) throw new Exception("Debe seleccionar un formato.");
            if (viewModel.eMarcacionBiometrico.id_empresa == 0) throw new Exception("Debe seleccionar una empresa.");

            // Procesamiento del excel para obtener las marcaciones registradas por el biometrico
            List<Detalle_Marcacion_Biometrico> marcacionesBiometrico = await SYNCFUSION_EXCEL_UTILS.ObtenerMarcacionesAsync(viewModel.archivo, formato, viewModel.eMarcacionBiometrico.id_empresa);
            if (!marcacionesBiometrico.Any()) throw new Exception("No se encontraron registros válidos.");

            for (int i = 0; i < marcacionesBiometrico.Count; i++)
            {
                marcacionesBiometrico[i].id = i + 1;
            }

            return Ok(new { transaccion = true, contenido = marcacionesBiometrico });
        }
        catch (Exception ex)
        {
            return Ok(new { transaccion = false, mensaje = ex.Message });
            throw;
        }
    }

    [HttpPost]
    public IActionResult VerDetalle([FromForm] long id2)
    {
        MarcacionBiometricoViewModel vMarcacionBiometricoViewModel = new MarcacionBiometricoViewModel(this.User);
        vMarcacionBiometricoViewModel.eMarcacionBiometrico = new BPU.Marcacion_Biometrico(this._appCnx).GetEntity(id2);
        return View(vMarcacionBiometricoViewModel);
    }
}