using DevTasker.Domain.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace DevTasker.Api.Dto.TaskItem.Requests
{
    public class UpdateTaskItemRequest
    {
        [Required(ErrorMessage = "id is required.")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("status")]
        public TaskItemStatus Status { get; set; }

        [JsonPropertyName("priority")]
        public TaskItemPriority Priority { get; set; }

        [JsonPropertyName("due_date")]
        public DateTime? DueDate { get; set; }

        [JsonPropertyName("completed_at")]
        public DateTime? CompletedAt { get; set; }
    }
}
