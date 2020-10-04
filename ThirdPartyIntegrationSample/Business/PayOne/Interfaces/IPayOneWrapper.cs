using System.Threading.Tasks;
using Business.PayOne.Model.Requests;
using Core.Rest;

namespace Business.PayOne.Interfaces
{
    public interface IPayOneWrapper
    {
        Task<RestResult<string>> ExecutePayOneRequestAsync(RequestBase requestDto);
    }
}