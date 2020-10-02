using System.Net.Http;
using System.Threading.Tasks;
using Core.Rest;

namespace Core.Interfaces
{
    public interface IRestClient
    {
        Task<RestResult<string>> ExecuteAsync(string absoluteUriPath, HttpMethod httpMethod, object entity = null,
            HttpContentType? httpContentType = null);
    }
}