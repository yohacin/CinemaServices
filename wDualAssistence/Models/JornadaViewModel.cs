using System.Security.Claims;

namespace wDualAssistence.Models;

public class JornadaViewModel : ViewModelBase
{
    public Entities.Public.Jornada eJornada { get; set; }

    public JornadaViewModel() : base()
    {
    }

    public JornadaViewModel(ClaimsPrincipal user) : base(user)
    {
    }
}
