using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onlineChat2.Models.DB_Models;

namespace onlineChat2.Controllers
{
    public class GlobalController : Controller
    {
        private readonly FeedbackContext _db;
        public GlobalController(FeedbackContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<List<Translation>> GetTranslations()
        {
            var translations = await _db.Translations.ToListAsync();
            return translations;
        }


        //[HttpGet]
        //public IActionResult GetData()
        //{
        //    var data = _db.NRegisters.ToList();
        //    return Json(data);
        //}    
    }
}
