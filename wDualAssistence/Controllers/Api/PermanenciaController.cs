using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EPU = Entities.Public;
using BPU = Business.Public;
using wDualAssistence.Helpers;

namespace wDualAssistence.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PermanenciaController : ApiMainController
    {
        [HttpGet("{fecha_inicio}/{fecha_fin}")]
        [BasicAuth]
        public List<EPU.Permanencia> listarPermanenciaPorFecha(string fecha_inicio, string fecha_fin)
        {
            List<EPU.Permanencia> lista = new List<EPU.Permanencia>();
            try
            {
                BPU.Permanencia bMarcacion = new BPU.Permanencia(this._appCnx);
                lista = bMarcacion.ListarPermanenciaPorFecha(Convert.ToDateTime(fecha_inicio), Convert.ToDateTime(fecha_fin));
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
            }

            return lista;
        }
    }
}