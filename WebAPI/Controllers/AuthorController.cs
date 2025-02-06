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
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        //LOGGER
        private readonly ILogRepository _logger;

        public AuthorController(IAuthorRepository authorRepository, ILogRepository logger)
        {
            _authorRepository = authorRepository;
            _logger = logger;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(AuthorDTO authorDTO)
        {
            var log = new Log();

            if (!ModelState.IsValid)
            {
                return BadRequest("Some properties are not valid");
            }

            var response = await _authorRepository.Create(authorDTO);
            
            if (response.Status == ResultStatus.Success)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 1,
                    LogMessage = "Author created successfully"

                };
                await _logger.CreateLog(log);
                return Ok(response);
            }

            log = new Log
            {
                CreatedTime = DateTime.Now,
                LogLevel = 3,
                LogMessage = "Author has not been created"

            };
            await _logger.CreateLog(log);
            return BadRequest(response);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var log = new Log();
            var response = await _authorRepository.Delete(id);
            if (response.Status == ResultStatus.NotFound)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 2,
                    LogMessage = "Author could not be found"

                };
                await _logger.CreateLog(log);
                return NotFound(response);
            }
            else if (response.Status == ResultStatus.Error)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 3,
                    LogMessage = "Something went wrong"

                };
                await _logger.CreateLog(log);
                return BadRequest(response);
            }
            log = new Log
            {
                CreatedTime = DateTime.Now,
                LogLevel = 1,
                LogMessage = "Author created successfully"

            };
            await _logger.CreateLog(log);
            return Ok(response);
        }

        [HttpGet("ReadById")]
        public async Task<IActionResult> ReadbyId(int id)
        {
            var log = new Log();
            var response = await _authorRepository.ReadByID(id);
            if (response == null)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 2,
                    LogMessage = "Athor cannot be found"

                };
                await _logger.CreateLog(log);
                return NotFound("Author cannot be found");
            }
            else if (response.AuthorId == 0)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 3,
                    LogMessage = "Something went wrong"

                };
                await _logger.CreateLog(log);

                return BadRequest("There is an error");
            }
            log = new Log
            {
                CreatedTime = DateTime.Now,
                LogLevel = 1,
                LogMessage = "Author created successfully"

            };
            await _logger.CreateLog(log);
            return Ok(response);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(AuthorDTO authorDTO)
        {
            var log = new Log();
            if (!ModelState.IsValid)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 3,
                    LogMessage = "Some properties are not valid"

                };
                await _logger.CreateLog(log);
                return BadRequest("Some properties are not valid");
            }

            var response = await _authorRepository.Update(authorDTO);

            if (response == null)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 2,
                    LogMessage = "Could not found"

                };
                await _logger.CreateLog(log);
                return NotFound("Author cannot be found");
            }
            else if (response.AuthorId == 0)
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
                LogMessage = "Author updated successfully"

            };
            await _logger.CreateLog(log);
            return Ok(response);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var log = new Log();
            var result = await _authorRepository.GetAll();

            if (result.Count > 0)
            {
                log = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 1,
                    LogMessage = "Author fetched successfully"

                };
                await _logger.CreateLog(log);
                return Ok(result);
            }
            log = new Log
            {
                CreatedTime = DateTime.Now,
                LogLevel = 3,
                LogMessage = "Non existant author"

            };
            await _logger.CreateLog(log);
            return NotFound("There is no author");
        }
    }
}
