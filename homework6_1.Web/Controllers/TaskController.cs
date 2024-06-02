using homework6_1.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace homework6_1.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IHubContext<TaskHub> _hub;
        public TaskController(IConfiguration configuration, IHubContext<TaskHub> hub)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
            _hub = hub;
        }

        
        [HttpGet]
        [Route("getall")]
        public List<TaskItem> GetTasks()
        {
            var repo = new TaskRepo(_connectionString);
            return repo.GetTasks();
        }

        [HttpPost]
        [Route("deletetask")]
        public void DeleteTask(TaskItem task)
        {
            var repo1 = new UserRepo(_connectionString);
            int userId = repo1.GetByEmail(User.Identity.Name).Id;
            if(!(userId == task.UserId))
            {
                return;
            }
            var repo = new TaskRepo(_connectionString);
            repo.DeleteTask(task);
            _hub.Clients.All.SendAsync("taskUpdate", repo.GetTasks().ToList());
        }

        [HttpPost]
        [Route("addtask")]
        public void AddTask(TaskItem task)
        {           
            var repo = new TaskRepo(_connectionString);
            repo.AddTask(task);
            _hub.Clients.All.SendAsync("newTask", task);
        }

        [HttpPost]
        [Route("updatetask")]
        public void UpdateTask(TaskItem task)
        {
            var repo1 = new UserRepo(_connectionString);
            task.UserId = repo1.GetByEmail(User.Identity.Name).Id;
            var repo = new TaskRepo(_connectionString);
            repo.UpdateTask(task);
            _hub.Clients.All.SendAsync("taskUpdate", repo.GetTasks().ToList());
        }


    }
}
