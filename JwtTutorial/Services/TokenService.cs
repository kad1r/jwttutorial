using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace JwtTutorial.Services
{
	public class TokenService : ITokenService
	{
		private readonly IConfiguration conf;

		public TokenService(IConfiguration configuration)
		{
			conf = configuration;
		}

		public string GenerateJwtRefreshToken()
		{
			return Guid.NewGuid().ToString().Replace("-", "");
		}

		public string GenerateJwtToken(IEnumerable<Claim> claims)
		{
			var secret = conf.GetSection("AppSettings:JwtSecret").Value;
			var tokenlifetime = conf.GetSection("AppSettings:TokenLifeTime").Value;
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.UTF8.GetBytes(secret);
			var tokenDescriptor = new SecurityTokenDescriptor();
			var subject = new ClaimsIdentity();

			subject.AddClaims(claims);
			tokenDescriptor.Subject = subject;
			tokenDescriptor.Expires = DateTime.UtcNow.Add(TimeSpan.Parse(tokenlifetime));
			tokenDescriptor.SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

			var token = tokenHandler.CreateToken(tokenDescriptor);

			return tokenHandler.WriteToken(token);
		}

		public List<Claim> GetClaims(string token)
		{
			SecurityToken securityToken;
			var tokenHandler = new JwtSecurityTokenHandler();
			var validationParameters = GetTokenValidationParameters();

			if (ValidateToken(token))
			{
				var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
				return principal.Claims.ToList();
			}
			else
			{
				return null;
			}
		}

		public IIdentity GetIdentity(string token)
		{
			SecurityToken securityToken;
			var tokenHandler = new JwtSecurityTokenHandler();
			var validationParameters = GetTokenValidationParameters();

			if (ValidateToken(token))
			{
				var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
				return principal.Identity;
			}
			else
			{
				return null;
			}
		}

		public TokenValidationParameters GetTokenValidationParameters()
		{
			var secret = conf.GetSection("AppSettings:JwtSecret").Value;
			var key = Encoding.UTF8.GetBytes(secret);
			var options = new TokenValidationParameters()
			{
				ValidateIssuerSigningKey = true,
				ValidateLifetime = true,
				RequireExpirationTime = true,
				ValidateIssuer = false,
				ValidateAudience = false,
				IssuerSigningKey = new SymmetricSecurityKey(key)
			};

			return options;
		}

		public bool ValidateToken(string token)
		{
			var isValid = false;

			try
			{
				SecurityToken securityToken;
				var tokenHandler = new JwtSecurityTokenHandler();
				var validationParameters = GetTokenValidationParameters();
				tokenHandler.ValidateToken(token, validationParameters, out securityToken);
				var jwtSecurityToken = securityToken as JwtSecurityToken;

				if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
				{
					isValid = false;
					//throw new SecurityTokenException("Invalid token");
				}
				else
				{
					isValid = true;
				}
			}
			catch (SecurityTokenValidationException ex)
			{
				var msg = ex.Message;
				// log message
			}
			catch (Exception ex)
			{
				var msg = ex.Message;
				// log message
			}

			return isValid;
		}
	}
}
