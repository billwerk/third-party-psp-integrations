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
        [Route("webhooks")]
        public Task<ObjectResult> Post([FromBody] string requestString)
        {
            return _paymentService.HandleWebhookAsync(requestString);
        }
    }
}