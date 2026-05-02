using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sismeing.Service
{
    public class JsonResponse<T>
    {
        private object value;
        private ResponseStatus error;

        [Required]
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ResponseStatus Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public T? Data { get; set; }

        public JsonResponse(T? data = default, string message = "", ResponseStatus status = ResponseStatus.success)
        {
            Status = status;
            Message = message;
            Data = data;
        }

        public JsonResponse(object value, string message, ResponseStatus error)
        {
            this.value = value;
            Message = message;
            this.error = error;
        }
    }

    public enum ResponseStatus
    {
        [EnumMember(Value = "success")]
        success,

        [EnumMember(Value = "error")]
        error,

        [EnumMember(Value = "incomplete")]
        incomplete
    }
}
