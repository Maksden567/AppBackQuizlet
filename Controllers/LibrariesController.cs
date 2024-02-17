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
using Firebase.Auth;
using Firebase.Storage;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Diagnostics.Metrics;
using Amazon.Translate;
using Amazon;
using Amazon.Translate.Model.Internal.MarshallTransformations;
using Amazon.Translate.Model;
using Telegram.Bot;
using Telegram.Bot.Types;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Threading;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Amazon.Runtime;
using Amazon.Polly;
using Amazon.Polly.Model;
using static System.Net.Mime.MediaTypeNames;
using Azure;

namespace AppQuizlet.Controllers
{
	[Route("/[action]")]
	public class LibrariesController : Controller
	{
		private readonly ApplicationContext _context;

		public static string apiKey = "AIzaSyCW48XZoX416QuvSgZasdq0lH23ha5bJ2o";
		public static string Bucket = "dashka-71be6.appspot.com";
		public static string authEmail = "babadid346@gmail.com";
		public static string authPassword = "Maksden567";

		private readonly IWebHostEnvironment _env;

		public LibrariesController(ApplicationContext context, IWebHostEnvironment hostingEnvironment)
		{
			_context = context;
			_env = hostingEnvironment;

		}

		public static byte[] ConvertIFormFileToBase64(IFormFile file)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				file.CopyTo(memoryStream);
				byte[] fileBytes = memoryStream.ToArray();
			

				return fileBytes;
			}
		}

		public static byte[] ConvertAudioToBase64(SynthesizeSpeechResponse response)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				response.AudioStream.CopyTo(memoryStream);
				byte[] fileBytes = memoryStream.ToArray();


				return fileBytes;
			}
		}


		[HttpPost]
		public async Task<IActionResult> uploadPhoto(IFormFile file)
		{

			var imageIn64 = ConvertIFormFileToBase64(file);

			
			var memoryStream = new MemoryStream(imageIn64);
		



			try
			{
				var authProvider = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
				var a = await authProvider.SignInWithEmailAndPasswordAsync(authEmail, authPassword);

				var cancelation = new CancellationTokenSource();

				var upload = new FirebaseStorage(

					Bucket,
					new FirebaseStorageOptions
					{


						AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
						ThrowOnCancel = true

					}


					)
					.Child($"assets/{file.FileName}")
					.PutAsync(memoryStream, cancelation.Token);
				var accessLink = await upload;
				return Json(accessLink);
			}
			catch (Exception ex) {
			
				Console.WriteLine(ex);
			
			}
						
				

			return BadRequest();





		}


		

		[HttpPost]
		
		[Authorize]
		
		public IActionResult createLibraries([FromBody] LibraryEnglish library)
		{
			string userId = User.Identity.GetUserId();
			
			if (userId == null) 
			{
				return Json(BadRequest("Ви не авторизовані для того щоб створювати модуль зареєструйтеся або авторизуйтеся"));
			}
			if (library.Name.IsNullOrEmpty())
			{
				Console.WriteLine(library.Name);
				return BadRequest(Json("Ви не вказали імя модуля"));
			}
			
			
			var module = new LibraryEnglish { UserId = userId, Name = library.Name,img=library.img };

			_context.libraries.Add(module);

			_context.SaveChanges();

			return Json(module);

		}

		[Authorize]
		[HttpGet]
		public IActionResult getLibraries()
		{
			var libraries = _context.libraries.Where(l=>l.UserId==User.Identity.GetUserId());

			if (libraries.IsNullOrEmpty())
			{
				return Json("У вас немає модулів");
			}
			return Json(libraries);

		}
		[Authorize]
		[HttpGet("/getOneLibraries/{id}")]

		public IActionResult getOneLibraries([FromRoute] int id)
		{
			var librarie = _context.libraries.FirstOrDefault(librarie=>librarie.Id==id);

			if (librarie==null)
			{
				return Json(NotFound("Такого модуля не існує"));
			}
			return Json(librarie);

		}

		[Authorize]
		[HttpPut("/editOneLibraries/{id}")]

		public IActionResult editOneLibraries([FromRoute] int id,[FromBody] LibraryEnglish library)
		{
			var librarie = _context.libraries.FirstOrDefault(librarie => librarie.Id == id);

			if (librarie == null)
			{
				return Json(NotFound("Такого модуля не існує"));
			}

			librarie.Name = library.Name;
			_context.libraries.Update(librarie);
			_context.SaveChanges();

			return Json("Все успішно змінено");

		}



			[Authorize]
		[HttpDelete("/deleteOneLibraries/{id}")]

		public IActionResult deleteOneLibraries([FromRoute] int id)
		{
			var librarie = _context.libraries.FirstOrDefault(librarie => librarie.Id == id);

			if (librarie == null)
			{
				return Json(NotFound("Такого модуля не існує"));
			}

		
			_context.libraries.Remove(librarie);
			_context.SaveChanges();

			return Json("Все успішно змінено");

		}

	
		[HttpPost("/searchLibrary")]

		public IActionResult SearchLibrary([FromQuery] string searchString)
		{
			var librarie = _context.libraries.Where(librarie => librarie.Name.Contains(searchString));

			if (librarie == null)
			{
				return Json(NotFound("Такого модуля не існує"));
			}


			
			

			return Json(librarie);

		}

		[HttpPost("/translateWord")]

		public async Task<IActionResult> TranslateWord([FromBody] string searchString)
		{
			var translate = new AmazonTranslateClient("AKIAYMDT7FSJAGZSOFHH", "ldbVO4I6YzO/mrEAHxa6kHlnuZI0nsW3y2tHPtv/", RegionEndpoint.USEast1);

			var request = new TranslateTextRequest() { Text=searchString,SourceLanguageCode="en",TargetLanguageCode="uk"};

			var a =await translate.TranslateTextAsync(request);


			return Json(a);

		}

		[HttpPost("/translateVoice")]
		[Consumes("text/plain")]

		public async Task<IActionResult> TranslateSpeach()
		{
			string searchString;
			using (var reader = new StreamReader(Request.Body))
			{
				searchString = await reader.ReadToEndAsync();
				if (searchString.IsNullOrEmpty())
				{
					return BadRequest("String is shit");
				}
			}

			var credentials = new BasicAWSCredentials("AKIAYMDT7FSJAGZSOFHH", "ldbVO4I6YzO/mrEAHxa6kHlnuZI0nsW3y2tHPtv/");

			var polly = new AmazonPollyClient(credentials, RegionEndpoint.USEast1);

			

			SynthesizeSpeechRequest sreq = new SynthesizeSpeechRequest();
			sreq.Text = searchString;
			sreq.OutputFormat = OutputFormat.Mp3;
			sreq.VoiceId = VoiceId.Amy;
			SynthesizeSpeechResponse sres = await polly.SynthesizeSpeechAsync(sreq);



			var imageIn64 = ConvertAudioToBase64(sres);


			var memoryStream = new MemoryStream(imageIn64);




			try
			{
				var authProvider = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
				var a = await authProvider.SignInWithEmailAndPasswordAsync(authEmail, authPassword);

				var cancelation = new CancellationTokenSource();

				var upload = new FirebaseStorage(

					Bucket,
					new FirebaseStorageOptions
					{


						AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
						ThrowOnCancel = true

					}


					)
					.Child($"assets/{Guid.NewGuid()}")
					.PutAsync(memoryStream, cancelation.Token);
				var accessLink = await upload;

				return Json(accessLink);
			}
			catch (Exception ex)
			{

				Console.WriteLine(ex);

			}



			return BadRequest();

		}

		



	}
}
