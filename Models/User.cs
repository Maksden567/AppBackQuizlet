using Microsoft.AspNetCore.Identity;

namespace App.Models
{
	public class User:IdentityUser
	{
		public List<LibraryEnglish> Modules { get; set; }
	}
}
