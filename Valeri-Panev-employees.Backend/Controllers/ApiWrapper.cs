using System.Collections.Generic;

namespace Valeri_Panev_employees.Backend.Controllers
{
    public class ApiWrapper<T>
    {
	    public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();
        public bool Success => Errors.Count == 0;

        public ApiWrapper() { }

        public ApiWrapper(T data)
        {
            Data = data;
        }

        public ApiWrapper(string error)
        {
            Errors = new List<string> { error };
        }

        public ApiWrapper(List<string> errors)
        {
            Errors = errors;
        }

        public static ApiWrapper<T> CreateSuccess(T data)
        {
            return new ApiWrapper<T>(data);
        }

        public static ApiWrapper<T> CreateError(string error)
        {
            return new ApiWrapper<T> { Errors = new List<string> { error } };
        }
    }
}