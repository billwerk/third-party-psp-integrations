using System.Net.Http;
using System.Text;
using Core.Interfaces;

namespace Core.Rest
{
    public class JsonStringContentFactory : IHttpContentFactory
    {
        public HttpContentType Type => HttpContentType.JsonStringContent;
        
        public HttpContent Create(object source)
        {
            return new StringContent(source as string, Encoding.UTF8, "application/json");
        }
    }
}