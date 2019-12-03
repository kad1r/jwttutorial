using JwtTutorial.Models.Context;
using JwtTutorial.Models.Entities;
using JwtTutorial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace JwtTutorial.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class IdentityController : ControllerBase
	{
		protected readonly BlogcodefirstContext dbContext;
		private readonly IConfiguration conf;
		private readonly ITokenService _tokenService;
		private readonly IUserService _userService;

		public IdentityController(BlogcodefirstContext context, IConfiguration configuration, ITokenService tokenService, IUserService userService)
		{
			dbContext = context;
			conf = configuration;
			_tokenService = tokenService;
			_userService = userService;
		}

		[AllowAnonymous, HttpGet]
		public IActionResult Test()
		{
			return BadRequest(new { message = "Hoop!" });
		}

		[AllowAnonymous, HttpPost("token")]
		public IActionResult GetToken()
		{
			return Ok("Henlo, im a gud boi");

			//var arr = new List<int>
			//{
			//	55,
			//	455
			//};
			//var intarr = System.Array.Empty<int>();

			////GC.GetTotalMemory(true);

			//var bf = new BinaryFormatter();
			//var ms = new MemoryStream();
			//byte[] Array;

			//bf.Serialize(ms, intarr);
			//Array = ms.ToArray();

			////var size = Marshal.SizeOf(arr);
			////GC.GetTotalMemory();
			//return Ok(new { size = Array.Length });
		}

		[AllowAnonymous, HttpPost("authenticate")]
		public async Task<IActionResult> Authenticate(User userInfo)
		{
			var user = await _userService.GetUser(userInfo.Email);

			if (user != null)
			{
				if (user.Password == userInfo.Password)
				{
					var claims = _userService.GenerateClaimsForUser(user);

					user.Token = _tokenService.GenerateJwtToken(claims);
					user.RefreshToken = _tokenService.GenerateJwtRefreshToken();
					await dbContext.SaveChangesAsync();

					return Ok(new { token = user.Token, refreshToken = user.RefreshToken });
				}
				else
				{
					return BadRequest(new { message = "Password is not matched!" });
				}
			}
			else
			{
				return BadRequest(new { message = "User is not exist!" });
			}
		}

		[HttpPost("refreshtoken")]
		public async Task<IActionResult> RefreshToken(string email, string token, string refreshToken)
		{
			var newToken = string.Empty;
			var newRefreshToken = string.Empty;
			var user = await _userService.GetUser(email);

			if (!_tokenService.ValidateToken(token))
			{
				if (user != null && user.RefreshToken == refreshToken)
				{
					var claims = _userService.GenerateClaimsForUser(user);

					newToken = _tokenService.GenerateJwtToken(claims);
					newRefreshToken = _tokenService.GenerateJwtRefreshToken();
				}
			}
			else
			{
				newToken = user.Token;
				newRefreshToken = user.RefreshToken;
			}

			return Ok(new { newToken, newRefreshToken });
		}
	}
}
