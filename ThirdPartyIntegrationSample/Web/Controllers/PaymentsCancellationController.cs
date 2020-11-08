using System;
using System.Threading.Tasks;
using Billwerk.Payment.SDK.DTO.ExternalIntegration;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Cancellation;
using Business.Enums;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    [ApiController]
    public class PaymentsCancellationController : ApiControllerBase
    {
        //TODO move to api/payone
        
        private readonly IPaymentServiceWrapper _paymentService;

        public PaymentsCancellationController(IPaymentServiceMethodsExecutor paymentServiceMethodsExecutor, IPaymentServiceWrapper paymentServiceWrapper) 
            : base(paymentServiceMethodsExecutor)
        {
            _paymentService = paymentServiceWrapper;
        }

        [HttpPost]
        [Route("api/payment/{id}/cancel")]
        public async Task<ObjectResult> Cancel(string id, ExternalPaymentCancellationRequestDTO dto)
        {
            //TODO review implementation 
            if (id != dto.TransactionId)
            {
                throw new ArgumentException("id != dto.TransactionId!"); 
            }
            
            return BuildResponse(await _paymentService.SendCancellation(PaymentServiceProvider.PayOne, dto));
        }
    }
}