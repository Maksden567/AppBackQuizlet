using App.Models;
using Microsoft.AspNetCore.Identity;

namespace AppQuizlet.Services
{
	public class TelegramService
	{
		public SignInManager<User> _SignInManager;

		public TelegramService( SignInManager<User> SignInManager )
		{

			_SignInManager = SignInManager;
			
		}
	}
}
