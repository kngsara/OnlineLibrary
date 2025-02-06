using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NuGet.Packaging;
using Shared;
using System.Net;

namespace WebApp.Controllers
{
    public class BookController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BookController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]

        public async Task<IActionResult> Index(string? searchTerm, int page)
        {
            var client = _httpClientFactory.CreateClient("BaseApi");

            if (page == 0)
            {
                page = 1;
            }

            var queryString = $"?SearchTerm={Uri.EscapeDataString(searchTerm ?? "")}" + $"&Page={page}";
        
            var response = await client.GetAsync($"Book/ReadAll{queryString}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound("Book not found");
            }
            else if(response.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest("Error happened");
            }
            var books = await response.Content.ReadFromJsonAsync<PaginatedResult>();

            ViewBag.Counter = books.TotalCount;
            ViewBag.SearchTerm = searchTerm;

            return View(books.Data);
        }

        [HttpGet]
        public async Task< IActionResult> Create()
        {
            var user = _httpContextAccessor.HttpContext.Session.GetString("Username");
            if (user != "admin")
            {
                return Unauthorized("You have to be admin to access this page");
            }

            var client = _httpClientFactory.CreateClient("BaseApi");
            var response = await client.GetAsync("Author/GetAll");

            if (response.StatusCode == HttpStatusCode.BadRequest)
            { 
                return BadRequest("Error happened");
            }
            var authors = await response.Content.ReadFromJsonAsync<List<AuthorDTO>>();

            var selectList = new List<SelectListItem>();

            foreach (var author in authors)
            {
                selectList.Add(new SelectListItem
                {
                    Text = author.FullName,
                    Value = author.AuthorId.ToString()
                });

            }

            ViewBag.Authors = selectList;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookDTO bookDTO)
        {
            var client = _httpClientFactory.CreateClient("BaseApi");
            var response = await client.PostAsJsonAsync("Book/Create", bookDTO);
            var responseObject = await response.Content.ReadFromJsonAsync<Response>();

            ViewBag.Response = responseObject;

            var responseAuthors = await client.GetAsync("Author/GetAll");

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest("Not found");
            }
            var authors = await responseAuthors.Content.ReadFromJsonAsync<List<AuthorDTO>>();

            var selectList = new List<SelectListItem>();

            foreach (var author in authors)
            {
                selectList.Add(new SelectListItem
                {
                    Text = author.FullName,
                    Value = author.AuthorId.ToString()
                });

            }

            ViewBag.Authors = selectList;

            return View();  
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var user = _httpContextAccessor.HttpContext.Session.GetString("Username");
            if (user != "admin")
            {
                return Unauthorized("You have to be admin to access this page");
            }

            var client = _httpClientFactory.CreateClient("BaseApi");
            var response = await client.GetAsync($"Book/ReadById?id={id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound("Could not be found");
            }
            else if(response.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest("Some error happened");
            }

            var book = await response.Content.ReadFromJsonAsync<BookDTO>();

            var authorResponse = await client.GetAsync("Author/GetAll");

            if (authorResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest("Error has occured");
            }
            var authors = await authorResponse.Content.ReadFromJsonAsync<List<AuthorDTO>>();

            var selectList = new List<SelectListItem>();

            foreach (var author in authors)
            {
                selectList.Add(new SelectListItem
                {
                    Text = author.FullName,
                    Value = author.AuthorId.ToString()
                });

            }

            ViewBag.Authors = selectList;
            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> Update(BookDTO bookDTO)
        {
            var client = _httpClientFactory.CreateClient("BaseApi");
            var response = await client.PutAsJsonAsync("Book/Update", bookDTO);

            if(response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            var responseObject = await response.Content.ReadFromJsonAsync<BookDTO>();

            ViewBag.Response = responseObject;
            
            return View();  
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = _httpContextAccessor.HttpContext.Session.GetString("Username");
            if (user == null)
            {
                return Unauthorized("You have to be logged in to access this page");
            }

            var client = _httpClientFactory.CreateClient("BaseApi");
            var response = await client.GetAsync($"Book/ReadById?id={id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound("Could not be found");
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest("Some error happened");
            }

            var book = await response.Content.ReadFromJsonAsync<BookDTO>();

            if (TempData["Loan"] != null)
            {
                var responseJson = TempData["Loan"] as string;
                ViewBag.Response = JsonConvert.DeserializeObject<Response>(responseJson);
            }

            return View(book);

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {

            var client = _httpClientFactory.CreateClient("BaseApi");
            var response = await client.DeleteAsync($"Book/Delete?id={id}");
            var responseObject = await response.Content.ReadFromJsonAsync<Response>();

            if (responseObject.Status == ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = responseObject.Message;

            return RedirectToAction("Index");
        }
    }
}
