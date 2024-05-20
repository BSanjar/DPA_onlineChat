using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onlineChat2.Models.DB_Models;
using System.Security.Claims;

namespace onlineChat2.Controllers
{
   
    public class UserController : Controller
	{
        private readonly FeedbackContext _db;

		public UserController(FeedbackContext db) {  _db = db; }

        [HttpGet]
		public IActionResult Login()
		{
			if (User.Identity.IsAuthenticated && (User.IsInRole("admin") || User.IsInRole("jur") || User.IsInRole("tech")))
			{
				return RedirectToAction("Index", "Admin");
			}
			return View();
		}

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            User user = await _db.Users.FirstOrDefaultAsync(a=>a.Email == email &&  a.Password == password);

			if (user != null)
			{
				await Authenticate(user);
                return RedirectToAction("Index", "Admin");
            }
			else
			{
				ViewBag.error = "Пользователь не найден";
				return View();
			}
        }        

        [HttpGet]
		public async Task<IActionResult> LoginTestTech2()
		{
			User userModel = new User();
			userModel.Email = "beka@gmail.com";
			userModel.Name = "Бека2";
			userModel.Id = "4";
			userModel.Role = "tech";
			//аутентификация

			await Authenticate(userModel);

			return RedirectToAction("Index", "Admin");
		}


		[HttpGet]
		public async Task<IActionResult> LoginTestTech()
		{
			User userModel = new User();
			userModel.Email = "beka@gmail.com";
			userModel.Name = "Бека";
			userModel.Id = "3";
			userModel.Role = "tech";
			//аутентификация

			await Authenticate(userModel);

			return RedirectToAction("Index", "Admin");
		}

		[HttpGet]
		public async Task<IActionResult> LoginTestUser()
		{
			User userModel = new User();
			userModel.Email = "baigaziev@gmail.com";
			userModel.Name = "Санжар";
			userModel.Id = "1";
			userModel.Role = "user";
			//аутентификация

			await Authenticate(userModel);

			return RedirectToAction("Index", "Home");
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
	}
}
