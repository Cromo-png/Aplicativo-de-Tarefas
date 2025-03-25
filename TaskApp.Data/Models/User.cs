using Microsoft.AspNetCore.Identity;

namespace TaskApp.Data.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        
        // Navigation property for tasks assigned to this user
        public virtual ICollection<TodoTask> AssignedTasks { get; set; } = new List<TodoTask>();
    }
} 