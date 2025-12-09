using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace DevTasker.Api.Dto.TaskItem.Requests
{
    public class UpdateTaskPriorityRequest
    {
        [Required(ErrorMessage = "status is required.")]
        [JsonPropertyName("task_id")]
        public int taskId { get; set; }

        [Required(ErrorMessage = "status is required.")]
        public int priority { get; set; }
    }
}
