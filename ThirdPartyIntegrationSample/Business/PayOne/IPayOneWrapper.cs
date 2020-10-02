using System.Threading.Tasks;
using Business.PayOne.Model;
using Business.PayOne.Model.Requests;
using Core.Rest;

namespace Business.PayOne
{
    public interface IPayOneWrapper
    {
        Task<RestResult<string>> ExecutePayOneRequestAsync(RequestBase requestDto);
    }
}