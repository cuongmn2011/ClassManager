// File: src/Application/DTOs/Common/ApiResponse.cs
using System.Text.Json.Serialization;

namespace Application.DTOs.Common
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; private set; }
        public bool IsSuccess { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Result { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Errors { get; private set; }

        // Private constructor to enforce using static factory methods
        private ApiResponse(int statusCode, bool isSuccess)
        {
            StatusCode = statusCode;
            IsSuccess = isSuccess;
        }

        // --- Static factory methods for creating successful responses ---
        public static ApiResponse<T> Success(T result, int statusCode = 200)
        {
            return new ApiResponse<T>(statusCode, true) { Result = result };
        }

        // --- Static factory methods for creating failure responses ---
        public static ApiResponse<T> Fail(string error, int statusCode = 400)
        {
            return new ApiResponse<T>(statusCode, false) { Errors = new List<string> { error } };
        }

        public static ApiResponse<T> Fail(List<string> errors, int statusCode = 400)
        {
            return new ApiResponse<T>(statusCode, false) { Errors = errors };
        }
    }
}