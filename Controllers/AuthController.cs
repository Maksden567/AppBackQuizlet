using App.Models;
using AppQuizlet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace AppQuizlet.Controllers
{

	[Route("/[action]")]
	public class AuthController : Controller
	{
		private readonly UserManager<User> _UserManager;
		private readonly SignInManager<User> _SignInManager;
		private readonly RoleManager<IdentityRole> _RoleManager;
		public AuthController(UserManager<User> userManager, SignInManager<User> SignInManager,RoleManager<IdentityRole> RoleManager)
		{
			_UserManager = userManager;
			_SignInManager = SignInManager;
			_RoleManager = RoleManager;
		}

		
		
		[HttpPost]
		public async Task<ActionResult> registration([FromBody]RegisterModel user)
		{
			if(ModelState.IsValid)
			{
				User userModel = new User { Email = user.username,UserName=user.username };
				var result = await _UserManager.CreateAsync(userModel, user.password);
				if (await _RoleManager.RoleExistsAsync("user"))
				{
					Console.WriteLine(true);
				}
				else
				{
					Console.WriteLine(false);
				}
				await _UserManager.AddToRoleAsync(userModel, "user");

				if(result.Succeeded)
				{
					await _SignInManager.SignInAsync(userModel, false);
					
					return Json(result);
					
				}
				else
				{
					return BadRequest(Json("Помилка при регістрації"));
				}
			}
			return BadRequest(Json("Помилка при регістрації"));
		}




		[HttpPost]
		
		 public  async Task<ActionResult> Login([FromBody] LoginModel user)
		{
			if (ModelState.IsValid)
			{
				var result = await _SignInManager.PasswordSignInAsync(user.username, user.password,false,false);
				if (result.Succeeded)
				{
					return Json("Ви авторизовані");
				}
				
			}
			return BadRequest(Json("Помилка при авторизації"));
		}

		






	}
}