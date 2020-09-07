using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACS.Models
{
    public class ResponseModel<T>
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new List<string> { };
        public T Body { get; set; }

        public static ResponseModel<T> SuccessResponse(T body)
        {
            return new ResponseModel<T> { Success = true, Body = body };
        }
        public static ResponseModel<T> ErrorResponse(T body)
        {
            return new ResponseModel<T> { Success = false, Body = body };
        }
        public static ResponseModel<T> ErrorResponse(T body, List<string> errors)
        {
            var response = ErrorResponse(body);
            response.Errors = errors;
            return response;
        }
    }
}
