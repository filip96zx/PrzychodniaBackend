using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Tenis.Core
{
    public class Result
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>()
            {
                Success = true,
                Value = value
            };
        }

        public static Result<T> Error<T>(string message)
        {
            return new Result<T>
            {
                Success = false,
                ErrorMessage = message
                //new List<ErrorMessage>()
                //{
                //    new ErrorMessage()
                //    {
                //        PropertyName = "message",
                //        Message = message
                //    }
                //}
            };
        }

        public static Result<T> Error<T>(IEnumerable<IdentityError> identityErrors)
        {
            return new Result<T>
            {
                Success = false,
                ErrorMessage = string.Join(", ", identityErrors.Select(x => x.Description))
                //identityErrors.Select(i => new ErrorMessage()
                //{
                //    PropertyName = i.Code,
                //    Message = i.Description
                //})
            };
        }
    }

    public class Result<T> : Result
    {
        public T Value { get; set; }
    }

    public class ErrorMessage
    {
        public string PropertyName { get; set; }
        public string Message { get; set; }
    }
}
