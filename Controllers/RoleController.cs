using App.Models;
using AppQuizlet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Principal;

namespace AppQuizlet.Controllers
{
	public class RoleController : Controller
	{
		private readonly UserManager<User> _UserManager;
		private readonly RoleManager<IdentityRole> _RoleManager;
		public RoleController(UserManager<User> userManager, RoleManager<IdentityRole> RoleManager)
		{
			_UserManager = userManager;
			_RoleManager = RoleManager;
		}


		public async Task<ActionResult> create([FromQuery] string name)
		{

			if(name !=null) {

				await _RoleManager.CreateAsync(new IdentityRole { Name = name });
				return Json("dsjnv");
			}

			return BadRequest("sd klcdskc");

			

		}

	

		



		







	}
}