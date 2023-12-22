using System.ComponentModel.DataAnnotations;

namespace App.Models
{
	public class LoginModel
	{
		[Required]
		public string username { get; set; }
		[Required]
		public string password { get; set; }

	}
}
