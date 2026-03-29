
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace letiahomes.Application.Common
{
    public class ApiResult<T>
    {
        private readonly T _value;
        public CustomError Error { get; }
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;

        private ApiResult(T value )
        {
            _value = value;
            IsSuccess = true;
            Error = CustomError.None;
        }
        private ApiResult(CustomError error)
        {
            if (error == CustomError.None)
            {
                throw new ArgumentException("invalid error", nameof(error));
            }
            IsSuccess = false;
            Error = error;
        }

        public T Value
        {
            get
            {
                if (IsFailure)
                {
                    throw new InvalidOperationException("there is no value for failure");
                }
                return _value!;
            }
            private init => _value = value;
        }
        public static ApiResult<T> Success(T value)
        {
            return new ApiResult<T>(value);
        }
        public static ApiResult<T> Failure(CustomError error)
        {
            return new ApiResult<T>(error);
        }
    }
}
