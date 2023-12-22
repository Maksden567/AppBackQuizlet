using App.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace App
{
	public class ApplicationContext:IdentityDbContext<User>
	{
		public DbSet<LibraryEnglish> libraries { get; set; } = null!;
		public DbSet<WordModel> words { get; set; } = null!;
		public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
		


	}
}
