using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared;
using System.Net;

namespace WebApp.Controllers
{
    public class LoanController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoanController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        { //check ako je admin
            var user = _httpContextAccessor.HttpContext.Session.GetString("Username");
            if (user != "admin")
            {
                return Unauthorized("You have to be admin to access this page");
            }
            var client = _httpClientFactory.CreateClient("BaseApi");
            var response = await client.GetAsync("Loan/GetAll");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound("Could not be found");
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest("Some error happened");
            }
            var loans = await response.Content.ReadFromJsonAsync<List<LoanDTO>>();

            return View(loans);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            var loanDTO = new LoanDTO
            {
                BookId = id,
                MemberId = _httpContextAccessor.HttpContext.Session.GetInt32("Member")
            };
            var client = _httpClientFactory.CreateClient("BaseApi");
            var response = await client.PostAsJsonAsync("Loan/Create", loanDTO);
            var responseObject = await response.Content.ReadFromJsonAsync<Response>();

            TempData["Loan"] = JsonConvert.SerializeObject(responseObject);

            return RedirectToAction("Details", "Book", new {id = id});
        }
        [HttpGet]
        public async Task<IActionResult> MyLoans()
        {
            var memberId = _httpContextAccessor.HttpContext.Session.GetInt32("Member");
            if (memberId == null)
            {
                return NotFound("Member not found");
            }

            var client = _httpClientFactory.CreateClient("BaseApi");
            var response = await client.GetAsync($"Loan/GetLoanByMember?id={memberId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound("Could not be found");
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest("Some error happened");
            }
            
            var loans = await response.Content.ReadFromJsonAsync<List<LoanDTO>>();

            if (TempData["ReturnBook"] != null)
            {
                var responseJson = TempData["ReturnBook"] as string;
                ViewBag.Response = JsonConvert.DeserializeObject<Response>(responseJson);
            }

            return View(loans);
        }

        [HttpGet]
        public async Task<IActionResult> Return (int id)
        {
            var client = _httpClientFactory.CreateClient("BaseApi");
            var response = await client.PutAsync($"Loan/ReturnBook?id={id}", null);
            var responseObject = await response.Content.ReadFromJsonAsync<Response>();

            TempData["ReturnBook"] = JsonConvert.SerializeObject(responseObject);

            return RedirectToAction("MyLoans");
        }
    }
}
