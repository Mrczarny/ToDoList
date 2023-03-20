using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.Linq;
using ToDoList.DataAccess;
using ToDoList.Models;
using System.Net.Http;

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
                return todos;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Problem();
            }

        }

        [HttpGet("{date:datetime}")]
        public ActionResult<IEnumerable<ToDoDto>> GetWeekToDos(DateTime date)
        {
            try
            {
                date = date.Subtract(TimeSpan.FromDays((int) date.DayOfWeek)).Date;
                var lastDayOfWeek = date.AddDays(7);
                var todos = new List<ToDoDto>();
                var weekTodo = _context.ToDoSet.Where(x => x.Date >= date && x.Date <= lastDayOfWeek).AsEnumerable();
                foreach (var todo in weekTodo)
                {
                    var dto = new ToDoDto(todo);
                    HttpContext.Session.Set(dto.ToDoGuid.ToString(), BitConverter.GetBytes(todo.Id));
                    todos.Add(dto);
                }
                return todos;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Problem();
            }
        }

        [HttpPut]
        [HttpPost("Edit")]
        public ActionResult EditToDo(ToDoDto model)
        {
            try
            {
                var entry = _context.Find<ToDoModel>(BitConverter.ToInt64(HttpContext.Session.Get(model.ToDoGuid.ToString())));
                if (entry != null)
                {
                    entry.Date = model.Date;
                    entry.Name = model.Name;
                    entry.Description = model.Description;
                    _context.ToDoSet.Update(entry);
                    _context.SaveChanges();
                    return Ok();
                }
                return NotFound("ToDo not found");
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Problem();
            }
        }

        [HttpDelete("{uid:guid}")]
        public ActionResult DeleteToDo(Guid uid)
        {
            try
            {
                var entry = _context.Find<ToDoModel>(BitConverter.ToInt64(HttpContext.Session.Get(uid.ToString())));
                if (entry != null)
                {
                    _context.ToDoSet.Remove(entry);
                    _context.SaveChanges();
                    return Ok();
                }

                return NotFound("ToDo not found");
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
