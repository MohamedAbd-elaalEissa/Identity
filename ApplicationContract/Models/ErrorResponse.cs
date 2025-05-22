using ApplicationContract.CustomException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationContract.Models
{
    public class ErrorResponse
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
        public string? StackTrace { get; set; }
        public DateTime Timestamp { get; set; }

        public ErrorResponse(Exception ex, HttpStatusCode statusCode, bool includeStackTrace = false)
        {
            Type = ex.GetType().Name;
            Message = ex.Message;
            Status = (int)statusCode;
            Timestamp = DateTime.UtcNow;

            if (ex is ValidationException validationEx)
            {
                Errors = validationEx.Errors;
            }

            if (includeStackTrace)
            {
                StackTrace = ex.StackTrace;
            }
        }
    }
}
