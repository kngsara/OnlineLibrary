using Microsoft.AspNetCore.Mvc;
using Shared;
using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;

namespace WebApp.Controllers
{
    public class MemberController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MemberController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = _httpContextAccessor.HttpContext.Session.GetString("Username");
            if (user == "admin")
            {
                return Unauthorized("Page not accessible for admin");
            }
            var client = _httpClientFactory.CreateClient("BaseApi");
            var response = await client.GetAsync($"Member/ReadById?id={id}");


            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound("Member could not be found");
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest("Some error happened");
            }

            var member = await response.Content.ReadFromJsonAsync<MemberDTO>();

            return View(member);


        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            registerDTO.Lozinka = HashPassword(registerDTO.Lozinka);

            var client = _httpClientFactory.CreateClient("BaseApi");
            var response = await client.PostAsJsonAsync("Member/Register", registerDTO);
            var responseObject = await response.Content.ReadFromJsonAsync<Response>();

            ViewBag.Response = responseObject;
            return View();

        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]  
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            loginDTO.Lozinka = HashPassword(loginDTO.Lozinka);

            var client = _httpClientFactory.CreateClient("BaseApi");
            var response = await client.PostAsJsonAsync("Member/Login", loginDTO);
            
            if (response.IsSuccessStatusCode)
            {
                var responseObject = await response.Content.ReadFromJsonAsync<MemberDTO>();
                _httpContextAccessor.HttpContext.Session.SetInt32("Member", responseObject.MemberId);
                _httpContextAccessor.HttpContext.Session.SetString("Username", responseObject.Username);

                return RedirectToAction("Index", "Home");
            }

            var responseError = await response.Content.ReadFromJsonAsync<Response>();
            ViewBag.Message = responseError.Message;   
          
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var client = _httpClientFactory.CreateClient("BaseApi");
            var response = await client.GetAsync($"Member/ReadById?id={id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound("Could not be found");
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest("Some error happened");
            }
            var member = await response.Content.ReadFromJsonAsync<MemberDTO>();

            return View(member);
        }
        [HttpPut]
        public async Task<MemberDTO> Update(MemberDTO memberDTO)
        {
            var client = _httpClientFactory.CreateClient("BaseApi");
            var response = await client.PutAsJsonAsync("Member/Update", memberDTO);

            if (response.IsSuccessStatusCode)
            {
                var member=await response.Content.ReadFromJsonAsync<MemberDTO>();
                _httpContextAccessor.HttpContext.Session.Remove("Username"); 
                _httpContextAccessor.HttpContext.Session.SetString("Username", member.Username);
                return member;
            }

            var responseMessage = await response.Content.ReadAsStringAsync();

            ViewBag.Response = responseMessage;
            return null;
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {

            var client = _httpClientFactory.CreateClient("BaseApi");
            var response = await client.DeleteAsync($"Member/Delete?id={id}");
            var responseObject = await response.Content.ReadFromJsonAsync<Response>();

            return RedirectToAction("Logout");


        }

        [HttpGet]
        public IActionResult Message(string message)
        {
            ViewBag.Response = message;
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
            return View("Login");

        }

        static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);

                //array to hexadecimal str
                StringBuilder result = new StringBuilder();
                foreach (byte b in hash)
                {
                    result.Append(b.ToString("x2"));
                }
                return result.ToString();
            }
        }
    }
}
