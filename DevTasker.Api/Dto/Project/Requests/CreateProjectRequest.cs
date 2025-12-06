using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DevTasker.Domain.Dto.Project.Requests
{
    public class CreateProjectRequest
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
