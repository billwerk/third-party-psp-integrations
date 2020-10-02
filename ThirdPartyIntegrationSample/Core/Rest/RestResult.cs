using System.Net;

namespace Core.Rest
{
    public class RestResult<TResult>
    {
        public TResult Result { get; set; }

        public Error Error { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public bool HasErrors => Error != null;

        public bool IsSuccessStatusCode
        {
            get
            {
                if (StatusCode >= HttpStatusCode.OK)
                {
                    return StatusCode <= (HttpStatusCode)299;
                }
                return false;
            }
        }

        public RestResult()
        {

        }

        public RestResult(string code, string message)
        {
            Error = new Error(code, message);
        }

        public RestResult(string message)
            : this(null, message)
        {

        }
    }
}