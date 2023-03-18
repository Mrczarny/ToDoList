using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using ToDoList.DataAccess;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly ToDoContext _context;
        private readonly ILogger<ToDoController> _logger;
        public ToDoController(ILogger<ToDoController> logger, ToDoContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ToDoDto>> GetAllToDos()
        {
            try
            {
                var todos = new List<ToDoDto>();
                foreach (var todo in _context.ToDoSet)
                {
                    todos.Add(new ToDoDto(todo));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Problem();
            }

        }

        [HttpPost]
        public ActionResult AddToDo(ToDoDto model)
        {
            try
            {
                _context.ToDoSet.Add(new ToDoModel(model));
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Problem();
            }

        }
    }
}
