using System;
using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [StringLength(20)]
        public string Priority { get; set; } = "Medium";

        [Display(Name = "Completed?")]
        public bool IsDone { get; set; }
        public string? UserId { get; set; }
    }
}
