using System;
using Newtonsoft.Json;

namespace ExtratosApi.Models 
{
    public class ResponseDetails 
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public override string ToString() 
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
