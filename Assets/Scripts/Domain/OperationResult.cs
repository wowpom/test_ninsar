using System;

namespace Game.Domain
{
    public readonly struct OperationResult<TValue>
    {
        public readonly TValue Value;
        public readonly string Error;
        public readonly Exception Cause;

        private OperationResult(TValue value, string error, Exception cause)
        {
            Value = value;
            Error = error;
            Cause = cause;
        }

        public bool Success => Error == null;

        public static OperationResult<TValue> Ok(TValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return new OperationResult<TValue>(value, null, null);
        }

        public static OperationResult<TValue> Fail(string error)
        {
            return Fail(error, null);
        }

        public static OperationResult<TValue> Fail(string error, Exception cause)
        {
            if (string.IsNullOrWhiteSpace(error))
            {
                throw new ArgumentException("Пустое описание ошибки передавать нельзя.", nameof(error));
            }

            return new OperationResult<TValue>(default, error, cause);
        }
    }
}
