using System.Threading.Tasks;
using Billwerk.Payment.PayOne.Model.Requests;
using Billwerk.Payment.SDK.Rest;

namespace Billwerk.Payment.PayOne.Interfaces
{
    public interface IPayOneWrapper
    {
        Task<RestResult<string>> ExecutePayOneRequestAsync(RequestBase requestDto);
    }
}