using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using Core.Interfaces;

namespace Core.Rest
{
    public class FormUrlEncodedContentFactory : IHttpContentFactory
    {
        public HttpContentType Type => HttpContentType.FormUrlEncodedContent;
        
        public HttpContent Create(object source)
        {
            var nameValueCollection = source as NameValueCollection;
            return new FormUrlEncodedContent(nameValueCollection?.AllKeys.Select(k => new KeyValuePair<string, string>(k, nameValueCollection[k])));
        }
    }
}