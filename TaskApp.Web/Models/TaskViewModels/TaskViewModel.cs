using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace TaskApp.Web.Models
{
    public class TaskViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(100, ErrorMessage = "O {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 3)]
        [Display(Name = "Título")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo {1} caracteres.")]
        [Display(Name = "Descrição")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data de entrega é obrigatória")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Entrega")]
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(7);

        [Required(ErrorMessage = "A dificuldade é obrigatória")]
        [Display(Name = "Dificuldade")]
        public string Difficulty { get; set; } = string.Empty;

        [Display(Name = "Responsável")]
        public string? UserId { get; set; }

        [Display(Name = "Concluída")]
        public bool IsCompleted { get; set; }

        // Para dropdown no formulário
        public SelectList? AssigneeList { get; set; }
        
        // Para exibir o nome do responsável
        public string? AssigneeName { get; set; }
    }

    public class TaskListViewModel
    {
        public IEnumerable<TaskViewModel> Tasks { get; set; } = new List<TaskViewModel>();
    }
} 