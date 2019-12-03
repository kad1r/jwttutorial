using JwtTutorial.Models.Context;
using JwtTutorial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace JwtTutorial.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class AuthAttribute : AuthorizeAttribute, IAuthorizationFilter
	{
		protected readonly BlogcodefirstContext dbContext;
		protected readonly ITokenService _tokenService;
		protected readonly IUserService _userService;

		public AuthAttribute(BlogcodefirstContext _context, ITokenService tokenService, IUserService userService)
		{
			dbContext = _context;
			_tokenService = tokenService;
			_userService = userService;
		}

		public void OnAuthorization(AuthorizationFilterContext context)
		{
			var request = context.HttpContext.Request;
			var authToken = new Microsoft.Extensions.Primitives.StringValues();
			var token = request.Headers.TryGetValue("Authorization", out authToken);

			authToken = authToken.ToString().Replace("Bearer ", "");

			if (_tokenService.ValidateToken(authToken))
			{
				var claims = _tokenService.GetClaims(authToken);
				var identity = _tokenService.GetIdentity(authToken);
				// TODO
				// You can do some extra checks here
				return;
			}
			else
			{
				context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Unauthorized);
				return;
			}
		}
	}
}
