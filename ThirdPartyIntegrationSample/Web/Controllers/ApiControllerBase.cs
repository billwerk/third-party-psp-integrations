using Core.Rest;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected ObjectResult BuildResponseFromResult<TResult, TData>(TResult result)
            where TResult : ResultBase<TData>
        {
            if (result.HasError)
            {
                return BadRequest(result.Error);
            }
            else
            {
                return Ok(result.Data);
            }
        }
    }
}