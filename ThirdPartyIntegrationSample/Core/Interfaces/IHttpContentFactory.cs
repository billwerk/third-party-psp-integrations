using System.Net.Http;
using Billwerk.Payment.SDK.Rest;
using Core.Rest;

namespace Core.Interfaces
{
    public interface IHttpContentFactory
    {
        HttpContentType Type { get; }
        HttpContent Create(object source);
    }
}