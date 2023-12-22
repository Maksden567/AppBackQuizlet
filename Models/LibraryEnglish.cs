using Microsoft.AspNetCore.Identity;


namespace App.Models
{
	public class LibraryEnglish
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public User? User { get; set; }
		public string? UserId { get; set; }
		public List<WordModel>? words { get ;set;}

		
	}
}
