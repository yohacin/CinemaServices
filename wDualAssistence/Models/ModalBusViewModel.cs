using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wDualAssistence.Models
{
    public class ModalBusViewModel
    {
        public enum TipoModal
        {
            Simple,
            Busqueda,
            Eliminar
        }

        public string titulo { get; set; }
        public TipoModal tipo { get; set; }
        public string texto { get; set; }
        public bool botonConfirmar { get; set; }
        public string textoConfirmar { get; set; }
        public bool botonCancelar { get; set; }
        public string textoCancelar { get; set; }
        public ContenidoModalViewModel contenidoViewModel { get; set; }

        public ModalBusViewModel()
        {
            this.textoConfirmar = "Aceptar";
            this.textoCancelar = "Cancelar";
        }

    }
}
