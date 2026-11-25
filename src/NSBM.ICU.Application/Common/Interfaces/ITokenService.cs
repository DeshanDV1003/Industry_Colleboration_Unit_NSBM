using NSBM.ICU.Application.Common.Models;

namespace NSBM.ICU.Application.Common.Interfaces;

public interface ITokenService
{
    AccessToken CreateToken(AuthenticatedUser user);
}
