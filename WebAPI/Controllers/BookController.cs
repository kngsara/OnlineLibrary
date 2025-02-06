using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Repository;
using Shared;
using Microsoft.Extensions.Logging;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        //injection of interface
        private readonly IBookRepository _bookRepository;
        //LOGGER
        private readonly ILogRepository _logger;

        public BookController(IBookRepository bookRepository, ILogRepository logger)
        {
            _bookRepository = bookRepository;
            _logger = logger;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(BookDTO bookDTO)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest("Some properties are not valid");
            }
            var response = await _bookRepository.Create(bookDTO);
            var log = new Log();

            if (response.Status == ResultStatus.Success)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 1,
                    LogMessage = "Book created successfully"

                };
                await _logger.CreateLog(log);
                return Ok(response);
            }
            else if(response.Status == ResultStatus.Error)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 1,
                    LogMessage = "Book cannot be created"

                };
                await _logger.CreateLog(log);
                return BadRequest(response);
            }
            log = new Log
            {
                CreatedTime = DateTime.Now,
                LogLevel = 1,
                LogMessage = "Request not found"

            };
            await _logger.CreateLog(log);
            return NotFound(response);
        }

        [HttpGet("ReadById")]
        
        public async Task<IActionResult> ReadById(int id)
        {
            var log = new Log();
            var response = await _bookRepository.ReadById(id);

            if (response == null)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 2,
                    LogMessage = "Book cannot be found"

                };
                await _logger.CreateLog(log);
                return NotFound("Book cannot be found");
            }
            else if (response.BookId == 0)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 3,
                    LogMessage = "There is an error"

                };
                await _logger.CreateLog(log);
                return BadRequest("There is an error");
            }
            log = new Log
            {
                CreatedTime = DateTime.Now,
                LogLevel = 1,
                LogMessage = "Book fetched successfully"

            };
            await _logger.CreateLog(log);
            return Ok(response);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(BookDTO bookDTO)
        {
            var log = new Log();
            if (!ModelState.IsValid)
            {
                return BadRequest("Some properties are not valid");
            }

            var response = await _bookRepository.Update(bookDTO);

            if (response == null)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 2,
                    LogMessage = "Book cannot be found"

                };
                await _logger.CreateLog(log);
                return NotFound("Book cannot be found");
            }
            else if(response.BookId == 0)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 3,
                    LogMessage = "Error happened"

                };
                await _logger.CreateLog(log);
                return BadRequest("There is an error");
            }
            log = new Log
            {
                CreatedTime = DateTime.Now,
                LogLevel = 1,
                LogMessage = "Book updated successfully"

            };
            await _logger.CreateLog(log);
            return Ok(response);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var log = new Log();
            var response = await _bookRepository.Delete(id);

            if(response.Status == ResultStatus.NotFound)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 2,
                    LogMessage = "Could not be found"

                };
                return NotFound(response);
            }
            else if(response.Status == ResultStatus.Error)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 3,
                    LogMessage = "An error happened"

                };
                return BadRequest(response);
            }
            log = new Log
            {
                CreatedTime = DateTime.Now,
                LogLevel = 1,
                LogMessage = "Book deleted successfully"

            };
            return Ok(response);
        }

        [HttpGet("ReadAll")]
        public async Task<IActionResult> ReadAll([FromQuery]RequestDTO requestDTO)
        {
            var log = new Log();
            var response = await _bookRepository.ReadAll(requestDTO);
            if (response.TotalCount > 0)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 1,
                    LogMessage = "Books fetched successfully"

                };
                return Ok(response);
            }
            else if(response.TotalCount == 0)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 3,
                    LogMessage = "Book fetch failed"

                };
                return BadRequest(response);
            }
            log = new Log
            {
                CreatedTime = DateTime.Now,
                LogLevel = 2,
                LogMessage = "Book could not be found"

            };
            return NotFound(response);
        }
    }
}
