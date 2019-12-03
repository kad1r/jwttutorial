using JwtTutorial.Models.Context;
using JwtTutorial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System;

namespace JwtTutorial.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class AuthAttribute : AuthorizeAttribute, IAuthorizationFilter
	{
		protected readonly BlogcodefirstContext dbContext;
		protected readonly ITokenService _tokenService;

		public AuthAttribute(BlogcodefirstContext _context, ITokenService tokenService)
		{
			dbContext = _context;
			_tokenService = tokenService;
		}

		public void OnAuthorization(AuthorizationFilterContext context)
		{
			var request = context.HttpContext.Request;

			if (request.HttpContext.User != null && request.HttpContext.User.Identity.IsAuthenticated)
			{
				// TODO
				// Do some checks like token, refresh token, extend timeout, roles and etc.

				var authToken = new Microsoft.Extensions.Primitives.StringValues();
				var token = request.Headers.TryGetValue("Authorization", out authToken);

				authToken = authToken.ToString().Replace("Bearer ", "");

				var principal = _tokenService.ValidateToken(authToken);

				var claims = context.HttpContext.User.Claims;

				foreach (var item in claims)
				{
					//retVal.Add(new { item.Type, item.Value });
				}

				context.HttpContext.Response.Cookies.Append("refresh", "me");
			}
			else
			{
				context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Unauthorized);
				return;
			}
		}
	}
}
