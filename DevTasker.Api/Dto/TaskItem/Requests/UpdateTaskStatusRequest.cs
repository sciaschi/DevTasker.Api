using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DevTasker.Api.Dto.TaskItem.Requests
{
    public class UpdateTaskStatusRequest
    {
        [Required(ErrorMessage = "status is required.")]
        public int Status { get; set; }
    }
}
