using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Web;
using ToDoList.DataAccess;
using ToDoList.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Hosting;

namespace ToDoList.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
   // [DisableCors]
    public class ToDoService : IHostedService
    {
        private readonly ToDoContext _toDoContext;
        private readonly ILogger<ToDoService> _logger;
        private readonly HttpContext _ass;

        public ToDoService(ILogger<ToDoService> logger, ToDoContext toDoContext, IHttpContextAccessor accessor)
        {
            _logger = logger;
            _toDoContext = toDoContext;
            //_htContext = httpContext;
           _ass = accessor.HttpContext;

        }

        //[HttpGet("{userId}")]
        public IEnumerable<ToDoDto> GetAllToDos(string userId)
        {
            try
            {
                var todos = new List<ToDoDto>();
                foreach (var todo in _toDoContext.ToDoSet.Where(x => x.UserId == userId))
                {
                    todos.Add(new ToDoDto(todo));
                }
                return todos;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw e;
            }

        }

        //[HttpGet("{userId}/{date:datetime}")]
        public IEnumerable<ToDoDto> GetWeekToDos(DateTime date, string userId)
        {
            try
            {
                date = date.Subtract(TimeSpan.FromDays((int)date.DayOfWeek)).Date;
                var lastDayOfWeek = date.AddDays(7);
                var todos = new List<ToDoDto>();
                var weekTodo = _toDoContext.ToDoSet.Where(x => x.Date >= date && x.Date <= lastDayOfWeek && x.UserId == userId).AsEnumerable();
                foreach (var todo in weekTodo)
                {
                    var dto = new ToDoDto(todo);
                    _ass.Session.Set(dto.ToDoGuid.ToString(), BitConverter.GetBytes(todo.Id));
                    todos.Add(dto);
                }
                return todos;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw e;
            }
        }

        //[HttpPut]
       // [HttpPost("Edit")]
        public void EditToDo(ToDoDto model)
        {
            try
            {
                var entry = _toDoContext.Find<ToDoModel>(BitConverter.ToInt64(_ass.Session.Get(model.ToDoGuid.ToString())));
                if (entry != null)
                {
                    entry.Date = model.Date;
                    entry.Name = model.Name;
                    entry.Description = model.Description;
                    _toDoContext.ToDoSet.Update(entry);
                    _toDoContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw e;
            }
        }

        //[HttpDelete("{uid:guid}")]
        public void DeleteToDo(Guid uid)
        {
            try
            {
                var entry = _toDoContext.Find<ToDoModel>(BitConverter.ToInt64(_ass.Session.Get(uid.ToString())));
                if (entry != null)
                {
                    _toDoContext.ToDoSet.Remove(entry);
                    _toDoContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw e;
            }
        }


        //[HttpPost("{userId}")]
        public void AddToDo(ToDoDto model, string userId)
        {
            try
            {
                //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _toDoContext.ToDoSet.Add(new ToDoModel(model, userId));
                _toDoContext.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw e;
            }

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {

        }
    }
}
