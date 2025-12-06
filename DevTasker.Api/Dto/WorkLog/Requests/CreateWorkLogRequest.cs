using DevTasker.Domain.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace DevTasker.Domain.Dto.WorkLog.Requests
{
    public class CreateWorkLogRequest
    {
        [Required(ErrorMessage = "task_item_id is required.")]
        [JsonPropertyName("task_item_id")]
        public int TaskItemId { get; set; }

        [JsonPropertyName("started_at")]
        public DateTime? StartedAt { get; set; }

        [JsonPropertyName("ended_at")]
        public DateTime? EndedAt { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }
    }
}
