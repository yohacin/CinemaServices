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
    public class TurnoController : ApiMainController
    {
        [HttpGet("{id_empleado}")]
        [BasicAuth]
        public string listarTurnosUsuario(long id_empleado)
        {
            List<EPU.Turno> lista = new List<EPU.Turno>();
            try
            {
                BPU.Turno bTurno = new BPU.Turno(this._appCnx);
                lista = bTurno.listarTurnosEmpleado(id_empleado);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                //throw ex;
            }
            return JsonConvert.SerializeObject(lista);
        }
    }
}