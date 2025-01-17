using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Navigations;
using wDualAssistence.Models;

namespace wDualAssistence.Controllers
{
    public class ConfiguracionController : MainController
    {
        public IActionResult Parametros()
        {
            ConfiguracionViewModel vConfiguracionViewModel = new ConfiguracionViewModel(this.User);
            if (!this._RevisarAcceso(vConfiguracionViewModel.idUsuario, vConfiguracionViewModel.tipo, Url.ActionContext.HttpContext.Request.Path))
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }

            vConfiguracionViewModel.listaCategoria_Tarea = new Business.Public.Categoria_Tarea(this._appCnx).GetLista();

            //Cargo Items para el Menu
            vConfiguracionViewModel.menuItems.Add(new
            {
                id = "1",
                text = "Editar",
                iconCss = "fa fa-pencil"
            });

            vConfiguracionViewModel.menuItems.Add(new
            {
                id = "2",
                text = "Eliminar",
                iconCss = "fa fa-remove"
            });

            ViewBag.headerCategoria = new TabHeader { Text = "Categorias", IconCss = "fa fa-sitemap" };
            ViewBag.headerTarea = new TabHeader { Text = "Tareas", IconCss = "fa fa-tasks" };
            ViewBag.headerFeriado = new TabHeader { Text = "Dias Feriados", IconCss = "fa fa-calendar-times-o" };
            ViewBag.headerAreasCargos = new TabHeader { Text = "Áreas y Cargos", IconCss = "fa fa-group" };

            return View(vConfiguracionViewModel);
        }
    }
}