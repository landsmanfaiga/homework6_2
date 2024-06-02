using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homework6_1.Data
{
    public class TaskRepo
    {
        private readonly string _connectionString;
        public TaskRepo(string connectionString)
        {
             _connectionString = connectionString;
        }

        public List<TaskItem> GetTasks()
        {
            var context = new taskDataContext(_connectionString);
            return context.Tasks.Include(t => t.User).ToList();
        }

        public void DeleteTask(TaskItem task)
        {
            var context = new taskDataContext(_connectionString);
            context.Tasks.Remove(task);
            context.SaveChanges();
        }

        public void AddTask(TaskItem task)
        {
            var context = new taskDataContext(_connectionString);
            context.Tasks.Add(task);
            context.SaveChanges();
        }

        public void UpdateTask(TaskItem task)
        {
            var context = new taskDataContext(_connectionString);
            context.Tasks.Update(task);
            context.SaveChanges();
        }
    }
}
