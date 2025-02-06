using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAPI.Models;
using WebAPI.Repository;
using Shared;
using Response = Shared.Response;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IMemberRepository _memberRepository;
        //LOGGER
        private readonly ILogRepository _logger;

        public MemberController(IMemberRepository memberRepository, ILogRepository logger)
        {
            _memberRepository = memberRepository;
            _logger = logger;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            var log = new Log();
            if (!ModelState.IsValid)
            {
                return BadRequest("Some properties are not valid");
            }

            var response = await _memberRepository.Register(registerDTO);

            if (response.Status == ResultStatus.Success)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 1,
                    LogMessage = "Member created successfully"

                };
                await _logger.CreateLog(log);
                return Ok(response);
            }
            else if (response.Status == ResultStatus.Error)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 3,
                    LogMessage = "Error at registration"

                };
                await _logger.CreateLog(log);
                return BadRequest(response);
            }
            log = new Log
            {
                CreatedTime = DateTime.Now,
                LogLevel = 2,
                LogMessage = "Registration not found"

            };
            await _logger.CreateLog(log);
            return NotFound(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var log = new Log();
            if (!ModelState.IsValid)
            {
                var notFoundRequest = new Response
                {
                    Status = ResultStatus.Error,
                    Message = "There is an error"
                };
            }

            var response = await _memberRepository.Login(loginDTO);

            if (response == null)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 2,
                    LogMessage = "Invalid username or password, not found"

                };
                await _logger.CreateLog(log);

                var notFoundRequest = new Response
                {
                    Status = ResultStatus.NotFound,
                    Message = "Invalid username or password"
                };

                return NotFound(notFoundRequest);
            }
            else if (response.MemberId == 0)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 3,
                    LogMessage = "There is an error"

                };
                await _logger.CreateLog(log);
                var notFoundRequest = new Response
                {
                    Status = ResultStatus.Error,
                    Message = "There is an error"
                };
            }

            return Ok(response);
        }

        [HttpGet("ReadById")]
        public async Task<IActionResult> ReadById(int id)
        {
            var log = new Log();
            var response = await _memberRepository.ReadById(id);

            if (response == null)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 2,
                    LogMessage = "Member not found"

                };
                await _logger.CreateLog(log);
                return NotFound("Member cannot be found");
            }
            else if (response.MemberId == 0)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 3,
                    LogMessage = "There is an error at searching members"

                };
                await _logger.CreateLog(log);
                return BadRequest("There is an error");
            }

            return Ok(response);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(MemberDTO memberDTO)
        {
            var log = new Log();
            if (!ModelState.IsValid)
            {
                return BadRequest("Some properties are not valid");
            }

            var response = await _memberRepository.Update(memberDTO);

            if (response == null)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 2,
                    LogMessage = "Member for update not found"

                };
                await _logger.CreateLog(log);
                return NotFound("Member cannot be found");
            }
            else if (response.MemberId == 0)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 3,
                    LogMessage = "Member updating error"

                };
                await _logger.CreateLog(log);
                return BadRequest("There is an error");
            }
            log = new Log
            {
                CreatedTime = DateTime.Now,
                LogLevel = 1,
                LogMessage = "Member updated successfully"

            };
            await _logger.CreateLog(log);
            return Ok(response);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var log = new Log();
            var response = await _memberRepository.Delete(id);

            if (response.Status == ResultStatus.Success)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 1,
                    LogMessage = "Member deleted successfully"

                };
                await _logger.CreateLog(log);
                return Ok(response);
            }
            else if (response.Status == ResultStatus.Error)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 3,
                    LogMessage = "Member deleting error"

                };
                await _logger.CreateLog(log);
                return BadRequest(response);
            }
            log = new Log
            {
                CreatedTime = DateTime.Now,
                LogLevel = 2,
                LogMessage = "Member for deleting not found"

            };
            await _logger.CreateLog(log);
            return NotFound(response);
        }
    }
}
