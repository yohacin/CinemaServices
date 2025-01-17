using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;

namespace wDualAssistence.Models;

public class FormatoMarcacionBiometricoViewModel : ViewModelBase
{
    public Entities.Public.Formato_Marcacion_Biometrico eFormato { get; set; }
    public List<Entities.Public.Formato_Marcacion_Biometrico> listaFormatos { get; set; }
    public long id_formato_seleccionado { get; set; }
    public IFormFile archivo { get; set; }

    public FormatoMarcacionBiometricoViewModel() : base()
    {
    }

    public FormatoMarcacionBiometricoViewModel(ClaimsPrincipal user) : base(user)
    {
    }
}
