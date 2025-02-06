using Microsoft.AspNetCore.Mvc;
using Shared;
using System.Net;

namespace WebApp.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient("BaseApi");
            var response = await client.GetAsync($"Author/ReadById?id={id}");
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound("Author not found");
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest("Error happened");
            }

            var author = await response.Content.ReadFromJsonAsync<AuthorDTO>();

            return View(author);

        }

        [HttpGet]
        public IActionResult Create()
        {
            var user = _httpContextAccessor.HttpContext.Session.GetString("Username");
            if (user != "admin")
            {
                return Unauthorized("You have to be admin to access this page");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AuthorDTO authorDTO)
        {

            var client = _httpClientFactory.CreateClient("BaseApi");
            var response = await client.PostAsJsonAsync("Author/Create", authorDTO);
            var responseObject = await response.Content.ReadFromJsonAsync<Response>(); 
            
            ViewBag.Response = responseObject;
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
            var response = await client.GetAsync($"Author/ReadById?id={id}");

            if(response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound("Author not found");
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest("Error happened");
            }
            var author = await response.Content.ReadFromJsonAsync<AuthorDTO>();

            return View(author);
        }

        [HttpPost]
        public async Task<IActionResult> Update(AuthorDTO authorDTO)
        {
            var client = _httpClientFactory.CreateClient("BaseApi");
            var response = await client.PutAsJsonAsync("Author/Update", authorDTO);

            if(response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details", new {id = authorDTO.AuthorId});
            }

            var responseMessage = await response.Content.ReadAsStringAsync();
            ViewBag.Response = responseMessage;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("BaseApi");
            var response = await client.DeleteAsync($"Author/Delete?id={id}");
            var responseObject = await response.Content.ReadFromJsonAsync<Response>();

            
            ViewBag.Response = responseObject.Message;

            return RedirectToAction("Message", new {message = responseObject.Message});
     

        }

        [HttpGet]
        public IActionResult Message(string message)
        {
            ViewBag.Response = message;
            return View();
        }
    }
}
