using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    public class WebhooksController : ApiControllerBase
    {
        private readonly IPaymentServiceWrapper _paymentService;

        public WebhooksController(IPaymentServiceMethodsExecutor paymentServiceMethodsExecutor,
            IPaymentServiceWrapper paymentService) : base(paymentServiceMethodsExecutor)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        [Route("api/webhooks")]
        public async Task<ObjectResult> Post([FromBody] string requestString)
        {
            var result = await _paymentService.HandleWebhookAsync(requestString);
            return Ok(new ByteArrayContent(new UTF8Encoding().GetBytes("TSOK")));
        }
    }
}