using Entities.Public;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;

namespace wDualAssistence.Models;

public class MarcacionBiometricoViewModel : ViewModelBase
{
    public Marcacion_Biometrico eMarcacionBiometrico { get; set; }
    public List<Formato_Marcacion_Biometrico> listaFormatos { get; set; }
    public List<Empresa> listaEmpresas { get; set; }
    public IFormFile archivo { get; set; }

    public MarcacionBiometricoViewModel() : base()
    {
    }

    public MarcacionBiometricoViewModel(ClaimsPrincipal User) : base(User)
    {
    }
}

