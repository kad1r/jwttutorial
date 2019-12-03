using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;

namespace JwtTutorial.Services
{
	public interface ITokenService
	{
		string GenerateJwtToken(IEnumerable<Claim> claims);
		string GenerateJwtRefreshToken();
		List<Claim> GetClaims(string token);
		IIdentity GetIdentity(string token);
		TokenValidationParameters GetTokenValidationParameters();
		bool ValidateToken(string token);
	}
}
