using System.Net;

namespace Core.Rest
{
    public class RestResult<TData> : ResultBase<TData>
    {
        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccessStatusCode
        {
            get
            {
                if (StatusCode >= HttpStatusCode.OK)
                {
                    return StatusCode <= (HttpStatusCode) 299;
                }

                return false;
            }
        }

        public RestResult(string code, string message) 
            : base(code, message)
        {
        }

        public RestResult(string message) 
            : this(null, message)
        {
        }
        
        public RestResult()
        {
        }
    }
}