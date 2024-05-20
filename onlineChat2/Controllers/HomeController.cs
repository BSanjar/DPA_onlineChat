using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using onlineChat2.Helpers;
using onlineChat2.Models;
using onlineChat2.Models.DB_Models;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace onlineChat2.Controllers
{
	//[Authorize]
	public class HomeController : Controller
	{
        private readonly appSettings _appSettings;
        private readonly ILogger<HomeController> _logger;
		private readonly FeedbackContext _db;
		public HomeController(ILogger<HomeController> logger, FeedbackContext db, IOptions<appSettings> appSettings)
		{
			_db = db;
			_logger = logger;
            _appSettings = appSettings.Value;
        }

		//короче, решил сделать авторизацию чз чат, т.е запросим email, name пошагово, каждый раз приходит запрос сюда, тут я проверяю и провожу авторизацию
		//если все ок то открываю чат. Если пользователь выбирает другого специалиста, то начинаю заново но не буду повторно создавать пользователя, просто проверяю имя в БД.

		public async Task<IActionResult> newChat()
		{
			ViewData["Title"] = "Онлайн чат ГАЗПД";

            ViewBag.step = 0;
			return View();
		}

		[HttpPost]
		public async Task<tempChat> newChat(tempChat requestData)
		{			
			User user;
			switch (requestData.step)
			{
				case "email":
					if (IsValidEmail(requestData.email))
					{
						user = new User();
						user = _db.Users.FirstOrDefault(a => a.Email == requestData.email);
						if (user != null)
						{
							//аутентифицирую
							await Authenticate(user);

							//если пользователь есть в БД, то прошу ввести тематику
							requestData.step = "typeTheme";
							requestData.name = user.Name;
							requestData.userId = user.Id;
							return requestData;
						}
						else
						{

							//Добавляю во временное хранилище
							requestData.step = "name";
							return requestData;
						}
					}
					else
					{
						//должен ввести почту
						requestData = new tempChat();
						requestData.step = "email";

						return requestData;
					}
					break;

				case "name":
					//регистрирую нового пользователя
					user = new User();
					user = createNewUser(requestData.name, requestData.email);

					//аутентифицирую
					await Authenticate(user);

					//должен ввести тему
					requestData.userId = user.Id;
					requestData.step = "typeTheme";
					return requestData;
					break;
				case "typeTheme":
					//Беру данные текущего пользователя
					var usr = await _db.Users.FirstOrDefaultAsync(a => a.Id == User.FindFirst("id").Value);

					//уведомляю в телеграм
					Integrations integrations = new Integrations(_appSettings);
					
					//await integrations.newOnlineChatMsg(requestData);

					//создаю чат и возвращаю
					var chat = await createNewChat(usr, requestData.typeTheme);
					requestData.step = "msg";
					requestData.groupId = chat.Id;
					ViewBag.chat = chat;

					


					return requestData;
					break;
				default: return requestData;
			}
		}

		public async Task Authenticate(User user)
		{
			// создаем один claim
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.Name),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.Role, user.Role),
				new Claim("id", user.Id)
			};
			// создаем объект ClaimsIdentity
			ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
			// установка аутентификационных куки
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));

		}

		private User createNewUser(string usrName, string usrEmail)
		{
			try
			{
				User user = new User();
				user.Name = usrName;
				user.Email = usrEmail;
				user.Password = usrEmail;
				user.Id = Guid.NewGuid().ToString();
				user.Role = "user";
				user.PhoneNumber = usrEmail;

				_db.Users.Add(user);
				_db.SaveChanges();

				return user;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw ex;
			}
		}


		/// <summary>
		/// Создает или возвращает существующий чат
		/// </summary>
		/// <param name="userSender"></param>
		/// <param name="typeTheme"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		private async Task<Chat> createNewChat(User userSender, string typeTheme)
		{
			//сначала проверяю был ли чат у юзера
			 Chat chat = new Chat();

			chat = await _db.Chats.FirstOrDefaultAsync(a=>a.User==userSender.Id && a.TypeTheme==typeTheme);	
			if(chat != null)
			{
				return chat;
			}


			//беру админа для переписки у которого соотвествующее направление и у которого было мало переписек
			var usrChats = await _db.Users.Include(c => c.ChatAdminNavigations)
				.Where(a => a.Role == typeTheme)
				.Select(u => new
				{
					User = u,
					ChatCount = u.ChatAdminNavigations.Count()
				})
				.OrderBy(u => u.ChatCount)
				.FirstOrDefaultAsync();

			if (usrChats != null && usrChats.User != null)
			{
				User usr = usrChats.User;
				string chatId = Guid.NewGuid().ToString();

				ViewBag.groupId = chatId;
				ViewBag.currentUser = await _db.Users.FirstOrDefaultAsync(a => a.Id == User.FindFirst("id").Value);

				Chat newChat = new Chat();
				newChat.Id = chatId;
				newChat.Admin = usr.Id;
				newChat.CreatedAt = DateTime.Now;
				newChat.UpdatedAt = DateTime.Now;
				newChat.User = userSender.Id;
				newChat.MsgSource = "onlinechat";
				newChat.Status = "new";
				newChat.LangMsg = "ru"; //временно
				newChat.TypeTheme = typeTheme;


				await _db.Chats.AddAsync(newChat);
				await _db.SaveChangesAsync();
				return newChat;
			}
			else
			{
				throw new Exception("Не удалось найти админа для переписки");
			}
		}

		static bool IsValidEmail(string email)
		{
			// Шаблон регулярного выражения для проверки адреса электронной почты
			string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

			// Проверка соответствия шаблону
			return Regex.IsMatch(email, pattern);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}


		[HttpGet]
		public IActionResult feedBack(string status = "", string message = "")
		{
            ViewBag.status = status;
            ViewBag.message = message;

            var categoryes = _db.Themcategoryes.ToList();
            ViewData["Title"] = "Форма обратной связи";
            return View(categoryes);
		}

		[HttpPost]
		public IActionResult feedBack(Chat chat)
        {
			try
			{
				string typeTheme = _db.Themcategoryes.FirstOrDefault(a => a.Category == chat.Title).TypeTheme;

				var user = createUser(chat.UserNavigation);

                if (user!=null) 
				{
                    //беру админа для переписки у которого соотвествующее направление и у которого было мало переписек
                    var usrChats =  _db.Users.Include(c => c.ChatAdminNavigations)
                        .Where(a => a.Role == typeTheme)
                        .Select(u => new
                        {
                            User = u,
                            ChatCount = u.ChatAdminNavigations.Count()
                        })
                        .OrderBy(u => u.ChatCount)
                        .FirstOrDefault();

                    if (usrChats != null && usrChats.User != null)
                    {
                        User usr = usrChats.User;

						chat.UserNavigation = null;
						chat.User = user.Id;
                        chat.Admin = usr.Id;
						chat.TypeTheme = typeTheme;
						chat.CreatedAt = DateTime.Now;
						chat.UpdatedAt = chat.CreatedAt;
						chat.Status = "new";
						chat.MsgSource = "feedback";
						chat.Id = Guid.NewGuid().ToString();

						_db.Chats.Add(chat);
						_db.SaveChanges();

                        return RedirectToAction("feedBack", "Home", new { status = "success", message = "Ваше обращение успешно отправлено на рассмотрение, в ближайшее время сотрудники ГАЗПД отправят ответ на вашу электронную почту которую вы указали." });
                    }
                    else
                    {
                        return RedirectToAction("feedBack", "Home", new { status = "error", message = "Не найден подходящий сотрудник, попробуйте позже" });
                    }
				}
				else
				{
                    return RedirectToAction("feedBack", "Home", new { status = "error", message = "Возникла проблема при регистрации, попробуйте еще раз" });
                }


            }
			catch (Exception ex)
			{

                return RedirectToAction("feedBack", "Home", new { status = "error", message = ex.Message });
            }
           
      
		}

		private User createUser(User user)
		{
			try
			{
				var userDb = _db.Users.FirstOrDefault(a => a.Email == user.Email);
				if (userDb == null)
                {
					user.Password = user.Email;
					user.Role = "user";
					user.Id = Guid.NewGuid().ToString();

					_db.Users.Add(user);
					_db.SaveChanges();
					return user;
				}
				else
				{
					return userDb;
				}
			}
			catch (Exception ex)
			{
				return null;
			}
        }

	}
}