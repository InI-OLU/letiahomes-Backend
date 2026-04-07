using System.Text.Json.Serialization;

namespace letiahomes.Application.Common
{
    public class ApiResult<T>
    {
        private readonly T? _value;

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public CustomError? Error { get; }

        // Serializer sees this — returns value on success, default on failure
        // Never throws — safe for JSON serialization
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data => IsSuccess ? _value : default;

        private ApiResult(T value)
        {
            _value = value;
            IsSuccess = true;
            Error = null;
        }

        private ApiResult(CustomError error)
        {
            if (error == CustomError.None)
                throw new ArgumentException("Invalid error", nameof(error));

            IsSuccess = false;
            Error = error;
            _value = default;
        }

        // Only used internally when you actually need the value in code
        // Never call this without checking IsSuccess first
        [JsonIgnore]
        public T Value
        {
            get
            {
                if (IsFailure)
                    throw new InvalidOperationException(
                        "Cannot access Value on a failed result. Check IsSuccess first.");
                return _value!;
            }
        }

        public static ApiResult<T> Success(T value) => new(value);
        public static ApiResult<T> Failure(CustomError error) => new(error);
    }
}