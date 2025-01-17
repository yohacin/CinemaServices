using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EPU = Entities.Public;
using BPU = Business.Public;
using Newtonsoft.Json;
using wDualAssistence.Helpers;

namespace wDualAssistence.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MovilController : ApiMainController
    {
        [HttpGet("{id_empleado}")]
        [BasicAuth]
        public string listarParametros(long id_empleado)
        {
            List<EPU.ConfigParametro> lista = new List<EPU.ConfigParametro>();
            try
            {
                BPU.ConfigParametro bConfig = new BPU.ConfigParametro(this._appCnx);

                lista = bConfig.ListarByEmpleado(id_empleado);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
            }

            return JsonConvert.SerializeObject(lista);
        }

        [HttpGet()]
        public string version()
        {
            try
            {
                BPU.ConfigParametro bConfig = new BPU.ConfigParametro(this._appCnx);
                EPU.ConfigParametro eConfigParametro = null;

                eConfigParametro = bConfig.BuscarParametroById(1L);

                return JsonConvert.SerializeObject(eConfigParametro);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }
    }
}