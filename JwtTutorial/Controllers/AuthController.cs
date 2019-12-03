using JwtTutorial.Attributes;
using JwtTutorial.Models.Context;
using JwtTutorial.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace JwtTutorial.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[ServiceFilter(typeof(AuthAttribute))]
	public class AuthController : ControllerBase
	{
		private readonly IConfiguration conf;
		private readonly ITokenService _tokenService;
		protected readonly BlogcodefirstContext dbContext;

		public AuthController(IConfiguration configuration, ITokenService tokenService, BlogcodefirstContext context)
		{
			conf = configuration;
			_tokenService = tokenService;
			dbContext = context;
		}

		[HttpPost("token")]
		public IActionResult GetTokenParameters()
		{
			var retVal = new List<dynamic>();
			var claims = Request.HttpContext.User.Claims;

			foreach (var item in claims)
			{
				retVal.Add(new { item.Type, item.Value });
			}

			var authToken = new Microsoft.Extensions.Primitives.StringValues();
			var token = Request.Headers.TryGetValue("Authorization", out authToken);

			authToken = authToken.ToString().Replace("Bearer ", "");

			var principal = _tokenService.ValidateToken(authToken);

			return Ok(new { retVal, principal });
		}
	}
}
