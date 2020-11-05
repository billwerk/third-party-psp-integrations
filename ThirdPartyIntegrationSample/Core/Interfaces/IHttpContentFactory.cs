using System.Net.Http;
using Billwerk.Payment.SDK.Enums;

namespace Core.Interfaces
{
    public interface IHttpContentFactory
    {
        HttpContentType Type { get; }
        HttpContent Create(object source);
    }
}