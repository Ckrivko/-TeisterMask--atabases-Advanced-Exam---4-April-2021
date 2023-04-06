using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TeisterMask.Data.Models
{
    public class EmployeeTask
    {        

        [ForeignKey(nameof(EmployeeId))]
        [Required]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [ForeignKey(nameof(TaskId))]
        [Required]
        public int TaskId { get; set; }

        public Task Task { get; set; }

    }
}
