using JwtTutorial.Models.Context;
using JwtTutorial.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JwtTutorial.Services
{
	public class UserService : IUserService
	{
		private readonly BlogcodefirstContext _context;

		public UserService(BlogcodefirstContext context)
		{
			_context = context;
		}

		public IEnumerable<Claim> GenerateClaimsForUser(User user)
		{
			var claims = new List<Claim>();

			if (user != null && user.Id > 0)
			{
				claims.Add(new Claim("userid", user.Id.ToString()));
				claims.Add(new Claim("username", user.UserName.ToString()));
				claims.Add(new Claim("email", user.Email.ToString()));

				foreach (var role in user.UserRoles)
				{
					claims.Add(new Claim(ClaimTypes.Role, role.Role.Heading));
				}
			}

			return claims;
		}

		public async Task<User> GetUser(int id)
		{
			return await _context.Users.FindAsync(id);
		}

		public async Task<User> GetUserWithRoles(int id)
		{
			return await _context.Users
				.Include(x => x.UserRoles)
				.ThenInclude(x => x.Role)
				.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<User> GetUser(string email)
		{
			return await _context.Users.SingleOrDefaultAsync(x => x.Email == email);
		}

		public async Task<User> GetUserWithRoles(string email)
		{
			return await _context.Users
				.Include(x => x.UserRoles)
				.ThenInclude(x => x.Role)
				.FirstOrDefaultAsync(x => x.Email == email);
		}
	}
}
