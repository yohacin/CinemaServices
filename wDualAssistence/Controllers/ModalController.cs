using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using wDualAssistence.Models;

namespace wDualAssistence.Controllers
{
    [Authorize]
    public class ModalController : MainController
    {

        [HttpGet("Modal/MostrarModalEliminar")]
        public ActionResult MostrarModalEliminar()
        {
            ModalBusViewModel modalResponsablesNodo = new ModalBusViewModel()
            {
                titulo = "Confirmación",
                texto = "Está seguro de Eliminar ?",
                botonConfirmar = true,
                botonCancelar = true,
                textoConfirmar = "Eliminar",
                textoCancelar = "Cancelar",
                tipo = ModalBusViewModel.TipoModal.Eliminar,
                contenidoViewModel = null
            };

            return PartialView("_ModalBus", modalResponsablesNodo);
        }

        [HttpGet("Modal/MostrarModalConfirmar/{mensaje}")]
        public ActionResult MostrarModalConfirmar(string mensaje)
        {
            if (String.IsNullOrEmpty(mensaje))
            {
                mensaje = "Está seguro de realizar la acción ?";
            }
            else
            {
                mensaje = mensaje.Replace("_", " ") + "?";
            }

            ModalBusViewModel modalResponsablesNodo = new ModalBusViewModel()
            {
                titulo = "Confirmación",
                texto = mensaje,
                botonConfirmar = true,
                botonCancelar = true,
                textoConfirmar = "Aceptar",
                textoCancelar = "Cancelar",
                tipo = ModalBusViewModel.TipoModal.Eliminar,
                contenidoViewModel = null
            };

            return PartialView("_ModalBus", modalResponsablesNodo);
        }
    }
}
