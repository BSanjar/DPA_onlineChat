using DocumentFormat.OpenXml.Packaging;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using onlineChat2.Helpers;
using onlineChat2.Models;
using onlineChat2.Models.DB_Models;
using onlineChat2.Models.FliterModels;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace onlineChat2.Controllers
{
	[Authorize(Roles = "admin,tech,jur")]
	public class AdminController : Controller
	{
        private readonly appSettings _appSettings;
        private readonly FeedbackContext _db;
		private readonly int PageSize = 25; // Количество элементов на одной странице
		private readonly IHubContext<ChatHub> _hubContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminController(FeedbackContext db, IHubContext<ChatHub> hubContext, IWebHostEnvironment webHostEnvironment, IOptions<appSettings> appSettings)
		{
			_db = db;
			_hubContext = hubContext;
            _webHostEnvironment = webHostEnvironment;
			_appSettings = appSettings.Value;
        }

		[HttpGet]
		public async Task<IActionResult> Index(string? filter, string? typeTheme, string? searchInput, int page = 1,
		  SortState sortOrder = SortState.RegDateDesc)
		{
			User curUser = await _db.Users.FirstOrDefaultAsync(a => a.Id == User.FindFirst("id").Value);

			if (curUser != null)
			{
				IQueryable<Chat> query = _db.Chats.Include(a => a.UserNavigation).Include(a=>a.AdminNavigation);

				if (query != null)
				{
					// Фильтрация по источнику обращения
					if (!string.IsNullOrEmpty(filter) && filter != "all")
					{
						query = query.Where(p => p.MsgSource == filter);
					}


					if (!string.IsNullOrEmpty(typeTheme) && typeTheme != "all")
					{
						query = query.Where(p => p.TypeTheme == typeTheme);
					}


					// Фильтрация и поиск
					if (!string.IsNullOrEmpty(searchInput))
					{
						searchInput = searchInput.ToLower();
						query = query.Where(r =>
							(r.UserNavigation.Name != null && r.UserNavigation.Name.ToLower().Contains(searchInput)) ||
							(r.TypeTheme != null && r.TypeTheme.ToLower().Contains(searchInput)) ||
							(r.MsgSource != null && r.MsgSource.ToLower().Contains(searchInput)) ||
							(r.Question != null && r.Question.ToLower().Contains(searchInput)) ||
							(r.Title != null && r.Title.ToLower().Contains(searchInput))
						);
					}



					// Сортировка
					query = sortOrder switch
					{
						SortState.RegDateDesc => query.OrderByDescending(s => s.UpdatedAt),
						SortState.StatusAsc => query.OrderBy(s => s.Status),
						SortState.StatusDesc => query.OrderByDescending(s => s.Status),
						_ => query.OrderBy(s => s.UpdatedAt),
					};




					// Пагинация и формирование модели представления
					var paginatedData = PaginatedList<Chat>.Create(query.ToList(), page, PageSize);
					var viewModel = new IndexViewModel
					{
						Registers = paginatedData,
						SortViewModel = new SortViewModel(sortOrder),
						FilterViewModel = new FilterViewModel(filter, typeTheme, searchInput),
						TotalRecords = await query.CountAsync(),
					};


					return View(viewModel);
				}
			}

			return View();
		}

		[HttpGet]
		public async Task<IActionResult> show(string id, string status="", string message="")
		{
			ViewBag.status = status;
			ViewBag.message = message;
			var msg = await _db.Chats.FirstOrDefaultAsync(c => c.Id == id);

			await _db.Entry(msg)
				.Reference(a => a.UserNavigation)
				.LoadAsync();

			ViewBag.currentUser = await _db.Users.FirstOrDefaultAsync(a => a.Id == User.FindFirst("id").Value);

			if (msg.MsgSource == "onlinechat")
			{
				msg.Status = "closed";
				msg.Admin = User.FindFirst("id").Value;
				msg.UpdatedAt = DateTime.Now;

                await _db.SaveChangesAsync();
				//await _hubContext.Clients.Group(id).SendAsync("Receive", "User joined the group.");
				return View("OnlineChatMsg", msg);
			}
			else
			{
				return View("FeedbackMsg", msg);
			}
		}


		[HttpPost]
		public async Task<IActionResult> SendAswer(string id, string response)
		{
			try
			{
				var chat = await _db.Chats.Include(a=>a.UserNavigation).FirstOrDefaultAsync(a=>a.Id == id);
				chat.Answer = response;
				chat.Status = "closed";
				chat.Admin = User.FindFirst("id").Value;
				chat.UpdatedAt = DateTime.Now;

				await _db.SaveChangesAsync();

				Integrations integr = new Integrations(_appSettings);

				//await _db.Entry(chat).

				await integr.sendToEmail(chat);


                return RedirectToAction("show", new { id = id, status="success", message="Ваш ответ успешно отправлено на почту гражданина" });

            }
			catch (Exception ex)
			{
                return RedirectToAction("show", new { id = id, status = "error", message = "Возникла проблема при отправке: "+ex.Message+" -попробуйте еще раз" });
            }
		}


        [HttpGet]
        public async Task<FileResult> SaveFile(string id)
        {            
            return await GeneratePDF(id);
        }

        public async Task<FileResult>GeneratePDF(string id)
		{
            // Получение пути к папке TemplateFiles внутри проекта
            string templateFolderPath = Path.Combine(_webHostEnvironment.ContentRootPath, "TemplateFiles");

            // Формирование полного пути к файлу DOCX
            string docxFilePath = Path.Combine(templateFolderPath, "doc_temp_base.docx");

            string docxFilePathtemp = Path.Combine(templateFolderPath, "doc_temp.docx");
            try
			{       

				var chat = await _db.Chats.FirstOrDefaultAsync(a => a.Id == id);

				await _db.Entry(chat)
					.Reference(r => r.AdminNavigation).LoadAsync();

                await _db.Entry(chat)
                    .Reference(r => r.UserNavigation).LoadAsync();



            System.IO.File.Copy(docxFilePath, docxFilePathtemp);
			using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(docxFilePathtemp, true))
			{
				string docText = null;
				using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
				{
					docText = sr.ReadToEnd();
				}

				//string sendercard = p.SenderCardNumm.Substring(0, 6) + "******" + p.SenderCardNumm.Substring(12, 4);
				//fieldValues.Add("fromacc", sendercard);

				Regex regexText = new Regex("fio");
				docText = regexText.Replace(docText, chat.UserNavigation.Name);

				regexText = new Regex("title");
				docText = regexText.Replace(docText, chat.Title);

                regexText = new Regex("text");
                docText = regexText.Replace(docText, chat.Question);

                regexText = new Regex("admin");
                docText = regexText.Replace(docText, chat.AdminNavigation.Name);

                regexText = new Regex("date");
                docText = regexText.Replace(docText, chat.CreatedAt?.ToString("dd.MM.yyyy hh:mm:ss"));

                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
				{
					sw.Write(docText);
				}
			}

                byte[] data;
			using (StreamReader sr = new StreamReader(docxFilePathtemp))
			{
				using (MemoryStream ms = new MemoryStream())
				{
					sr.BaseStream.CopyTo(ms);
					data = ms.ToArray();
				}
			}

                
                return File(data, "application/word", "обращение" + ".docx");

            }
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}
			finally
			{
                System.IO.File.Delete(docxFilePathtemp);
            }



                //         // Создание документа
                //         iTextSharp.text.Document document = new iTextSharp.text.Document();
                //try
                //{
                //	// Создание объекта PdfWriter для записи в файл
                //	PdfWriter.GetInstance(document, new FileStream(tempFilePath, FileMode.Create));

            //	// Открытие документа
            //	document.Open();


            //             // Использование встроенного шрифта Helvetica с указанием кодировки BaseFont.CP1251 для поддержки кириллицы
            //             Font font = new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, BaseColor.BLACK);


            //             // Добавление контента
            //             document.Add(new Paragraph("Пример текста в PDF документе", font));
            //	document.Add(new Paragraph("Пример текста в jpg документе",font));

            //	// Добавление изображения (путь к изображению)
            //	// Image img = Image.GetInstance("путь_к_изображению");
            //	// document.Add(img);

            //}
            //catch (DocumentException dex)
            //{
            //	// Обработка ошибок
            //	Console.Error.WriteLine(dex.Message);
            //}
            //finally
            //{
            //	// Закрытие документа
            //	document.Close();
            //}


            // Создание FileResult из временного файла и его возвращение
            //    byte[] fileBytes = System.IO.File.ReadAllBytes(tempFilePath);
            //string fileName = "document.pdf"; // Имя файла PDF
            //return new FileContentResult(fileBytes, "application/pdf")
            //{
            //    FileDownloadName = fileName
            //};
        }
	}
}
