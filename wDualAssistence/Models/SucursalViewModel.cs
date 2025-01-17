using Entities.Public;
using System.Collections.Generic;
using System.Security.Claims;

namespace wDualAssistence.Models;

public class SucursalViewModel : ViewModelBase
{
    public Entities.Public.Sucursal eSucursal { get; set; }
    public List<Ciudad> listaCiudades { get; set; }
    public SucursalViewModel() : base()
    {
    }

    public SucursalViewModel(ClaimsPrincipal user) : base(user)
    {
    }
}
