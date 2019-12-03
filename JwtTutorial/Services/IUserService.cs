using JwtTutorial.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JwtTutorial.Services
{
	public interface IUserService
	{
		IEnumerable<Claim> GenerateClaimsForUser(User user);
		Task<User> GetUser(int id);
		Task<User> GetUserWithRoles(int id);
		Task<User> GetUser(string email);
		Task<User> GetUserWithRoles(string email);
	}
}
