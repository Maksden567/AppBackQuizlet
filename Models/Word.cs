using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
	public class WordModel
	{
		[Key]
		public int WordId { get; set; }
		public string NameEnglish { get; set; }
		public string TranslateName { get; set; }
		public bool? isKnowen { get; set; }
		public LibraryEnglish? LibraryEnglish { get; set; }
		public int LibraryEnglishId { get; set; }

		public string? imageUrl { get; set; }

		public string? musicUrl { get; set; }

	}
}
