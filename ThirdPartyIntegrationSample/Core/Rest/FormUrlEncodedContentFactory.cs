using System.Linq;
using System.Net.Http;
using Core.Extensions;
using Core.Interfaces;

namespace Core.Rest
{
    public class FormUrlEncodedContentFactory : IHttpContentFactory
    {
        public HttpContentType Type => HttpContentType.FormUrlEncodedContent;
        
        public HttpContent Create(object source)
        {
            return new FormUrlEncodedContent(source.ToDictionary<string>().AsEnumerable());
        }
    }
}