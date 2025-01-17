using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wDualAssistence.Hubs
{
    public class EncoladorHub : Hub<IClock>
    {
        private readonly IHubContext<EncoladorHub> _hubContext;

        public EncoladorHub(IHubContext<EncoladorHub> hubContext)
        {
            _hubContext = hubContext;
        }

    }

    public interface IClock
    {
        Task Validar();
    }
}
