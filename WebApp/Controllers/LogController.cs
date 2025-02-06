using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared;
using System.Net.Http;

namespace WebApp.Controllers
{
    public class LogController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var user = _httpContextAccessor.HttpContext.Session.GetString("Username");
            if (user != "admin")
            {
                return Unauthorized("You have to be admin to access this page");
            }

            ViewBag.HasData = true;
            return View(new List<LogDTO>());
        }

        [HttpPost]
        public async Task<IActionResult> Index(int n)
        {
            if (n == 0)
            {
                ViewBag.HasData = false;
                return View(new List<LogDTO>());
            }
            var client = _httpClientFactory.CreateClient("BaseApi");
            var response = await client.GetAsync($"Log/GetLogs/{n}");
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<List<LogDTO>>(responseBody);
                return View(responseObject);
            }
            ViewBag.HasData = false;
            return View(new List<LogDTO>());
        }


        [HttpGet]
        public async Task<IActionResult> Count()
        {
            var user = _httpContextAccessor.HttpContext.Session.GetString("Username");
            if (user != "admin")
            {
                return Unauthorized("You have to be admin to access this page");
            }

            var client = _httpClientFactory.CreateClient("BaseApi");
            var response = await client.GetAsync($"Log/GetCountedLogs");

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var responseResult = int.Parse(responseString);
                ViewBag.TotalLogCount = responseResult;
            }
            return View();
        }
    }
}
