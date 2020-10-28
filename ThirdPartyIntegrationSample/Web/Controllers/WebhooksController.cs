using System.IO;
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
        public ObjectResult Post()
        {
            using var streamReader = new StreamReader(Request.Body);
            var requestString = streamReader.ReadToEndAsync().Result;
            return _paymentService.HandleWebhookAsync(requestString);
        }
    }
}