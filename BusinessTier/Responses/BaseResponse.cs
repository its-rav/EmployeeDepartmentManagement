using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessTier.Responses
{
    public class BaseResponse<T> where T : class
    {
        public T Data { get; set; }
        public string Message { get; set; }
    }
}
