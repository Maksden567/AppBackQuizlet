using App;
using App.Models;
using Microsoft.AspNet.Identity;
using App.Models;
using AppQuizlet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Collections.Specialized;
using Microsoft.IdentityModel.Tokens;

namespace AppQuizlet.Controllers
{

	public class WordController : Controller
	{
		private readonly ApplicationContext _context;
		

		public WordController (ApplicationContext context)
		{
			_context = context;
			
		}



		[Authorize]
		[HttpPost("liabries/{id}/createWord")]

		
		public IActionResult createWord([FromBody] WordModel wordBody, [FromRoute] int id)
		{
			Console.WriteLine(id);
			if (wordBody.NameEnglish == null)
			{
				return Json(BadRequest("Вкажіть слово слова"));
			}
			if (wordBody.TranslateName == null)
			{
				return Json(BadRequest("Вкажіть переклад слова"));
			}

			
			var word = new WordModel {TranslateName= wordBody.TranslateName,NameEnglish= wordBody.NameEnglish,isKnowen=false,LibraryEnglishId= id,imageUrl=wordBody.imageUrl, musicUrl=wordBody.musicUrl};
			
			_context.words.Add(word);

			_context.SaveChanges();

			return Json(word);

		}

		[Authorize]
		[HttpGet("liabries/{id}/getWords")]
		public IActionResult getWords([FromRoute] int id)
		{


			var words = _context.words.Where(word => word.LibraryEnglishId == id);

			if (words.IsNullOrEmpty())
			{
				return Json(NotFound("У даному модулі немає слів"));
			}

			return Json(words);


		}

		[Authorize]
		[HttpPut("liabries/{id}/editWord/{wordId}")]
		public IActionResult editWords([FromRoute] int id, [FromRoute] int wordId, [FromBody] WordModel word)
		{


			var words = _context.words.Where(word => word.LibraryEnglishId == id);

			if (words.IsNullOrEmpty())
			{
				return Json(NotFound("Даний модуль не містить слів"));
			}

			var wordExist=words.FirstOrDefault(word => word.WordId == wordId);

			if (wordExist == null)
			{
				return Json(NotFound("Немає слова в даному модулі"));
			}

			wordExist.NameEnglish = word.NameEnglish;
			wordExist.isKnowen = word.isKnowen;
			wordExist.TranslateName= word.TranslateName;

			_context.words.Update(wordExist);

			_context.SaveChanges();

			return Json("Все успішно змінено");
			
			

		


		}

		[Authorize]
		[HttpDelete("liabries/{id}/deleteWord/{wordId}")]
		public IActionResult deleteWord([FromRoute] int id, [FromRoute] int wordId)
		{


			var words = _context.words.Where(word => word.LibraryEnglishId == id);

			if (words.IsNullOrEmpty())
			{
				return Json(NotFound("Даний модуль не містить слів"));
			}

			var wordExist = words.FirstOrDefault(word => word.WordId == wordId);

			if (wordExist == null)
			{
				return Json(NotFound("Немає слова в даному модулі"));
			}

			_context.words.Remove(wordExist);

			_context.SaveChanges();

			return Json("Все успішно змінено");






		}



	}
}
