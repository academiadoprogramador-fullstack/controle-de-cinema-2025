using ControleDeCinema.Dominio.ModuloAutenticacao;
using System.Security.Claims;

namespace ControleDeCinema.WebApp.Identity;

public class TenantProvider(IHttpContextAccessor contextAccessor) : ITenantProvider
{
    public Guid? UsuarioId
    {
        get
        {
            var claimId = contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);

            if (claimId is null)
                return null;

            return Guid.Parse(claimId.Value);
        }
    }
}