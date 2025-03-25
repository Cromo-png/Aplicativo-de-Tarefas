using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskApp.Data.Models
{
    public class TodoTask
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
        
        [MaxLength(50)]
        public string Difficulty { get; set; } = string.Empty;
        
        public bool IsCompleted { get; set; } = false;
        
        // Foreign key for User
        public string? UserId { get; set; }
        
        // Navigation property
        [ForeignKey("UserId")]
        public virtual User? Assignee { get; set; }
    }
} 