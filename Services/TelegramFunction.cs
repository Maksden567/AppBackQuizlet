using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using AppQuizlet.Controllers;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNet.Identity;
using App.Models;
using Firebase.Auth;
using App;
using System.Reflection;
using System.Collections.Generic;

using System.IO;
using Firebase.Storage;
using Google.Apis.Storage.v1.Data;
using Amazon.Polly.Model;
using Amazon.Polly;
using Amazon.Runtime;
using Amazon;

namespace AppQuizlet.Services
{
	public class TelegramFunction
	{

		private readonly Microsoft.AspNetCore.Identity.SignInManager<App.Models.User> _signInManager;
		private ApplicationContext  _db;

		public TelegramFunction(SignInManager<App.Models.User> signInManager,ApplicationContext context)
		{
			_signInManager = signInManager;
			_db = context;
		}


		public async Task bot()
		{

			var botClient = new TelegramBotClient("6320318391:AAHYg3yB7Iy8Vl1FFq3mg03r98cm1mw70y8");
			using CancellationTokenSource cts = new();
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			string email = "";
			int countAnswer = 0;
			int countofCorrectAnswer = 0;
			string password = "";
			string accessLinkPhoto = "";
			string newModuleTitle = "";
			string newTitle = "";
			string newTranslateTitle = "";
			bool isedit = false;
			bool isAutorize = false;
			bool isCreate = false;
			bool isDelete = false;
			int moduleId = 0;
			bool isNoAddPhoto = false;
			bool isAddPhoto = false;
			bool isAddPhotoLink = false;
			bool isChangePhoto = false;
			bool isChangeLink = false;
			bool isNoChangeLink = false;
			bool isTitleEdit = false;
			bool isTranslateTitle = false;
			string apiKey = "AIzaSyCW48XZoX416QuvSgZasdq0lH23ha5bJ2o";
			string Bucket = "dashka-71be6.appspot.com";
			string authEmail = "babadid346@gmail.com";
			string authPassword = "Maksden567";
			bool isModuleCreated = false;
			long chatid1 = 0;
			WordModel editmodule = null;
			App.Models.User user = null;
			ReceiverOptions receiverOptions = new()

			{
				AllowedUpdates = new UpdateType[]
		{
		UpdateType.CallbackQuery,
		UpdateType.Message,
		UpdateType.EditedMessage,
		UpdateType.PollAnswer,
		UpdateType.Poll,
		}
			};
			botClient.StartReceiving(
				updateHandler: HandleUpdateAsync,
				pollingErrorHandler: HandlePollingErrorAsync,
				receiverOptions: receiverOptions,
				cancellationToken: cts.Token
			);
			async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken token)
			{
				throw new NotImplementedException();
			}





			async Task defaultCCommand(long chatId, CancellationToken token, List<InlineKeyboardButton> buttons)
			{
				password = "";
				email = "";
				isAutorize = false;
				isCreate = false;
				isedit = false;
				isTitleEdit = false;
				isChangePhoto = false;
				isChangeLink = false;
				isNoChangeLink = false;
				isModuleCreated = false;
				moduleId = 0;
				isAddPhoto = false;
				isNoAddPhoto = false;
				isDelete = false;
				isTranslateTitle = false;
				{
					Message sentMessage = await botClient.SendTextMessageAsync(
					chatId: chatId,
					text: "Привіт, тисни кнопку війти щоб увійти до свого акаунта",
					replyMarkup: new InlineKeyboardMarkup(buttons),
					cancellationToken: token);
				};


			}






			async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken token)
			{

				string command;
				bool isEmailChecked = false;


				if (update.PollAnswer != null)
				{

					if (update.PollAnswer.OptionIds[0] == dictionary.FirstOrDefault(x => x.Key == update.PollAnswer.PollId).Value)
					{


						countofCorrectAnswer++;

					};
					countAnswer++;
					if (countAnswer == dictionary.Count)
					{
						{
							Message sentMessage = await botClient.SendTextMessageAsync(
							chatId: update.PollAnswer.User.Id,
							parseMode: ParseMode.Html,
							text: $"<i>Ви пройшли тест на</i> <b>{(countofCorrectAnswer * 100) / countAnswer}%</b> ✅",
							cancellationToken: token);
						};

					}


				}



				if (update.CallbackQuery != null)
				{




					if (!update.CallbackQuery.Data.Contains("getModule") && _db.words.FirstOrDefault(w => w.WordId.ToString() == update.CallbackQuery.Data) != null)
					{


						var selectObj = _db.words.FirstOrDefault(w => w.WordId.ToString() == update.CallbackQuery.Data);
						Message sentMessage = await botClient.SendTextMessageAsync(
						chatId: update.CallbackQuery.From.Id,
						text: $"{selectObj.NameEnglish}-{selectObj.TranslateName}",
						cancellationToken: token);
						if (selectObj.imageUrl != null)
						{
							Message sentMessage3 = await botClient.SendPhotoAsync(
					chatId: update.CallbackQuery.From.Id,
					photo: InputFile.FromUri(selectObj.imageUrl)
					);
						}

						if (selectObj.musicUrl != null)
						{
							Message sentMessage2 = await botClient.SendVoiceAsync(
						chatId: update.CallbackQuery.From.Id,
						voice: InputFile.FromUri($"{selectObj.musicUrl}")



						);
						}

					}


					//update.CallbackQuery.Data = "";
					if (update.CallbackQuery.Data == "login")
					{

						Message sentMessage = await botClient.SendTextMessageAsync(
						chatId: update.CallbackQuery.From.Id,
						text: "Введи email",
						cancellationToken: token);

					}

					//update.CallbackQuery.Data = "";
		if (update.CallbackQuery.Data.Contains("deleteModule"))
					{
						chatid1=update.CallbackQuery.From.Id;	
						var id = update.CallbackQuery.Data.Substring(12, update.CallbackQuery.Data.Length - 12);
						var deleteModule = _db.libraries.FirstOrDefault(l => l.Id.ToString() == id);
						_db.libraries.Remove(deleteModule);
						_db.SaveChanges();
						isDelete = true;
						isAutorize = true;
						{
							Message sentMessage = await botClient.SendTextMessageAsync(
							chatId: update.CallbackQuery.From.Id,
							text: "Модуль удалено успішно ✅",
							cancellationToken: token);
						};
					}

					IQueryable<WordModel> words = null;
					var wordsID = "";

					if (update.CallbackQuery.Data.Contains("deleteWord"))
					{
						var id = update.CallbackQuery.Data.Substring(10, update.CallbackQuery.Data.Length - 10);
						var deleteModule = _db.words.FirstOrDefault(word => word.WordId.ToString() == id);
						_db.words.Remove(deleteModule);
						_db.SaveChanges();
						{
							Message sentMessage = await botClient.SendTextMessageAsync(
							chatId: update.CallbackQuery.From.Id,
							text: "Слово удалено успішно ✅",
							cancellationToken: token);
						};

						wordsID = deleteModule.LibraryEnglishId.ToString();
						isedit = false;
						isCreate = false;
					}

					if (update.CallbackQuery.Data.Contains("test"))
					{

						string module_id = update.CallbackQuery.Data.Substring(4, update.CallbackQuery.Data.Length - 4);
					
						var moduleTestWords = _db.words.Where(word => word.LibraryEnglishId.ToString() == module_id).ToList();
						if (moduleTestWords.Count < 4)
						{
							wordsID = module_id;
							Message sentMessage = await botClient.SendTextMessageAsync(
						chatId: update.CallbackQuery.From.Id,
						text: "У вас мало слів для проходження тесту 🤬",
						cancellationToken: token);
						}
						else
						{


							foreach (var word in moduleTestWords)
							{

								var options = moduleTestWords.Where(w => w.WordId != word.WordId).OrderBy(x => Guid.NewGuid()).Take(3).Select(w => w.TranslateName).ToList();
								options.Add(word.TranslateName);
								var options1 = options.OrderBy(x => Guid.NewGuid()).Take(4).ToList();

								var correctTitle = options1.FirstOrDefault(o => o == word.TranslateName);
								var correctID = options1.IndexOf(correctTitle);



								Message sentMessage1 = await botClient.SendPollAsync(
								chatId: update.CallbackQuery.From.Id,
								type: PollType.Quiz,
								question: $"Який переклад слова {word.NameEnglish}",
								options: options1,
								isAnonymous: false,

								correctOptionId: correctID

								);


								dictionary.Add(sentMessage1.Poll.Id, correctID);


							}
						}

					}

					//update.CallbackQuery.Data = "";
					if (update.CallbackQuery.Data.Contains("edit"))
					{

						var id = update.CallbackQuery.Data.Substring(4, update.CallbackQuery.Data.Length - 4);
						editmodule = _db.words.FirstOrDefault(word => word.WordId.ToString() == id);
						isedit = true;
						List<List<InlineKeyboardButton>> wordsButtons = new List<List<InlineKeyboardButton>> { };
						List<InlineKeyboardButton> addPhoto = new List<InlineKeyboardButton> { };
						List<InlineKeyboardButton> addPhotoLink = new List<InlineKeyboardButton> { };
						List<InlineKeyboardButton> notAddPhoto = new List<InlineKeyboardButton> { };
						addPhoto.Add(InlineKeyboardButton.WithCallbackData("Change Title 🖼", $"changeTitle{id}"));
						addPhotoLink.Add(InlineKeyboardButton.WithCallbackData("Change translate 🖼", $"changeTranslate{id}"));
						notAddPhoto.Add(InlineKeyboardButton.WithCallbackData("Change Photo 🖼", $"changePhoto{id}"));
						wordsButtons.Add(addPhoto);
						wordsButtons.Add(addPhotoLink);
						wordsButtons.Add(notAddPhoto);
						Message sentMessage = await botClient.SendTextMessageAsync(
						chatId: update.CallbackQuery.From.Id,
						text: "Що ви хочете змінити?",
						replyMarkup: new InlineKeyboardMarkup(wordsButtons),
						cancellationToken: token);




					}

					if (update.CallbackQuery.Data.Contains("changeTranslate"))
					{

					
						isTranslateTitle = true;
						Message sentMessage = await botClient.SendTextMessageAsync(
						chatId: update.CallbackQuery.From.Id,
						text: "Напишіть новий перевод",
						cancellationToken: token);




					}

					if (update.CallbackQuery.Data.Contains("changeTitle"))
					{

						
						isTitleEdit = true;
						Message sentMessage = await botClient.SendTextMessageAsync(
						chatId: update.CallbackQuery.From.Id,
						text: "Напишіть новий заголовок",
						cancellationToken: token);




					}
					if (update.CallbackQuery.Data.Contains("changePhoto"))
					{


						isChangePhoto = true;


						List<List<InlineKeyboardButton>> wordsButtons = new List<List<InlineKeyboardButton>> { };
						List<InlineKeyboardButton> addPhoto = new List<InlineKeyboardButton> { };
						List<InlineKeyboardButton> addPhotoLink = new List<InlineKeyboardButton> { };
						List<InlineKeyboardButton> notAddPhoto = new List<InlineKeyboardButton> { };
						addPhoto.Add(InlineKeyboardButton.WithCallbackData("change Photo 🖼", $"changeLocal"));
						addPhotoLink.Add(InlineKeyboardButton.WithCallbackData("change Link 🖼", $"changelink"));
						notAddPhoto.Add(InlineKeyboardButton.WithCallbackData("Not please 🖼", $"notChangePhoto"));
						wordsButtons.Add(addPhoto);
						wordsButtons.Add(addPhotoLink);
						wordsButtons.Add(notAddPhoto);
						Message sentMessage = await botClient.SendTextMessageAsync(
						chatId: update.CallbackQuery.From.Id,
						text: "Ви хочете змінити  фото?",
						replyMarkup: new InlineKeyboardMarkup(wordsButtons),
						cancellationToken: token);




					}


					if (update.CallbackQuery.Data.Contains("changeLocal"))
					{
						isChangePhoto = true;
						Message sentMessage = await botClient.SendTextMessageAsync(
						chatId: update.CallbackQuery.From.Id,
						text: "Загрузіть фото локально",
						cancellationToken: token);
					}

					if (update.CallbackQuery.Data.Contains("changelink"))
					{
						isChangeLink = true;
						Message sentMessage = await botClient.SendTextMessageAsync(
						chatId: update.CallbackQuery.From.Id,
						text: "Введіть лінк на фото",
						cancellationToken: token);
					}

					if (update.CallbackQuery.Data.Contains("notChangePhoto"))
					{
						isNoChangeLink = true;
					}




					if (update.CallbackQuery.Data.Contains("create"))
					{
						accessLinkPhoto = "";
						newTitle = "";
						newTranslateTitle = "";
						isNoAddPhoto = false;
						isAddPhoto = false;
						isAddPhotoLink = false;
						isCreate = true;
						Message sentMessage = await botClient.SendTextMessageAsync(
						chatId: update.CallbackQuery.From.Id,
						text: "Введіть назву",
						cancellationToken: token);
					}



					if (update.CallbackQuery.Data == _db.words.Select(w => w.WordId).ToString())
					{
						isCreate = true;
						Message sentMessage = await botClient.SendTextMessageAsync(
						chatId: update.CallbackQuery.From.Id,
						text: "Введіть назву",
						cancellationToken: token);
					}


					if (update.CallbackQuery.Data.Contains("addModule"))
					{
						isModuleCreated = true;
						Message sentMessage = await botClient.SendTextMessageAsync(
						chatId: update.CallbackQuery.From.Id,
						text: "Введіть назву",
						cancellationToken: token);
					}




					
					if (update.CallbackQuery.Data.Contains("getModule"))
					{
						accessLinkPhoto = "";
						newTitle = "";
						newTranslateTitle = "";
						isNoAddPhoto = false;
						isAddPhoto = false;
						wordsID = update.CallbackQuery.Data.Substring(9, update.CallbackQuery.Data.Length - 9);
					}

					if (wordsID != "")
					{
						words = _db.words.Where(word => word.LibraryEnglishId.ToString() == wordsID);

					}


					if (!words.IsNullOrEmpty())
					{
						moduleId = words.First().LibraryEnglishId;
						List<List<InlineKeyboardButton>> wordsButtons = new List<List<InlineKeyboardButton>> { };
						List<InlineKeyboardButton> createBtn = new List<InlineKeyboardButton> { };
						List<InlineKeyboardButton> buttonTest = new List<InlineKeyboardButton> { };
						foreach (var word in words)
						{
							List<InlineKeyboardButton> wordButton = new List<InlineKeyboardButton> { };
							List<InlineKeyboardButton> deleteAndEdit = new List<InlineKeyboardButton> { };
							if (word.isKnowen == true) {
								wordButton.Add(InlineKeyboardButton.WithCallbackData($"{word.NameEnglish}-{word.TranslateName}  ✅", $"{word.WordId}"));
							}
							else
							{
								wordButton.Add(InlineKeyboardButton.WithCallbackData($"{word.NameEnglish}-{word.TranslateName}  ☑️", $"{word.WordId}"));
							}

							deleteAndEdit.Add(InlineKeyboardButton.WithCallbackData("DELETE ❌", $"deleteWord{word.WordId}"));
							deleteAndEdit.Add(InlineKeyboardButton.WithCallbackData("EDIT ✏️", $"edit{word.WordId}"));

							wordsButtons.Add(wordButton);
							wordsButtons.Add(deleteAndEdit);

						}
						createBtn.Add(InlineKeyboardButton.WithCallbackData("Create 📝", $"create"));
						buttonTest.Add(InlineKeyboardButton.WithCallbackData("PassTest 📝", $"test{moduleId}"));
						wordsButtons.Add(createBtn);
						wordsButtons.Add(buttonTest);
						{
							Message sentMessage = await botClient.SendTextMessageAsync(
							chatId: update.CallbackQuery.From.Id,
							text: "Ось всі твої слова, обирай яке додати слово або тисни створити слово щоб додати нове",
							replyMarkup: new InlineKeyboardMarkup(wordsButtons),
							cancellationToken: token);
						};

					}

					else if (_db.libraries.FirstOrDefault(l => l.Id.ToString() == wordsID) != null)
					{
						moduleId = _db.libraries.FirstOrDefault(l => l.Id.ToString() == wordsID).Id;
						List<List<InlineKeyboardButton>> wordsButtons = new List<List<InlineKeyboardButton>> { };
						List<InlineKeyboardButton> createBtn = new List<InlineKeyboardButton> { };
						createBtn.Add(InlineKeyboardButton.WithCallbackData("Create 📝", $"create"));
						wordsButtons.Add(createBtn);
						{
							Message sentMessage = await botClient.SendTextMessageAsync(
							chatId: update.CallbackQuery.From.Id,
							text: "У тебе немає слів добавляй нове та вивчай разом з нами🤗",
							replyMarkup: new InlineKeyboardMarkup(wordsButtons),
							cancellationToken: token);
						};
					}



					if (update.CallbackQuery.Data.Contains("addPhoto"))
					{
						isAddPhoto = true;
						Message sentMessage = await botClient.SendTextMessageAsync(
						chatId: update.CallbackQuery.From.Id,
						text: "Загрузіть фото локально",
						cancellationToken: token);
					}

					if (update.CallbackQuery.Data.Contains("Link"))
					{
						isAddPhotoLink = true;
						Message sentMessage = await botClient.SendTextMessageAsync(
						chatId: update.CallbackQuery.From.Id,
						text: "Напишіть лінку на фото",
						cancellationToken: token);
					}



					if (update.CallbackQuery.Data.Contains("notAddPhoto"))
					{
						isNoAddPhoto = true;
						if (!newTitle.IsNullOrEmpty() && !newTranslateTitle.IsNullOrEmpty() && isNoAddPhoto)
						{

							using (var memoryStream = new MemoryStream())
							{

								var credentials = new BasicAWSCredentials("AKIAYMDT7FSJAGZSOFHH", "ldbVO4I6YzO/mrEAHxa6kHlnuZI0nsW3y2tHPtv/");

								var polly = new AmazonPollyClient(credentials, RegionEndpoint.USEast1);



								SynthesizeSpeechRequest sreq = new SynthesizeSpeechRequest();
								sreq.Text = newTranslateTitle;
								sreq.OutputFormat = OutputFormat.Mp3;
								sreq.VoiceId = VoiceId.Amy;
								SynthesizeSpeechResponse sres = await polly.SynthesizeSpeechAsync(sreq);
								sres.AudioStream.CopyTo(memoryStream);
								byte[] audioBytes = memoryStream.ToArray();

								var memory = new MemoryStream(audioBytes);
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
										.PutAsync(memory, cancelation.Token);
								var accessLink = await upload;
								var wordWithPhoto = new WordModel { TranslateName = newTranslateTitle, NameEnglish = newTitle, isKnowen = false, LibraryEnglishId = moduleId, musicUrl = accessLink };
								_db.words.Add(wordWithPhoto);
								_db.SaveChanges();


							}
							chatid1 = update.CallbackQuery.From.Id;

							isAutorize = true;

						}
					}

					
					

				}

					
					// Only process text messages

					if (update.Message != null)
				{


					command = update.Message.Text;

					var chatId = update.Message.Chat.Id;


					InlineKeyboardButton urlButton = InlineKeyboardButton.WithCallbackData("Увійти", "login");



					List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton> { urlButton };







					switch (command)
					{
						case "/start":
							defaultCCommand(chatId, token, buttons);

							break;

					}

					if (email.IsNullOrEmpty() && update.Message.Text.Contains("@gmail.com"))
					{
						email = update.Message.Text;
						Message sentMessage = await botClient.SendTextMessageAsync(
						chatId: chatId,
						text: "Введи password",
						cancellationToken: token);

					}

					else if (password.IsNullOrEmpty() && update.Message.Text != null && !email.IsNullOrEmpty())
					{
						password = update.Message.Text;

					}



					if (!email.IsNullOrEmpty() && !password.IsNullOrEmpty())
					{


						if (update.Message.Text == password)
						{
							user = _db.Users.FirstOrDefault(user => user.Email == email);
							var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

							if (result.Succeeded)
							{
								Message sentMessage = await botClient.SendTextMessageAsync(
								chatId: chatId,
								text: "Ви авторизовані",
								cancellationToken: token);
							}

							isAutorize = true;
						}




						if (isTitleEdit)
						{

							if (newTitle.IsNullOrEmpty() && update.Message.Text != null && update.Message.Text != password)
							{
								newTitle = update.Message.Text;
								isAutorize = true;
							}


							if (!newTitle.IsNullOrEmpty())
							{
								editmodule.NameEnglish = newTitle;
								_db.SaveChanges();
							}

							newTitle = "";
							isTitleEdit = false;
						}

						if (isTranslateTitle)
						{

							if (newTranslateTitle.IsNullOrEmpty() && update.Message.Text != null && update.Message.Text != password)
							{
								newTranslateTitle = update.Message.Text;
								isAutorize = true;
							}


							if (!newTranslateTitle.IsNullOrEmpty())
							{
								using (var memoryStream = new MemoryStream())
								{

									var credentials = new BasicAWSCredentials("AKIAYMDT7FSJAGZSOFHH", "ldbVO4I6YzO/mrEAHxa6kHlnuZI0nsW3y2tHPtv/");

									var polly = new AmazonPollyClient(credentials, RegionEndpoint.USEast1);



									SynthesizeSpeechRequest sreq = new SynthesizeSpeechRequest();
									sreq.Text = newTranslateTitle;
									sreq.OutputFormat = OutputFormat.Mp3;
									sreq.VoiceId = VoiceId.Amy;
									SynthesizeSpeechResponse sres = await polly.SynthesizeSpeechAsync(sreq);
									sres.AudioStream.CopyTo(memoryStream);
									byte[] audioBytes = memoryStream.ToArray();

									var memory = new MemoryStream(audioBytes);
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
											.PutAsync(memory, cancelation.Token);
									var accessLink = await upload;
									editmodule.musicUrl = accessLink;
									editmodule.TranslateName = newTranslateTitle;
									_db.SaveChanges();


								}

								
							}

							newTranslateTitle = "";
							isTranslateTitle= false;
						}

						if (isChangePhoto && update.Message.Photo!=null)
						{
							var file1 = update.Message.Photo;

							var file = await botClient.GetFileAsync(update.Message.Photo[1].FileId);
							if (!file.FilePath.IsNullOrEmpty())

								using (var memoryStream = new MemoryStream())
								{



									await botClient.DownloadFileAsync(file.FilePath, memoryStream);
									byte[] fileBytes = memoryStream.ToArray();
									var memory = new MemoryStream(fileBytes);
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
										.Child($"assets/{"mxmxmx"}.jpg")
										.PutAsync(memory, cancelation.Token);
									accessLinkPhoto = await upload;
									editmodule.imageUrl = accessLinkPhoto;
								}

							isAutorize = true;
							isChangePhoto = false;
						}

						if (isChangeLink)
						{

							editmodule.imageUrl = update.Message.Text;
					

							isAutorize = true;
							isChangeLink = false;
						}


						if (isNoChangeLink)
						{


							isAutorize = true;
							isNoChangeLink = false;
						}



						if (isCreate)
						{


							if (newTitle.IsNullOrEmpty() && update.Message.Text != null && update.Message.Text != password)
							{
								newTitle = update.Message.Text;

								{
									Message sentMessage = await botClient.SendTextMessageAsync(
									chatId: chatId,
									text: "Введіть перевод",
									cancellationToken: token);
								}
								isAutorize = false;
							}

							else if (update.Message.Text != null && !newTitle.IsNullOrEmpty() && newTranslateTitle.IsNullOrEmpty())
							{
								newTranslateTitle = update.Message.Text;



								List<List<InlineKeyboardButton>> wordsButtons = new List<List<InlineKeyboardButton>> { };
								List<InlineKeyboardButton> addPhoto = new List<InlineKeyboardButton> { };
								List<InlineKeyboardButton> addPhotoLink = new List<InlineKeyboardButton> { };
								List<InlineKeyboardButton> notAddPhoto = new List<InlineKeyboardButton> { };
								addPhoto.Add(InlineKeyboardButton.WithCallbackData("Add 🖼", $"addPhoto"));
								addPhotoLink.Add(InlineKeyboardButton.WithCallbackData("AddLink 🖼", $"Link"));
								notAddPhoto.Add(InlineKeyboardButton.WithCallbackData("Not please 🖼", $"notAddPhoto"));
								wordsButtons.Add(addPhoto);
								wordsButtons.Add(addPhotoLink);
								wordsButtons.Add(notAddPhoto);
								Message sentMessage = await botClient.SendTextMessageAsync(
								chatId: chatId,
								text: "Ви хочете додати фото?",
								replyMarkup: new InlineKeyboardMarkup(wordsButtons),
								cancellationToken: token);
							}


							

							else if (update.Message.Photo != null && isAddPhoto)
							{


								var file1 = update.Message.Photo;

								var file = await botClient.GetFileAsync(update.Message.Photo[1].FileId);
								if (!file.FilePath.IsNullOrEmpty())

									using (var memoryStream = new MemoryStream())
									{



										await botClient.DownloadFileAsync(file.FilePath, memoryStream);
										byte[] fileBytes = memoryStream.ToArray();
										var memory = new MemoryStream(fileBytes);
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
											.Child($"assets/{"mxmxmx"}.jpg")
											.PutAsync(memory, cancelation.Token);
										accessLinkPhoto = await upload;
									}
							}



						}

					

						if (!newTitle.IsNullOrEmpty() && !newTranslateTitle.IsNullOrEmpty() && isAddPhoto)
						{

							using (var memoryStream = new MemoryStream())
							{

								var credentials = new BasicAWSCredentials("AKIAYMDT7FSJAGZSOFHH", "ldbVO4I6YzO/mrEAHxa6kHlnuZI0nsW3y2tHPtv/");

								var polly = new AmazonPollyClient(credentials, RegionEndpoint.USEast1);



								SynthesizeSpeechRequest sreq = new SynthesizeSpeechRequest();
								sreq.Text = newTranslateTitle;
								sreq.OutputFormat = OutputFormat.Mp3;
								sreq.VoiceId = VoiceId.Amy;
								SynthesizeSpeechResponse sres = await polly.SynthesizeSpeechAsync(sreq);
								sres.AudioStream.CopyTo(memoryStream);
								byte[] audioBytes = memoryStream.ToArray();

								var memory = new MemoryStream(audioBytes);
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
										.PutAsync(memory, cancelation.Token);
								var accessLink = await upload;
								var wordWithPhoto = new WordModel { TranslateName = newTranslateTitle, NameEnglish = newTitle, imageUrl = accessLinkPhoto, isKnowen = false, LibraryEnglishId = moduleId, musicUrl = accessLink };
								_db.words.Add(wordWithPhoto);
								_db.SaveChanges();

							}
							isAutorize = true;

						}

						else if (!newTitle.IsNullOrEmpty() && !newTranslateTitle.IsNullOrEmpty() && isAddPhotoLink)
						{

							var imgUrl = update.Message.Text;

							using (var memoryStream = new MemoryStream())
							{

								var credentials = new BasicAWSCredentials("AKIAYMDT7FSJAGZSOFHH", "ldbVO4I6YzO/mrEAHxa6kHlnuZI0nsW3y2tHPtv/");

								var polly = new AmazonPollyClient(credentials, RegionEndpoint.USEast1);



								SynthesizeSpeechRequest sreq = new SynthesizeSpeechRequest();
								sreq.Text = newTranslateTitle;
								sreq.OutputFormat = OutputFormat.Mp3;
								sreq.VoiceId = VoiceId.Amy;
								SynthesizeSpeechResponse sres = await polly.SynthesizeSpeechAsync(sreq);
								sres.AudioStream.CopyTo(memoryStream);
								byte[] audioBytes = memoryStream.ToArray();

								var memory = new MemoryStream(audioBytes);
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
										.PutAsync(memory, cancelation.Token);
								var accessLink = await upload;
								var wordWithPhoto = new WordModel { TranslateName = newTranslateTitle, NameEnglish = newTitle, imageUrl = imgUrl, isKnowen = false, LibraryEnglishId = moduleId, musicUrl = accessLink, };
								_db.words.Add(wordWithPhoto);
								_db.SaveChanges();

							}
							isAutorize = true;
						}






					


					}




					if (isModuleCreated)
					{
						if (newModuleTitle.IsNullOrEmpty() && update.Message.Text != null && update.Message.Text != password)
						{
							newModuleTitle = update.Message.Text;
							isAutorize = true;
						}

						if (!newModuleTitle.IsNullOrEmpty())
						{
							var module = new LibraryEnglish { Name = newModuleTitle, UserId = user.Id };
							_db.libraries.Add(module);
							_db.SaveChanges();
						}

						isModuleCreated = false;
					}




					if (isAutorize)
					{
						var modules = _db.libraries.Where(module => module.UserId == user.Id);
						List<List<InlineKeyboardButton>> moduleButtons = new List<List<InlineKeyboardButton>> { };
						List<InlineKeyboardButton> createModuleBtn = new List<InlineKeyboardButton> { };
						foreach (var module in modules)
						{
							List<InlineKeyboardButton> moduleButton = new List<InlineKeyboardButton> { };
							List<InlineKeyboardButton> deleteModuleButton = new List<InlineKeyboardButton> { };
							moduleButton.Add(InlineKeyboardButton.WithCallbackData($"{module.Name}", $"getModule{module.Id}"));
							deleteModuleButton.Add(InlineKeyboardButton.WithCallbackData($"Удалить модуль ❌", $"deleteModule{module.Id}"));
							moduleButtons.Add(moduleButton);
							moduleButtons.Add(deleteModuleButton);
						}

						createModuleBtn.Add(InlineKeyboardButton.WithCallbackData($"✏️ Створити модуль", $"addModule"));
						moduleButtons.Add(createModuleBtn);

						{
							Message sentMessage = await botClient.SendTextMessageAsync(
							chatId: chatId,
							text: "Ось всі твої модулі, обирай до якого додати слово або тисни створити модуль щоб створити новий",
							replyMarkup: new InlineKeyboardMarkup(moduleButtons),
							cancellationToken: token);
						};


					}







				}

				else if (isNoAddPhoto || isDelete)
				{
					if (isAutorize)
					{
						var modules = _db.libraries.Where(module => module.UserId == user.Id);
						List<List<InlineKeyboardButton>> moduleButtons = new List<List<InlineKeyboardButton>> { };
						List<InlineKeyboardButton> createModuleBtn = new List<InlineKeyboardButton> { };
						foreach (var module in modules)
						{
							List<InlineKeyboardButton> moduleButton = new List<InlineKeyboardButton> { };
							List<InlineKeyboardButton> deleteModuleButton = new List<InlineKeyboardButton> { };
							moduleButton.Add(InlineKeyboardButton.WithCallbackData($"{module.Name}", $"getModule{module.Id}"));
							deleteModuleButton.Add(InlineKeyboardButton.WithCallbackData($"Удалить модуль ❌", $"deleteModule{module.Id}"));
							moduleButtons.Add(moduleButton);
							moduleButtons.Add(deleteModuleButton);
						}

						createModuleBtn.Add(InlineKeyboardButton.WithCallbackData($"✏️ Створити модуль", $"addModule"));
						moduleButtons.Add(createModuleBtn);

						{
							Message sentMessage = await botClient.SendTextMessageAsync(
							chatId: chatid1,
							text: "Ось всі твої модулі, обирай до якого додати слово або тисни створити модуль щоб створити новий",
							replyMarkup: new InlineKeyboardMarkup(moduleButtons),
							cancellationToken: token);
						};


					}

					isDelete = false;
					isAutorize = false;

				}
				}

			}
		}
	}

