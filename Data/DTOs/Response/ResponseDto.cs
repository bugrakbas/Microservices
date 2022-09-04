using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs.Response
{
    public class ResponseDto<T> where T : class
    {
        public string Message { get; set; }
        public T Data { get; set; }
        public HttpStatusCode Status { get; set; }
        public bool IsSuccess { get; set; }
    }
}
