using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAPI.Models;
using WebAPI.Repository;
using Shared;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoanRepository _loanRepository;
        //LOGGER
        private readonly ILogRepository _logger;

        public LoanController(ILoanRepository loanRepository, ILogRepository logger)
        {
            _loanRepository = loanRepository;
            _logger = logger;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(LoanDTO loanDTO)
        {
            var log = new Log();
            if (!ModelState.IsValid)
            {
                return BadRequest("Some properties are not valid");
            }

            var response = await _loanRepository.Create(loanDTO);

            if (response.Status == ResultStatus.Success)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 1,
                    LogMessage = "Loan created successfully"

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
                    LogMessage = "Loan creation error"

                };
                await _logger.CreateLog(log);

                return BadRequest(response);
            }

            log = new Log
            {
                CreatedTime = DateTime.Now,
                LogLevel = 2,
                LogMessage = "Loan could not be found"

            };
            await _logger.CreateLog(log);
            return NotFound(response);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var log = new Log();
            var loans = await _loanRepository.GetAll();

            if (loans.Count > 0)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 1,
                    LogMessage = "Loan fetched successfully"

                };
                await _logger.CreateLog(log);
                return Ok(loans);
            }
            log = new Log
            {
                CreatedTime = DateTime.Now,
                LogLevel = 2,
                LogMessage = "Loans not found"

            };
            await _logger.CreateLog(log);
            return NotFound("There are no loans");
        }

        [HttpPut("ReturnBook")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var log = new Log();
            var response = await _loanRepository.ReturnBook(id);

            if (response.Status == ResultStatus.Success)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 1,
                    LogMessage = "Book returned successfully"

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
                    LogMessage = "Book return error"

                };
                await _logger.CreateLog(log);
                return BadRequest(response);
            }
            log = new Log
            {
                CreatedTime = DateTime.Now,
                LogLevel = 2,
                LogMessage = "Book for return not found"

            };
            await _logger.CreateLog(log);
            return NotFound(response);
        }

        [HttpGet("GetLoanByMember")]
        public async Task<IActionResult> GetLoanByMember(int id)
        {
            var log = new Log();
            var loans = await _loanRepository.GetLoanByMember(id);

            if (loans.Count > 0)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 1,
                    LogMessage = "Loans found"

                };
                await _logger.CreateLog(log);
                return Ok(loans);
            }
            log = new Log
            {
                CreatedTime = DateTime.Now,
                LogLevel = 2,
                LogMessage = "Loans not found"

            };
            await _logger.CreateLog(log);
            return NotFound("There are no loans");
        }
    }
}
