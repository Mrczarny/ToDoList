using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using ToDoList.Models;

namespace ToDoList.Pages
{
    [SaveTempData]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ToDoService _toDoService;
        //private readonly HttpClient _httpClient;
        //private readonly UserManager<ToDoListUser> _userManager;


        public IndexModel(ILogger<IndexModel> logger, ToDoService toDoService, UserManager<ToDoListUser> UserManager)
        {
            _logger = logger;
            _toDoService = toDoService;
            //_httpClient = client.CreateClient("baseHttp");
            //_userManager = UserManager;
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(User.)
        }

        [TempData]
        public string ToDosJson { get; set; }

        public List<List<ToDoDto>> ToDos { get; set; }

        
        [BindProperty]
        public ToDoDto? SelectedToDo { get; set; }

        //[TempData]
        //public string SelectedWeekString { get; set; }

        [TempData]
        public DateTime SelectedWeek { get; set; }

        [ValidateNever]
        [BindProperty]
        public DayOfWeek SelectedDay { get; set; }

        [BindProperty]
        public string SelectedGuid { get; set; }

        public async Task<IActionResult> OnGetAsync(DateTime? date)
        {
            try
            {
                if (date != null) SelectedWeek = (DateTime) date;
                if (SelectedWeek == default) SelectedWeek = DateTime.Now - TimeSpan.FromDays((int)DateTime.Now.DayOfWeek);

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var response = _toDoService.GetWeekToDos(SelectedWeek, userId);
                  // await _httpClient.GetFromJsonAsync<IEnumerable<ToDoDto>>($"/api/ToDo/{userId}/{SelectedWeek.Date:yyyy-MM-dd}");


                ToDos = SortToDos(response);
                ToDosJson = JsonConvert.SerializeObject(ToDos);
                TempData.Keep();
                return Page();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                ToDos = new List<List<ToDoDto>>();
                //return Page();
                return RedirectToPage("/Error");
            }

        }


        public IActionResult OnPostNextWeekAsync()
        {
            try
            {
                SelectedWeek = SelectedWeek.AddDays(7);
                return RedirectToPage(SelectedWeek);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return RedirectToPage("/Error");
            }
        }

        public IActionResult OnPostPrevWeekAsync()
        {
            try
            {
                SelectedWeek = SelectedWeek.Subtract(TimeSpan.FromDays(7));
                return RedirectToPage(SelectedWeek);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostChangeDayAsync()
        {
            try
            {
                ToDos = JsonConvert.DeserializeObject<List<List<ToDoDto>>>(ToDosJson);
                if (ToDos != null)
                {
                    if (SelectedWeek == default) SelectedWeek = DateTime.Now - TimeSpan.FromDays((int)DateTime.Now.DayOfWeek);
                    SelectedToDo = (from todoList in ToDos
                        from toDo in todoList
                        where toDo.ToDoGuid == Guid.Parse(SelectedGuid)
                        select toDo).First();
                    var newDate = SelectedWeek.AddDays((int) SelectedDay);
                    SelectedToDo.Date = new DateTime(newDate.Year, newDate.Month, newDate.Day , SelectedToDo.Date.Hour,
                        SelectedToDo.Date.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
                    _toDoService.EditToDo(SelectedToDo);
                    //await _httpClient.PostAsJsonAsync<ToDoDto>("/api/ToDo/Edit", SelectedToDo);
                }
                return RedirectToPage(SelectedWeek);

            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            try
            {
                ToDos = JsonConvert.DeserializeObject<List<List<ToDoDto>>>(ToDosJson);
                if (ToDos != null)
                {
                    TryValidateModel(SelectedToDo);
                    if (ModelState.GetValidationState(nameof(SelectedToDo.Name)) == ModelValidationState.Invalid || ModelState.GetValidationState(nameof(SelectedToDo.Date)) == ModelValidationState.Invalid)
                    {
                        return RedirectToPage(SelectedWeek);
                    }
                    var oldToDo = (from todoList in ToDos
                        from toDo in todoList
                        where toDo.ToDoGuid == SelectedToDo.ToDoGuid
                        select toDo).First();
                    oldToDo.Date = oldToDo.Date.Subtract(new TimeSpan(oldToDo.Date.Hour, oldToDo.Date.Minute, 0));
                    oldToDo.Date = oldToDo.Date.Add(new TimeSpan(SelectedToDo.Date.Hour, SelectedToDo.Date.Minute, 0));
                    SelectedToDo.Date = oldToDo.Date;
                    //await _httpClient.PostAsJsonAsync<ToDoDto>("/api/ToDo/Edit", SelectedToDo);
                    _toDoService.EditToDo(SelectedToDo);
                }
                return RedirectToPage(SelectedWeek);

            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            try
            {
                if (SelectedToDo != default)
                {
                    _toDoService.DeleteToDo(SelectedToDo.ToDoGuid);
                    //await _httpClient.DeleteAsync($"/api/ToDo/{SelectedToDo.ToDoGuid}");
                }
                return RedirectToPage(SelectedWeek);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return RedirectToPage("/Error");
            }
        }

        public IActionResult OnPostSelectToDo()
        {

            ToDos =  JsonConvert.DeserializeObject<List<List<ToDoDto>>>(ToDosJson);
            if (ToDos != null)
            {
                foreach (var todoList in ToDos)
                {
                    if (todoList.Exists(x => x.ToDoGuid.ToString() == SelectedGuid)) SelectedToDo = todoList.Find(x => x.ToDoGuid.ToString() == SelectedGuid);
                }
                SelectedToDo.Date = DateTime.Parse(SelectedToDo.Date.ToString("g"));
                TempData.Keep();
                return Page(); 
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                TryValidateModel(SelectedToDo);
                if (ModelState.GetValidationState(nameof(SelectedToDo)) == ModelValidationState.Invalid || ModelState.GetValidationState(nameof(SelectedToDo.Date)) == ModelValidationState.Invalid)
                {
                    RedirectToPage();
                }
                if (SelectedWeek == default) SelectedWeek = DateTime.Now - TimeSpan.FromDays((int)DateTime.Now.DayOfWeek);
                var selectedDate = SelectedWeek.AddDays((int) SelectedDay);
                SelectedToDo.Date = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, SelectedToDo.Date.Hour,
                    SelectedToDo.Date.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //await _httpClient.PostAsJsonAsync($"/api/ToDo/{userId}", SelectedToDo);
                _toDoService.AddToDo(SelectedToDo, userId);
                return RedirectToPage(SelectedWeek);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return RedirectToPage("/Error");
            }

        }

        private List<List<ToDoDto>> SortToDos(IEnumerable<ToDoDto> toDos)
        {
            var result = new List<List<ToDoDto>>();
            foreach (DayOfWeek weekDay in (DayOfWeek[])Enum.GetValues(typeof(DayOfWeek)))
            {
                result.Add(toDos.Where(x => x.Date.DayOfWeek == weekDay).ToList());
            }
            return result;
        }
    }
}
